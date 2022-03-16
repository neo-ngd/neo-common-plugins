using Microsoft.AspNetCore.Mvc;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.VM;
using Neo.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NeoTransaction = Neo.Network.P2P.Payloads.Transaction;

namespace Neo.Plugins
{
    partial class RosettaController
    {
        /// <summary>
        /// Get all Transaction Identifiers in the mempool
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JObject Mempool(NetworkRequest request)
        {
            if (request.NetworkIdentifier.Blockchain.ToLower() != "neo n3")
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();

            NeoTransaction[] neoTxes = system.MemPool.ToArray();
            TransactionIdentifier[] transactionIdentifiers = neoTxes.Select(p => new TransactionIdentifier(p.Hash.ToString())).ToArray();
            MempoolResponse response = new MempoolResponse(transactionIdentifiers);
            return response.ToJson();
        }

        /// <summary>
        /// Get a transaction in the mempool by its Transaction Identifier. 
        /// This is a separate request than fetching a block transaction (/block/transaction) because some blockchain nodes need to know that a transaction query 
        /// is for something in the mempool instead of a transaction in a block. 
        /// Transactions may not be fully parsable until they are in a block (ex: may not be possible to determine the fee to pay before a transaction is executed). 
        /// On this endpoint, it is ok that returned transactions are only estimates of what may actually be included in a block.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JObject MempoolTransaction(MempoolTransactionRequest request)
        {
            if (request.NetworkIdentifier.Blockchain.ToLower() != "neo n3")
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();
            // check tx
            if (request.TransactionIdentifier == null)
                return Error.TX_IDENTIFIER_INVALID.ToJson();
            if (!UInt256.TryParse(request.TransactionIdentifier.Hash, out UInt256 txHash))
                return Error.TX_HASH_INVALID.ToJson();
            NeoTransaction neoTx = system.MemPool.ToArray().FirstOrDefault(p => p.Hash == txHash);
            if (neoTx == default(NeoTransaction))
                return Error.TX_NOT_FOUND.ToJson();

            Transaction tx = ConvertTx(neoTx);
            MempoolTransactionResponse response = new MempoolTransactionResponse(tx);
            return response.ToJson();
        }

        // cannot get operations from transaction in mempool, since some transfer may occur when a contract dynamic call takes place
        private Transaction ConvertTx(NeoTransaction neoTx)
        {
            Dictionary<string, JObject> metadata = new Dictionary<string, JObject>();

            metadata.Add("size", neoTx.Size);
            metadata.Add("version", neoTx.Version);
            metadata.Add("nonce", neoTx.Nonce);
            metadata.Add("sysfee", neoTx.SystemFee);
            metadata.Add("netfee", neoTx.NetworkFee);
            metadata.Add("sender", neoTx.Sender.ToAddress(system.Settings.AddressVersion));
            metadata.Add("script", Convert.ToBase64String(neoTx.Script));
            metadata.Add("signers", neoTx.Signers.Select(p => p.ToJson()).ToArray());
            metadata.Add("attributes", neoTx.Attributes.Select(p => p.ToJson()).ToArray());
            metadata.Add("witnesses", neoTx.Witnesses.Select(p => p.ToJson()).ToArray());
            return new Transaction(new TransactionIdentifier(neoTx.Hash.ToString()), ConvertToTransferOperation(neoTx.Script), new Metadata(metadata));
        }


        private static readonly byte[] ContractCall = new byte[] { 0x62, 0x7d, 0x5b, 0x52 };



        private static Operation[] ConvertToTransferOperation(byte[] scripts)
        {
            var instructions = Parse(scripts);
            if (instructions.Count != 10
               || instructions[^8].OpCode != OpCode.PUSHDATA1
               || instructions[^7].OpCode != OpCode.PUSHDATA1
               || instructions[^6].OpCode != OpCode.PUSH4
               || instructions[^5].OpCode != OpCode.PACK
               || instructions[^4].OpCode != OpCode.PUSH15
               || instructions[^3].OpCode != OpCode.PUSHDATA1 || instructions[^3].TokenString != "transfer"
               || instructions[^2].OpCode != OpCode.PUSHDATA1
               || instructions[^1].OpCode != OpCode.SYSCALL)
            {
                return new Operation[0];
            }
            var from = new UInt160(instructions[3].Operand.ToArray());
            var to = new UInt160(instructions[2].Operand.ToArray());
            var amount = ConvertInteger(instructions[1]);
            var tokenhash = new UInt160(instructions[8].Operand.ToArray());
            Operation fromOperation = new Operation(new OperationIdentifier(0),
                                      OperationType.Transfer,
                                      null, // vm state is HALT, FAULT transfer is ignored
                                      null,
                                      new AccountIdentifier(from.ToAddress(53)),
                                      (-amount).ToNEOorGASAmount(tokenhash),
                                      null
                                      );
            Operation toOperation = new Operation(new OperationIdentifier(1),
                OperationType.Transfer,
                null,
                new OperationIdentifier[] { new OperationIdentifier(0) }, // related Operation is the fromOperation
                new AccountIdentifier(to.ToAddress(53)),
                (amount).ToNEOorGASAmount(tokenhash),
                null
                );
            return new Operation[] { fromOperation, toOperation };
        }

        private static BigInteger ConvertInteger(Instruction instruction)
        {
            switch (instruction.OpCode)
            {
                case OpCode.PUSHINT8:
                case OpCode.PUSHINT16:
                case OpCode.PUSHINT32:
                case OpCode.PUSHINT64:
                case OpCode.PUSHINT128:
                case OpCode.PUSHINT256:
                    {
                        return (new BigInteger(instruction.Operand.Span));
                    }
                case OpCode.PUSHM1:
                case OpCode.PUSH0:
                case OpCode.PUSH1:
                case OpCode.PUSH2:
                case OpCode.PUSH3:
                case OpCode.PUSH4:
                case OpCode.PUSH5:
                case OpCode.PUSH6:
                case OpCode.PUSH7:
                case OpCode.PUSH8:
                case OpCode.PUSH9:
                case OpCode.PUSH10:
                case OpCode.PUSH11:
                case OpCode.PUSH12:
                case OpCode.PUSH13:
                case OpCode.PUSH14:
                case OpCode.PUSH15:
                case OpCode.PUSH16:
                    {
                        return (int)instruction.OpCode - (int)OpCode.PUSH0;
                    }
                default:
                    throw new Exception($"Unknown OpCode{instruction.OpCode}");
            }

        }

        public static List<Instruction> Parse(byte[] scripts)
        {
            var result = new List<Instruction>();
            try
            {
                var s = new Script(scripts);
                for (int ip = 0; ip < scripts.Length;)
                {
                    var instruction = s.GetInstruction(ip);
                    result.Add(instruction);
                    ip += instruction.Size;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{scripts.ToHexString()}:{e}");
            }
            return result;
        }
    }
}
