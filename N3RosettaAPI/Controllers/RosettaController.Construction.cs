using Akka.Actor;
using Neo.Cryptography;
using Neo.Cryptography.ECC;
using Neo.IO;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.Network.P2P;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.Wallets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using static Neo.Ledger.Blockchain;
using NeoTransaction = Neo.Network.P2P.Payloads.Transaction;

namespace Neo.Plugins
{
    // derive public key: derive
    // make transaction sequence: preprocess -> metadata -> payloads -> parse -> combine -> parse -> hash -> submit
    partial class RosettaController
    {
        /// <summary>
        /// Combine creates a network-specific transaction from an unsigned transaction and an array of provided signatures. 
        /// The signed transaction returned from this method will be sent to the `/construction/submit` endpoint by the caller.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JObject ConstructionCombine(ConstructionCombineRequest request)
        {
            if (request.NetworkIdentifier.Blockchain.ToLower() != "neo n3")
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();

            NeoTransaction neoTx;
            try
            {
                byte[] txRaw = Convert.FromBase64String(request.UnsignedTransaction); // use base64 string in N3
                neoTx = txRaw.AsSerializable<NeoTransaction>();
            }
            catch (Exception)
            {
                return Error.TX_DESERIALIZE_ERROR.ToJson();
            }

            if (neoTx.Witnesses == null || neoTx.Witnesses.Length != neoTx.Signers.Length)
            {
                return Error.TX_WITNESSES_INVALID.ToJson();
            }


            if (request.Signatures.Length == 0)
                return Error.NO_SIGNATURE.ToJson();

            var signData = neoTx.GetSignData(system.Settings.Network);
            for (int i = 0; i < neoTx.Signers.Length; i++)
            {
                var witness = neoTx.Witnesses[i];
                if (witness.VerificationScript.IsSignatureContract())
                {
                    var sign = request.Signatures.First(s =>
                        s.PublicKey.AddressHash == neoTx.Signers[i].Account);
                    CreateSignatureWitness(witness, sign, signData);
                }
                else if (witness.VerificationScript.IsMultiSigContract(out var m, out ECPoint[] points))
                {
                    var pubKeys = points.OrderBy(p => p).Select(p => p.ToUInt160FromPublicKey()).ToList();
                    var signs = new List<byte[]>();
                    var count = 0;
                    foreach (var pubkeyhash in pubKeys)
                    {
                        if (count == m) break;
                        var sign = request.Signatures.First(s => s.PublicKey.AddressHash == pubkeyhash);
                        signs.Add(sign.HexBytes.HexToBytes());
                        count++;
                    }
                    var mulWitness = new Witness();
                    using (ScriptBuilder sb = new ScriptBuilder())
                    {
                        signs.ForEach(p => sb.EmitPush(p));
                        witness.InvocationScript = sb.ToArray();
                    }
                }
            }

            byte[] signed = neoTx.ToArray();
            ConstructionCombineResponse response = new ConstructionCombineResponse(Convert.ToBase64String(signed));
            return response.ToJson();
        }

        /// <summary>
        /// Derive returns the network-specific address associated with a public key. 
        /// Blockchains that require an on-chain action to create an account should not implement this method.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JObject ConstructionDerive(ConstructionDeriveRequest request)
        {
            if (request.NetworkIdentifier.Blockchain.ToLower() != "neo n3")
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();

            if (request.PublicKey.CurveType != CurveType.Secp256r1)
                return Error.CURVE_NOT_SUPPORTED.ToJson();

            ECPoint pubKey;
            try
            {
                pubKey = ECPoint.FromBytes(request.PublicKey.HexBytes.HexToBytes(), ECCurve.Secp256r1);
            }
            catch (Exception)
            {
                return Error.INVALID_PUBLIC_KEY.ToJson();
            }

            string address = Contract.CreateSignatureRedeemScript(pubKey).ToScriptHash().ToAddress(system.Settings.AddressVersion);
            ConstructionDeriveResponse response = new ConstructionDeriveResponse(new AccountIdentifier(address));
            return response.ToJson();
        }

