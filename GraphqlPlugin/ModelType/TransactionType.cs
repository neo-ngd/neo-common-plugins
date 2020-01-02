using GraphQL.Types;
using Neo.Network.P2P.Payloads;
using Neo.Wallets;
using System.Linq;
using Neo.IO;
using Neo.IO.Json;
using Neo;  
using Neo.VM;
using System;

namespace GraphQLPlugin.ModelType
{
    public class TransactionsType : ObjectGraphType<GraphTransaction>
    {
        public TransactionsType()
        {
            Field("Transaction", x => x.Transaction, type: typeof(TransactionType));
            Field("BlockHash", x => x.BlockHash.ToString());
            Field(x => x.Confirmations, type: typeof(IntGraphType));
            Field(x => x.VMState, type: typeof(VMStateEnum));
            Field(x => x.BlockTime, type: typeof(UIntGraphType)); 
        }
    }

    public class TransactionType : ObjectGraphType<Transaction>
    {
        public TransactionType()
        {
            Field("Hash", x => x.Hash.ToString());
            Field(x => x.Size);
            Field(x => x.Version, type:typeof(ByteGraphType));
            Field(x => x.Nonce, type: typeof(UIntGraphType));
            Field("Sender", x => x.Sender.ToAddress());  
            Field("SystemFee", x => x.SystemFee.ToString());
            Field("NetworkFee", x => x.NetworkFee.ToString());
            Field(x => x.ValidUntilBlock, type: typeof(UIntGraphType));
            Field(x => x.Attributes, type: typeof(ListGraphType<TransactionAttributeType>));
            Field(x => x.Cosigners, type: typeof(ListGraphType<CosignersType>));
            Field("Script", x => Convert.ToBase64String(x.Script));
            Field(x => x.Witnesses, type: typeof(ListGraphType<WitnessesType>));
        }
    }

    public class TransactionAttributeType : ObjectGraphType<TransactionAttribute>
    {
        public TransactionAttributeType()
        {
            Field(x => x.Usage, type: typeof(TransactionAttributeUsageType));
            Field("Data", x => Convert.ToBase64String(x.Data));
        }
    }

    public class TransactionAttributeUsageType : EnumerationGraphType<TransactionAttributeUsage> { }

    public class CosignersType : ObjectGraphType<Cosigner>
    {
        public CosignersType()
        {
            Field("Account", x => x.Account.ToString());
            Field(x => x.Scopes, type: typeof(WitnessScopeEnum));
            Field("AllowedContracts", x => x.AllowedContracts.Select(p => (JObject)p.ToString()).ToArray(), type: typeof(ListGraphType<StringGraphType>));
            Field("AllowedGroups", x => x.AllowedGroups.Select(p => (JObject)p.ToString()).ToArray(), type: typeof(ListGraphType<StringGraphType>));
        }
    }

    public class WitnessScopeEnum : EnumerationGraphType<WitnessScope> { }

    public class VMStateEnum : EnumerationGraphType<VMState> { }

    public class GraphTransaction
    {
        public Transaction Transaction { get; set; }

        public UInt256 BlockHash { get; set; }

        public int? Confirmations { get; set; }

        public uint? BlockTime { get; set; }

        public VMState? VMState { get; set; }

        public JObject ToJson()
        {
            JObject json = Transaction.ToJson();
            if (Confirmations != null)
            {
                json["blockhash"] = BlockHash.ToString();
                json["confirmations"] = Confirmations;
                json["blocktime"] = BlockTime;
                if (VMState != null)
                {
                    json["vmState"] = VMState;
                }
            }
            return json;
        }

        public static GraphTransaction FromJson(JObject json)
        {
            GraphTransaction transaction = new GraphTransaction();
            transaction.Transaction = Transaction.FromJson(json);
            if (json["confirmations"] != null)
            {
                transaction.BlockHash = UInt256.Parse(json["blockhash"].AsString());
                transaction.Confirmations = (int)json["confirmations"].AsNumber();
                transaction.BlockTime = (uint)json["blocktime"].AsNumber();
                transaction.VMState = json["vmState"]?.TryGetEnum<VMState>();
            }
            return transaction;
        }
    }
}
