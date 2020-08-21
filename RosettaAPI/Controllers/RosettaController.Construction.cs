using Akka.Actor;
using Microsoft.AspNetCore.Mvc;
using Neo.Cryptography;
using Neo.Cryptography.ECC;
using Neo.IO;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.Network.P2P;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.VM;
using Neo.Wallets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NeoTransaction = Neo.Network.P2P.Payloads.Transaction;

namespace Neo.Plugins
{
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
            NeoTransaction neoTx;
            try
            {
                byte[] unsigned = request.UnsignedTransaction.HexToBytes();
                TransactionType type = (TransactionType)unsigned[0];
                switch (type)
                {
                    case TransactionType.ClaimTransaction:
                        neoTx = new ClaimTransaction();
                        break;
                    case TransactionType.ContractTransaction:
                        neoTx = new ContractTransaction();
                        break;
                    case TransactionType.StateTransaction:
                        neoTx = new StateTransaction();
                        break;
                    case TransactionType.InvocationTransaction:
                        neoTx = new InvocationTransaction();
                        break;
                    default:
                        throw new ArgumentException();
                }
                using (MemoryStream ms = new MemoryStream(unsigned, false))
                using (BinaryReader br = new BinaryReader(ms, Encoding.UTF8))
                {
                    (neoTx as IVerifiable).DeserializeUnsigned(br);
                }

            }
            catch (Exception)
            {
                return Error.TX_DESERIALIZE_ERROR.ToJson();
            }

            if (neoTx.Witnesses != null && neoTx.Witnesses.Length > 0)
                return Error.TX_ALREADY_SIGNED.ToJson();

            if (request.Signatures.Length == 0)
                return Error.NO_SIGNATURE.ToJson();

            Witness witness;
            if (request.Signatures.Length == 1) // single signed
                witness = CreateSignatureWitness(request.Signatures[0]);
            else
                witness = CreateMultiSignatureWitness(request.Signatures);

            if (witness is null)
                return Error.INVALID_SIGNATURES.ToJson();

            neoTx.Witnesses = new Witness[] { witness };
            byte[] signed = neoTx.ToArray();
            ConstructionCombineResponse response = new ConstructionCombineResponse(signed.ToHexString());
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
            if (request.PublicKey.CurveType != CurveType.Secp256r1)
                return Error.CURVE_NOT_SUPPORTED.ToJson();

            ECPoint pubKey;
            try
            {
                pubKey = ECPoint.FromBytes(request.PublicKey.Bytes, ECCurve.Secp256r1);
            }
            catch (Exception)
            {
                return Error.INVALID_PUBLIC_KEY.ToJson();
            }