        /// <summary>
        /// Hash returns the network-specific transaction hash for a signed transaction.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JObject ConstructionHash(ConstructionHashRequest request)
        {
            if (request.NetworkIdentifier.Blockchain.ToLower() != "neo n3")
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();

            NeoTransaction neoTx;
            try
            {
                neoTx = Convert.FromBase64String(request.SignedTransaction).AsSerializable<NeoTransaction>(); // base64
            }
            catch (Exception)
            {
                return Error.TX_DESERIALIZE_ERROR.ToJson();
            }
            var hash = neoTx.Hash.ToString();
            ConstructionHashResponse response = new ConstructionHashResponse(new TransactionIdentifier(hash));
            return response.ToJson();
        }

        /// <summary>
        /// Get any information required to construct a transaction for a specific network. 
        /// Metadata returned here could be a recent hash to use, an account sequence number, or even arbitrary chain state. 
        /// The request used when calling this endpoint is often created by calling `/construction/preprocess` in an offline environment. 
        /// You should NEVER assume that the request sent to this endpoint will be created by the caller or populated with any custom parameters. This must occur in /construction/preprocess.
        /// It is important to clarify that this endpoint should not pre-construct any transactions for the client (this should happen in `/construction/payloads`). 
        /// This endpoint is left purposely unstructured because of the wide scope of metadata that could be required.
        /// </summary>
        /// <returns></returns>
        public JObject ConstructionMetadata(ConstructionMetadataRequest request)
        {
            Console.WriteLine($"ConstructionMetadata Start");
            if (request.NetworkIdentifier.Blockchain.ToLower() != "neo n3")
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();

            // get signers
            Signer[] signers = Array.Empty<Signer>();
            try
            {
                signers = (request.Options["signers"] as JArray).Select(p => p.ToSigner(system.Settings.AddressVersion)).ToArray();
            }
            catch (Exception)
            {
                return Error.PARAMETER_INVALID.ToJson();
            }

            // get public keys, need public keys to calculate network fee
            if (!request.Options.ContainsKey("signer_metadata"))
                return Error.PARAMETER_INVALID.ToJson();
            SignerMetadata[] signerMetadatas = (request.Options["signer_metadata"] as JArray).Select(p => SignerMetadata.FromJson(p, system.Settings.AddressVersion)).ToArray();
            if (request.PublicKeys == null)
                return Error.PARAMETER_INVALID.ToJson();

            // get script
            byte[] script = Array.Empty<byte>();
            if (request.Options.ContainsKey("has_existing_script") && request.Options["has_existing_script"].AsBoolean() == true)
            {
                if (request.Options.ContainsKey("script"))
                    script = Convert.FromBase64String(request.Options["script"].AsString());
                else
                    return Error.PARAMETER_INVALID.ToJson();
            }
            else if (request.Options.ContainsKey("has_existing_script") && request.Options["has_existing_script"].AsBoolean() == false)
            {
                if (request.Options.ContainsKey("operations_script"))
                    script = Convert.FromBase64String(request.Options["operations_script"].AsString());
                else
                    return Error.PARAMETER_INVALID.ToJson();
            }
            else
                return Error.PARAMETER_INVALID.ToJson();

            // get max fee
            long maxFee = 2000000000;
            if (request.Options.ContainsKey("max_fee"))
                maxFee = long.Parse(request.Options["max_fee"].AsString());

            // invoke script
            NeoTransaction tx = new NeoTransaction
            {
                Signers = signers,
                Attributes = Array.Empty<TransactionAttribute>(),
            };
            using ApplicationEngine engine = ApplicationEngine.Run(script, system.StoreView, container: tx, settings: system.Settings, gas: maxFee);
            if (engine.State == VMState.FAULT)
                return Error.VM_FAULT.ToJson();

            // script
            tx.Script = script;

            // version
            var version = (byte)0;
            tx.Version = version;

            // nonce
            Random random = new();
            var nonce = (uint)random.Next();
            tx.Nonce = nonce;

            // system fee
            var sysFee = engine.GasConsumed;
            tx.SystemFee = sysFee;

            // network fee is not available here, will be calculated in 
            var netFee = CalculateNetworkFee(system.StoreView, tx, request.PublicKeys, signerMetadatas);

            // valid until block
            var validUntilBlock = NativeContract.Ledger.CurrentIndex(system.StoreView) + system.Settings.MaxValidUntilBlockIncrement;

            Metadata metadata = new(new()
            {
                { "version", version },
                { "nonce", nonce },
                { "sysfee", sysFee },
                { "netfee", netFee },
                { "validuntilblock", validUntilBlock },
                { "script", Convert.ToBase64String(script) },
                { "signers", request.Options["signers"] },
                { "signer_metadata", request.Options["signer_metadata"] }
            });

            var suggeustFee = new Amount[] {
                (netFee+sysFee).ToGasAmount(),
            };

            ConstructionMetadataResponse response = new ConstructionMetadataResponse(metadata, suggeustFee);
            return response.ToJson();
        }

        /// <summary>
        /// Parse is called on both unsigned and signed transactions to understand the intent of the formulated transaction. 
        /// This is run as a sanity check before signing (after `/construction/payloads`) and before broadcast (after `/construction/combine`).
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JObject ConstructionParse(ConstructionParseRequest request)
        {
            if (request.NetworkIdentifier.Blockchain.ToLower() != "neo n3")
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();

            NeoTransaction neoTx;
            try
            {
                if (request.Signed)
                {
                    neoTx = Convert.FromBase64String(request.Transaction).AsSerializable<NeoTransaction>(); // base64
                }
                else
                {
                    neoTx = new NeoTransaction();
                    byte[] rawTx = Convert.FromBase64String(request.Transaction);
                    using MemoryStream ms = new MemoryStream(rawTx, false);
                    using BinaryReader br = new BinaryReader(ms, Encoding.UTF8);
                    neoTx.DeserializeUnsigned(br);
                    neoTx.Witnesses = Array.Empty<Witness>();
                }
            }
            catch (Exception)
            {
                return Error.TX_DESERIALIZE_ERROR.ToJson();
            }

            Transaction tx = ConvertTx(neoTx);
            Operation[] operations = tx.Operations; // empty, 
            string[] signers = Array.Empty<string>(); // signers are included in metadata
            if (request.Signed)
            {
                //signers = neoTx.Signers.Select(s => s.Account.ToAddress(system.Settings.AddressVersion)).ToArray();
                signers = GetRequiredSigners(neoTx).Select(s => s.ToAddress(system.Settings.AddressVersion)).ToArray();
            }
            ConstructionParseResponse response = new ConstructionParseResponse(operations, signers, tx.Metadata);
            return response.ToJson();
        }