            string address = ("21" + pubKey.EncodePoint(true).ToHexString() + "ac").HexToBytes().ToScriptHash().ToAddress();
            ConstructionDeriveResponse response = new ConstructionDeriveResponse(address);
            return response.ToJson();
        }

        /// <summary>
        /// Hash returns the network-specific transaction hash for a signed transaction.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JObject ConstructionHash(ConstructionHashRequest request)
        {
            NeoTransaction neoTx;
            try
            {
                neoTx = NeoTransaction.DeserializeFrom(request.SignedTransaction.HexToBytes());
            }
            catch (Exception)
            {
                return Error.TX_DESERIALIZE_ERROR.ToJson();
            }
            var hash = neoTx.Hash.ToString();
            ConstructionHashResponse response = new ConstructionHashResponse(hash);
            return response.ToJson();
        }

        /// <summary>
        /// Get any information required to construct a transaction for a specific network. Metadata returned here could be a recent hash to use, an account sequence number, or even arbitrary chain state. 
        /// The request used when calling this endpoint is often created by calling `/construction/preprocess` in an offline environment. 
        /// It is important to clarify that this endpoint should not pre-construct any transactions for the client (this should happen in `/construction/payloads`). 
        /// </summary>
        /// <returns></returns>
        public JObject ConstructionMetadata(ConstructionMetadataRequest request)
        {
            // check params
            if (request.Options is null || request.Options.Pairs is null ||
                !request.Options.ContainsKey("tx_type"))
                return Error.PARAMETER_INVALID.ToJson();
            if (!request.Options.TryGetValue("tx_type", out JObject txType))
                return Error.PARAMETER_INVALID.ToJson();

            Metadata metadata;
            try
            {
                switch (txType.AsString())
                {
                    case nameof(ClaimTransaction):
                        {
                            JObject json = new JObject();
                            json["type"] = "CoinReference[]";
                            json["required"] = "true";
                            json["reference"] = @"https://github.com/neo-project/neo/blob/master-2.x/neo/Network/P2P/Payloads/CoinReference.cs";
                            json["example"] = "[{\"txid\": \"0xe1e44f41a1f0854063ccdc9beb7537fc40565575e0ae2366b4a93a73c18b6166\", \"vout\": 2},{\"txid\": \"0x69fd452c92fb0e5861b27588549a8e55d3f9fee542884ae317600508bbacedbb\",\"vout\": 2}]";
                            metadata = new Metadata(new Dictionary<string, JObject>
                            {
                                { "tx_type", nameof(ClaimTransaction) },
                                { "claims", json }
                            });
                            break;
                        }
                    case nameof(ContractTransaction):
                        {
                            metadata = new Metadata(new Dictionary<string, JObject>
                            {
                                { "tx_type", nameof(ContractTransaction) }
                            });
                            break;
                        }
                    case nameof(InvocationTransaction):
                        {
                            JObject script = new JObject();
                            script["type"] = "byte[] (represented in hex string)";
                            script["required"] = "true";
                            script["example"] = "00046e616d656724058e5e1b6008847cd662728549088a9ee82191";
                            JObject gas = new JObject();
                            gas["type"] = "Fixed8";
                            gas["required"] = "false";
                            gas["reference"] = @"https://github.com/neo-project/neo/blob/master-2.x/neo/Fixed8.cs";
                            gas["example"] = "1234567890";
                            metadata = new Metadata(new Dictionary<string, JObject>
                            {
                                { "tx_type", nameof(InvocationTransaction) },
                                { "script", script },
                                { "gas", gas }
                            });
                            break;
                        }
                    case nameof(StateTransaction):
                        {
                            JObject json = new JObject();
                            json["type"] = "StateDescriptor[]";
                            json["required"] = "true";
                            json["reference"] = @"https://docs.neo.org/tutorial/zh-cn/3-transactions/3-NEO_transaction_types.html#statetransaction";
                            json["example"] = "[{\"type\":\"Account\",\"key\":\"ce47912d51b7ed8a24a77f97f2cb6ef9a0e0bf0d\",\"field\":\"Votes\",\"value\":\"04024c7b7fb6c310fccf1ba33b082519d82964ea93868d676662d4a59ad548df0e7d02aaec38470f6aad0042c6e877cfd8087d2676b0f516fddd362801b9bd3936399e02ca0e27697b9c248f6f16e085fd0061e26f44da85b58ee835c110caa5ec3ba55402df48f60e8f3e01c48ff40b9b7f1310d7a8b2a193188befe1c2e3df740e895093\"}]";
                            metadata = new Metadata(new Dictionary<string, JObject>
                            {
                                { "tx_type", nameof(StateTransaction) },
                                { "descriptors", json }
                            });
                            break;
                        }
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception)
            {
                return Error.PARAMETER_INVALID.ToJson();
            }

            ConstructionMetadataResponse response = new ConstructionMetadataResponse(metadata);
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
            NeoTransaction neoTx;
            try
            {
                if (request.Signed)
                {
                    neoTx = NeoTransaction.DeserializeFrom(request.Transaction.HexToBytes());
                }
                else
                {
                    byte[] rawTx = request.Transaction.HexToBytes();
                    TransactionType type = (TransactionType)rawTx[0];
                    switch (type)
                    {
                        case TransactionType.ClaimTransaction:
                            neoTx = new ClaimTransaction();
                            break;
                        case TransactionType.ContractTransaction:
                            neoTx = new ContractTransaction();
                            break;
                        case TransactionType.StateTransaction:
                            neoTx = new StateTransaction();
                            break;
                        case TransactionType.InvocationTransaction:
                            neoTx = new InvocationTransaction();
                            break;
                        default:
                            throw new ArgumentException();
                    }
                    using (MemoryStream ms = new MemoryStream(rawTx, false))
                    using (BinaryReader br = new BinaryReader(ms, Encoding.UTF8))
                    {
                        (neoTx as IVerifiable).DeserializeUnsigned(br);
                    }
                }
            }
            catch (Exception)
            {
                return Error.TX_DESERIALIZE_ERROR.ToJson();
            }

            Transaction tx = ConvertTx(neoTx);
            Operation[] operations = tx.Operations;
            string[] signers = new string[0];
            if (request.Signed)
            {
                signers = GetSignersFromWitnesses(neoTx.Witnesses);
                if (signers is null)
                    return Error.TX_WITNESS_INVALID.ToJson();
            }
            ConstructionParseResponse response = new ConstructionParseResponse(operations, signers, tx.Metadata);
            return response.ToJson();
        }

        /// <summary>
        /// Payloads is called with an array of operations and the response from `/construction/metadata`. 
        /// It returns an unsigned transaction blob and a collection of payloads that must be signed by particular addresses using a certain SignatureType. 
        /// The array of operations provided in transaction construction often times can not specify all "effects" of a transaction. 
        /// However, they can deterministically specify the "intent" of the transaction, which is sufficient for construction. 
        /// For this reason, parsing the corresponding transaction in the Data API (when it lands on chain) will contain a superset of whatever operations were provided during construction.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JObject ConstructionPayloads(ConstructionPayloadsRequest request)
        {
            NeoTransaction neoTx;
            try
            {
                neoTx = ConvertOperations(request.Operations, request.Metadata);
            }
            catch (Exception)
            {
                return Error.PARAMETER_INVALID.ToJson();
            }
            byte[] unsignedRaw = (neoTx as IVerifiable).GetHashData();
            var scriptHashes = neoTx.GetScriptHashesForVerifying(Blockchain.Singleton.GetSnapshot());
            SigningPayload[] signingPayloads = new SigningPayload[scriptHashes.Length];
            for (int i = 0; i < scriptHashes.Length; i++)
            {
                signingPayloads[i] = new SigningPayload(scriptHashes[i].ToAddress(), unsignedRaw);
            }
            ConstructionPayloadsResponse response = new ConstructionPayloadsResponse(unsignedRaw.ToHexString(), signingPayloads);
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
            // the metadata in request should include tx type and type specific info
            Metadata metadata = request.Metadata;
            if (metadata is null || metadata.Pairs is null || metadata.Pairs.Count == 0)
                return Error.TX_METADATA_INVALID.ToJson();
            if (!metadata.TryGetValue("tx_type", out JObject txType))
                return Error.TX_METADATA_INVALID.ToJson();

            try
            {
                switch (txType.AsString())
                {
                    case nameof(ClaimTransaction):
                        JArray coins = metadata["claims"] as JArray;
                        CoinReference[] coinReferences = coins.Select(p => new CoinReference()
                        {
                            PrevHash = UInt256.Parse(p["txid"].AsString()),
                            PrevIndex = ushort.Parse(p["vout"].AsString())
                        }).ToArray();
                        break;
                    case nameof(ContractTransaction):
                        break;
                    case nameof(InvocationTransaction):
                        byte[] script = metadata["script"].AsString().HexToBytes();
                        Fixed8 gas = Fixed8.Parse(metadata["gas"].AsString());
                        break;
                    case nameof(StateTransaction):
                        JArray descriptors = metadata["descriptors"] as JArray;
                        StateDescriptor[] stateDescriptors = descriptors.Select(p => new StateDescriptor()
                        {
                            Type = p["type"].TryGetEnum<StateType>(),
                            Key = p["key"].AsString().HexToBytes(),
                            Field = p["field"].AsString(),
                            Value = p["value"].AsString().HexToBytes()
                        }).ToArray();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch (Exception)
            {
                return Error.TX_METADATA_INVALID.ToJson();
            }
            ConstructionPreprocessResponse response = new ConstructionPreprocessResponse(request.Metadata);
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
            NeoTransaction neoTx;
            try
            {
                neoTx = NeoTransaction.DeserializeFrom(request.SignedTransaction.HexToBytes());
            }
            catch (Exception)
            {
                return Error.TX_DESERIALIZE_ERROR.ToJson();
            }

            RelayResultReason reason = system.Blockchain.Ask<RelayResultReason>(neoTx).Result;

            switch (reason)
            {
                case RelayResultReason.Succeed:
                    TransactionIdentifier transactionIdentifier = new TransactionIdentifier(neoTx.Hash.ToString());
                    ConstructionSubmitResponse response = new ConstructionSubmitResponse(transactionIdentifier);
                    return response.ToJson();
                case RelayResultReason.AlreadyExists:
                    return Error.ALREADY_EXISTS.ToJson();
                case RelayResultReason.OutOfMemory:
                    return Error.OUT_OF_MEMORY.ToJson();
                case RelayResultReason.UnableToVerify:
                    return Error.UNABLE_TO_VERIFY.ToJson();
                case RelayResultReason.Invalid:
                    return Error.TX_INVALID.ToJson();
                case RelayResultReason.PolicyFail:
                    return Error.POLICY_FAIL.ToJson();
                default:
                    return Error.UNKNOWN_ERROR.ToJson();
            }
        }

        private NeoTransaction ConvertOperations(Operation[] operations, Metadata metadata)
        {
            TransactionType type = metadata["tx_type"].AsString().ToTransactionType();

            // operations only contains utxo transfers, and in a special order
            List<CoinReference> inputs = new List<CoinReference>();
            List<TransactionOutput> outputs = new List<TransactionOutput>();
            for (int i = 0; i < operations.Length; i++)
            {
                var operation = operations[i];
                // handle from ops, CoinChange field should have values
                if (operation.RelatedOperations is null)
                {
                    if (operation.CoinChange is null || operation.CoinChange.CoinAction != CoinAction.CoinSpent)
                        throw new ArgumentException();
                    var coin = operation.CoinChange;
                    inputs.Add(new CoinReference()
                    {
                        PrevHash = coin.CoinIdentifier.GetTxHash(),
                        PrevIndex = (ushort)coin.CoinIdentifier.GetIndex()
                    });
                }
                else // handle to ops, CoinChange field may be null
                {
                    string symbol = operation.Amount.Currency.Symbol.ToUpper();
                    UInt256 assetId = symbol == "NEO" ? Blockchain.GoverningToken.Hash : symbol == "GAS" ? Blockchain.UtilityToken.Hash : throw new NotSupportedException();
                    Fixed8 value = Fixed8.Parse(operation.Amount.Value);
                    UInt160 scriptHash = operation.Account.Address.ToScriptHash();
                    outputs.Add(new TransactionOutput()
                    {
                        AssetId = assetId,
                        Value = value,
                        ScriptHash = scriptHash
                    });
                }
            }

            NeoTransaction neoTx;
            // fetch exclusive data for each tx type
            switch (type)
            {
                case TransactionType.ClaimTransaction:
                    neoTx = new ClaimTransaction();
                    JArray coins = metadata["claims"] as JArray;
                    CoinReference[] coinReferences = coins.Select(p => new CoinReference()
                    {
                        PrevHash = UInt256.Parse(p["txid"].AsString()),
                        PrevIndex = ushort.Parse(p["vout"].AsString())
                    }).ToArray();
                    ((ClaimTransaction)neoTx).Claims = coinReferences;
                    break;
                case TransactionType.ContractTransaction:
                    neoTx = new ContractTransaction();
                    break;
                case TransactionType.InvocationTransaction:
                    neoTx = new InvocationTransaction();
                    byte[] script = metadata["script"].AsString().HexToBytes();
                    Fixed8 gas = Fixed8.Parse(metadata["gas"].AsString());
                    ((InvocationTransaction)neoTx).Script = script;
                    ((InvocationTransaction)neoTx).Gas = gas;
                    break;
                case TransactionType.StateTransaction:
                    neoTx = new StateTransaction();
                    JArray descriptors = metadata["descriptors"] as JArray;
                    StateDescriptor[] stateDescriptors = descriptors.Select(p => new StateDescriptor()
                    {
                        Type = p["type"].TryGetEnum<StateType>(),
                        Key = p["key"].AsString().HexToBytes(),
                        Field = p["field"].AsString(),
                        Value = p["value"].AsString().HexToBytes()
                    }).ToArray();
                    ((StateTransaction)neoTx).Descriptors = stateDescriptors;
                    break;
                default:
                    throw new NotSupportedException();
            }
            neoTx.Inputs = inputs.ToArray();
            neoTx.Outputs = outputs.ToArray();
            neoTx.Attributes = new TransactionAttribute[] { };
            return neoTx;
        }

        private string[] GetSignersFromWitnesses(Witness[] witnesses)
        {
            List<ECPoint> pubKeys = new List<ECPoint>();
            try
            {
                foreach (var witness in witnesses)
                {
                    byte[] script = witness.VerificationScript;
                    if (script.IsSignatureContract())
                        pubKeys.Add(ECPoint.DecodePoint(script.Skip(1).Take(33).ToArray(), ECCurve.Secp256r1));
                    else if (script.IsMultiSigContract())
                    {
                        int i = 0;
                        switch (script[i++])
                        {
                            case 1:
                                ++i;
                                break;
                            case 2:
                                i += 2;
                                break;
                        }
                        while (script[i++] == 33)
                        {
                            pubKeys.Add(ECPoint.DecodePoint(script.Skip(i).Take(33).ToArray(), ECCurve.Secp256r1));
                            i += 33;
                        }
                    }
                    else
                        throw new NotSupportedException();
                }
            }
            catch (Exception)
            {
                return null;
            }
            return pubKeys.Select(p => ("21" + p.EncodePoint(true).ToHexString() + "ac").HexToBytes().ToScriptHash().ToAddress()).ToArray();
        }

        private Witness CreateSignatureWitness(Signature signature)
        {
            if (!VerifyKeyAndSignature(signature, out ECPoint pubKey))
                return null;

            Witness witness = new Witness();
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush(signature.Bytes);
                witness.InvocationScript = sb.ToArray();
            }
            witness.VerificationScript = Contract.CreateSignatureRedeemScript(pubKey);

            return witness;
        }

        private Witness CreateMultiSignatureWitness(Signature[] signatures)
        {
            ECPoint[] pubKeys = new ECPoint[signatures.Length];
            for (int i = 0; i < signatures.Length; i++)
            {
                if (!VerifyKeyAndSignature(signatures[i], out ECPoint pubKey))
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
                dic.Values.ToList().ForEach(p => sb.EmitPush(p.Bytes));
                witness.InvocationScript = sb.ToArray();
            }
            witness.VerificationScript = Contract.CreateMultiSigRedeemScript(signatures.Length, dic.Keys.ToArray());

            return witness;
        }

        private bool VerifyKeyAndSignature(Signature signature, out ECPoint pubKey)
        {
            pubKey = null;

            // 0. only support ecdsa and secp256k1 for now, hashing is sha256
            if (signature.SignatureType != SignatureType.Ecdsa || signature.PublicKey.CurveType != CurveType.Secp256r1)
                return false;

            // 1. check public key bytes
            try
            {
                pubKey = ECPoint.FromBytes(signature.PublicKey.Bytes, ECCurve.Secp256r1);
            }
            catch (Exception)
            {
                return false;
            }

            // 2. check if public key matches address
            if (("21" + pubKey.EncodePoint(true).ToHexString() + "ac").HexToBytes().ToScriptHash().ToAddress() != signature.SigningPayload.Address)
                return false;

            // 3. check if public key and signature matches
            return Crypto.Default.VerifySignature(signature.SigningPayload.Bytes, signature.Bytes, pubKey.EncodePoint(false));
        }

    }
}