        /// <summary>
        /// Payloads is called with an array of operations and the response from `/construction/metadata`. 
        /// It returns an unsigned transaction blob and a collection of payloads that must be signed by particular addresses using a certain SignatureType. 
        /// The array of operations provided in transaction construction often times can not specify all "effects" of a transaction (consider invoked transactions in Ethereum). 
        /// However, they can deterministically specify the "intent" of the transaction, which is sufficient for construction. 
        /// For this reason, parsing the corresponding transaction in the Data API (when it lands on chain) will contain a superset of whatever operations were provided during construction.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JObject ConstructionPayloads(ConstructionPayloadsRequest request)
        {
            if (request.NetworkIdentifier.Blockchain.ToLower() != "neo n3")
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();

            // The Operations in the request of this method won't be handled again because they are already handled in `/construction/preprocess`
            NeoTransaction neoTx;
            UInt160[] pendingSigners = new UInt160[0];
            try
            {
                neoTx = new();
                neoTx.Script = Convert.FromBase64String(request.Metadata["script"].AsString());
                neoTx.Version = (byte)request.Metadata["version"].GetInt32();
                neoTx.Nonce = (uint)request.Metadata["nonce"].GetInt32();
                neoTx.SystemFee = (long)request.Metadata["sysfee"].GetNumber();
                neoTx.NetworkFee = (long)request.Metadata["netfee"].GetNumber();
                neoTx.ValidUntilBlock = (uint)request.Metadata["validuntilblock"].GetInt32();
                neoTx.Signers = (request.Metadata["signers"] as JArray).Select(p => p.ToSigner(system.Settings.AddressVersion)).ToArray();
                neoTx.Attributes = Array.Empty<TransactionAttribute>();

                //PublicKeyUsage[] publicKeyUsages = (request.Metadata["public_key_usages"] as JArray).Select(p => PublicKeyUsage.FromJson(p)).ToArray();
                SignerMetadata[] signerMetadatas = (request.Metadata["signer_metadata"] as JArray).Select(p => SignerMetadata.FromJson(p, system.Settings.AddressVersion)).ToArray();

                if (signerMetadatas.Length > 0)
                {
                    if (request.PublicKeys == null)
                        return Error.PARAMETER_INVALID.ToJson();

                    (neoTx.Witnesses, pendingSigners) = CreateWitness(system.StoreView, neoTx, request.PublicKeys, signerMetadatas);
                }
                // no witnesses
            }
            catch (Exception)
            {
                return Error.PARAMETER_INVALID.ToJson();
            }

            byte[] txRaw = neoTx.ToArray();
            //var scriptHashes = neoTx.GetScriptHashesForVerifying(system.StoreView);

            SigningPayload[] signingPayloads = new SigningPayload[pendingSigners.Length];
            for (int i = 0; i < pendingSigners.Length; i++)
            {
                var account = new AccountIdentifier(pendingSigners[i].ToAddress(system.Settings.AddressVersion));
                signingPayloads[i] = new SigningPayload(neoTx.GetSignData(system.Settings.Network).Sha256().ToHexString(), account);
            }
            ConstructionPayloadsResponse response = new ConstructionPayloadsResponse(Convert.ToBase64String(txRaw), signingPayloads);
            return response.ToJson();
        }

        /// <summary>
        /// Preprocess is called prior to `/construction/payloads` to construct a request for any metadata that is needed for transaction construction given (i.e. account nonce). 
        /// The request returned from this method will be used by the caller (in a different execution environment) to call the `/construction/metadata` endpoint.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JObject ConstructionPreprocess(ConstructionPreprocessRequest request)
        {
            if (request.NetworkIdentifier.Blockchain.ToLower() != "neo n3")
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();

            // this method is used to create metadata required for the construction of tx in an offline environment
            // find signers (who pay the fees) in request.Metadata
            if (request.Metadata == null)
                return Error.PARAMETER_INVALID.ToJson();
            if (!request.Metadata.ContainsKey("signers"))
                return Error.PARAMETER_INVALID.ToJson();

            Dictionary<string, JObject> pairs = new();
            pairs.Add("signers", request.Metadata["signers"]);
            pairs.Add("signer_metadata", request.Metadata["signer_metadata"]);


            // get script from either existing script in metadata or converting from operations
            byte[] script = Array.Empty<byte>();
            if (request.Metadata.ContainsKey("script"))
            {
                pairs.Add("has_existing_script", true);
                pairs.Add("script", request.Metadata["script"]);
            }
            else
            {
                pairs.Add("has_existing_script", false);
                try
                {
                    script = ConvertOperationsToScript(request.Operations);
                }
                catch (Exception)
                {
                    return Error.PARAMETER_INVALID.ToJson();
                }
                pairs.Add("operations_script", Convert.ToBase64String(script));
            }

            // get max fee
            Amount[] amounts = request.MaxFee;
            if (amounts != null && amounts.Length > 0)
            {
                if (amounts[0].Currency.Symbol.ToLower() == "gas")
                {
                    var maxFee = long.Parse(amounts[0].Value);
                    pairs.Add("max_fee", maxFee);
                }
            }

            Metadata options = new(pairs);          
            SignerMetadata[] signerMetadatas = (request.Metadata["signer_metadata"] as JArray).Select(p => SignerMetadata.FromJson(p, system.Settings.AddressVersion)).ToArray();
            AccountIdentifier[] publicKeys = GetRequiredSigners(signerMetadatas);
            ConstructionPreprocessResponse response = new(options, publicKeys);
            return response.ToJson();
        }



        /// <summary>
        /// Submit a pre-signed transaction to the node. This call should not block on the transaction being included in a block. 
        /// Rather, it should return immediately with an indication of whether or not the transaction was included in the mempool. 
        /// The transaction submission response should only return a 200 status if the submitted transaction could be included in the mempool. 
        /// Otherwise, it should return an error.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JObject ConstructionSubmit(ConstructionSubmitRequest request)
        {
            if (request.NetworkIdentifier.Blockchain.ToLower() != "neo n3")
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();

            NeoTransaction neoTx;
            try
            {
                neoTx = Convert.FromBase64String(request.SignedTransaction).AsSerializable<NeoTransaction>();
            }
            catch (Exception)
            {
                return Error.TX_DESERIALIZE_ERROR.ToJson();
            }

            RelayResult result = system.Blockchain.Ask<RelayResult>(neoTx).Result;

            switch (result.Result)
            {
                case VerifyResult.Succeed:
                    TransactionIdentifier transactionIdentifier = new TransactionIdentifier(neoTx.Hash.ToString());
                    ConstructionSubmitResponse response = new ConstructionSubmitResponse(transactionIdentifier);
                    return response.ToJson();
                case VerifyResult.AlreadyExists:
                    return Error.ALREADY_EXISTS.ToJson();
                case VerifyResult.OutOfMemory:
                    return Error.OUT_OF_MEMORY.ToJson();
                case VerifyResult.UnableToVerify:
                    return Error.UNABLE_TO_VERIFY.ToJson();
                case VerifyResult.Invalid:
                    return Error.TX_INVALID.ToJson();
                case VerifyResult.Expired:
                    return Error.TX_EXPIRED.ToJson();
                case VerifyResult.InsufficientFunds:
                    return Error.INSUFFICIENT_FUNDS.ToJson();
                case VerifyResult.PolicyFail:
                    return Error.POLICY_FAIL.ToJson();
                default:
                    return Error.UNKNOWN_ERROR.ToJson();
            }
        }

        private AccountIdentifier[] GetRequiredSigners(SignerMetadata[] signerMetadatas)
        {
            var list = new HashSet<string>();
            foreach (var signerMetadata in signerMetadatas)
            {
                if (signerMetadata.RelatedAccounts.Length == 0)
                {
                    list.Add(signerMetadata.SignerAccount);
                }
                else
                {
                    foreach (var account in signerMetadata.RelatedAccounts)
                    {
                        list.Add(account);
                    }
                }
            }
            return list.Select(s => new AccountIdentifier(s)).ToArray();
        }

        private List<UInt160> GetRequiredSigners(NeoTransaction signedTx)
        {
            var signers = new List<UInt160>();

            foreach (var witness in signedTx.Witnesses)
            {
                if (witness.VerificationScript.IsSignatureContract())
                {
                    signers.Add(witness.VerificationScript.ToScriptHash()); ;
                }
                else if (witness.VerificationScript.IsMultiSigContract(out var m, out ECPoint[] points))
                {
                    signers.AddRange(points.Select(p => p.ToUInt160FromPublicKey()));
                }
            }
            return signers;
        }

        private (Witness[], UInt160[]) CreateWitness(DataCache snapshot, NeoTransaction tx, PublicKey[] publicKeys, SignerMetadata[] signerMetadatas)
        {
            UInt160[] hashes = tx.GetScriptHashesForVerifying(snapshot);
            var witnesses = new List<Witness>();
            var pendingSigners = new List<UInt160>();

            foreach (UInt160 hash in hashes)
            {
                var signerMetadata = signerMetadatas.First(p => p.SignerAccountHash == hash);
                if (signerMetadata.RelatedAccounts.Length == 0)
                {
                    var publicKey = publicKeys.First(p => p.AddressHash == signerMetadata.SignerAccountHash);
                    pendingSigners.Add(signerMetadata.SignerAccountHash);
                    witnesses.Add(new Witness()
                    {
                        VerificationScript = Contract.CreateSignatureRedeemScript(ECPoint.FromBytes(publicKey.HexBytes.HexToBytes(), ECCurve.Secp256r1))
                    });
                }
                else if (signerMetadata.RelatedAccounts.Length > 0)
                {
                    var relateAccounts = signerMetadata.RelatedAccounts.Select(a => a.ToUInt160(system.Settings.AddressVersion)).ToHashSet();
                    var pubKeys = publicKeys.Where(p => relateAccounts.Contains(p.HexBytes.ToUInt160FromPublicKey()));
                    pendingSigners.AddRange(pubKeys.Select(p => p.AddressHash));
                    witnesses.Add(new Witness()
                    {
                        VerificationScript = Contract.CreateMultiSigRedeemScript(signerMetadata.M, pubKeys.Select(p => ECPoint.FromBytes(p.HexBytes.HexToBytes(), ECCurve.Secp256r1)).ToArray())
                    });
                }
                else
                {
                    //We can support more contract types in the future.
                }
            }
            return (witnesses.ToArray(), pendingSigners.ToArray());
        }

        private long CalculateNetworkFee(DataCache snapshot, NeoTransaction tx, PublicKey[] publicKeys, SignerMetadata[] signers)
        {
            UInt160[] hashes = tx.GetScriptHashesForVerifying(snapshot);
            // base size for transaction: includes const_header + signers + attributes + script + hashes
            int size = NeoTransaction.HeaderSize + tx.Signers.GetVarSize() + tx.Attributes.GetVarSize() + tx.Script.GetVarSize() + IO.Helper.GetVarSize(hashes.Length);
            uint exec_fee_factor = NativeContract.Policy.GetExecFeeFactor(snapshot);
            long networkFee = 0;

            var publicKeyMap = publicKeys.ToDictionary(p => p.HexBytes.ToUInt160FromPublicKey(), p => p);
            // tx.Witnesses is empty
            foreach (UInt160 hash in hashes)
            {
                var pubKeys = new List<PublicKey>();
                var signer = signers.First(p => p.SignerAccount.ToUInt160(system.Settings.AddressVersion) == hash);
                if (signer.RelatedAccounts?.Length > 0 && signer.M > 0)
                {
                    foreach (var account in signer.RelatedAccounts)
                    {
                        var accountHash = account.ToUInt160(system.Settings.AddressVersion);
                        if (publicKeyMap.ContainsKey(accountHash))
                        {
                            pubKeys.Add(publicKeyMap[accountHash]);
                        }
                        else
                        {
                            throw new Exception($"Miss publickey for [{accountHash}] in [{hash}]");
                        }
                    }
                }
                else
                {
                    if (publicKeyMap.ContainsKey(hash))
                    {
                        pubKeys.Add(publicKeyMap[hash]);
                    }
                    else
                    {
                        throw new Exception($"Miss publickey for [{hash}]");
                    }
                }
                //var pubKeys = pubKeyUsage.KeyIndexs.Select(p => publicKeys.ElementAt(p)).ToArray();

                byte[] witness_script;

                if (pubKeys.Count == 1)
                {
                    witness_script = Contract.CreateSignatureRedeemScript(pubKeys.Select(p => ECPoint.FromBytes(p.HexBytes.HexToBytes(), ECCurve.Secp256r1)).First());
                    size += 67 + witness_script.GetVarSize();
                    networkFee += exec_fee_factor * Neo.SmartContract.Helper.SignatureContractCost();
                }
                else if (pubKeys.Count > 1)
                {
                    witness_script = Contract.CreateMultiSigRedeemScript(signer.M, pubKeys.Select(p => ECPoint.FromBytes(p.HexBytes.HexToBytes(), ECCurve.Secp256r1)).ToArray());
                    int size_inv = 66 * signer.M;
                    size += IO.Helper.GetVarSize(size_inv) + size_inv + witness_script.GetVarSize();
                    networkFee += exec_fee_factor * Neo.SmartContract.Helper.MultiSignatureContractCost(signer.M, pubKeys.Count);
                }
                else
                {
                    //We can support more contract types in the future.
                }
            }
            networkFee += size * NativeContract.Policy.GetFeePerByte(snapshot);
            return networkFee;
        }

     
        private byte[] ConvertOperationsToScript(Operation[] operations)
        {
            var n = operations.Length;
            if (n % 2 != 0) throw new ArgumentException("operation number must be even");
            var script = Array.Empty<byte>();
            using (ScriptBuilder sb = new())
            {
                for (int i = 0; i < n; i += 2)
                {
                    var fromOp = operations[i];
                    var toOp = operations[i + 1];
                    if (fromOp.OperationIdentifier.Index % 2 != 0 ||
                        toOp.OperationIdentifier.Index % 2 != 1 ||
                        fromOp.OperationIdentifier.Index + 1 != toOp.OperationIdentifier.Index ||
                        toOp.RelatedOperations[0].Index != fromOp.OperationIdentifier.Index ||
                        !fromOp.Amount.Equals(toOp.Amount))
                        throw new ArgumentException("invalid operations");

                    var from = fromOp.Account.Address.ToScriptHash(system.Settings.AddressVersion);
                    var to = toOp.Account.Address.ToScriptHash(system.Settings.AddressVersion);
                    var asset = UInt160.Parse(fromOp.Amount.Currency.Metadata["script_hash"].AsString());
                    var amount = BigInteger.Parse(fromOp.Amount.Value.TrimStart('-'));
                    sb.EmitDynamicCall(asset, "transfer", from, to, amount, "rosetta");
                }
                script = sb.ToArray();
            }
            return script;
        }

        private Witness CreateSignatureWitness(Witness witness, Signature signature, byte[] signData)
        {
            if (!VerifyKeyAndSignature(signature, signData, out ECPoint pubKey))
            {
                Console.WriteLine($"Verify Fail:{signature.HexBytes},{signData.ToHexString()},{pubKey.EncodePoint(false).ToHexString()}");
                return null;
            }

            //Witness witness = new Witness();
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush(signature.HexBytes.HexToBytes());
                witness.InvocationScript = sb.ToArray();
            }
            witness.VerificationScript = Contract.CreateSignatureRedeemScript(pubKey);

            return witness;
        }

        private Witness CreateMultiSignatureWitness(Signature[] signatures, byte[] signData)
        {
            ECPoint[] pubKeys = new ECPoint[signatures.Length];
            for (int i = 0; i < signatures.Length; i++)
            {
                if (!VerifyKeyAndSignature(signatures[i], signData, out ECPoint pubKey))
                    return null;
                pubKeys[i] = pubKey;
            }

            // sort public keys in ascending order, also match their signature
            Dictionary<ECPoint, Signature> dic = pubKeys.Select((p, i) => new
            {
                PubKey = p,
                Sig = signatures[i]
            }).OrderBy(p => p.PubKey).ToDictionary(p => p.PubKey, p => p.Sig);

            Witness witness = new Witness();
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                dic.Values.ToList().ForEach(p => sb.EmitPush(p.HexBytes.HexToBytes()));
                witness.InvocationScript = sb.ToArray();
            }
            witness.VerificationScript = Contract.CreateMultiSigRedeemScript(signatures.Length, dic.Keys.ToArray());

            return witness;
        }

        private bool VerifyKeyAndSignature(Signature signature, byte[] signData, out ECPoint pubKey)
        {
            pubKey = null;

            // 0. only support ecdsa and secp256k1 for now, hashing is sha256
            if (signature.SignatureType != SignatureType.Ecdsa || signature.PublicKey.CurveType != CurveType.Secp256r1)
                return false;

            // 1. check public key bytes
            try
            {
                pubKey = ECPoint.FromBytes(signature.PublicKey.HexBytes.HexToBytes(), ECCurve.Secp256r1);
            }
            catch (Exception)
            {
                return false;
            }

            // 2. check if public key matches address
            if (Contract.CreateSignatureRedeemScript(pubKey).ToScriptHash().ToAddress(system.Settings.AddressVersion) != signature.SigningPayload.AccountIdentifier.Address)
                return false;

            if (signature.SigningPayload.HexBytes != signData.Sha256().ToHexString())
            {
                Console.WriteLine($"SigningPayload.HexBytes: {signature.SigningPayload.HexBytes} not equal to {signData.ToHexString()}");
                return false;
            }
          
            // 3. check if public key and signature matches
            return Crypto.VerifySignature(signData, signature.HexBytes.HexToBytes(), pubKey);
        }
    }
}
