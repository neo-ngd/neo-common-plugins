using GraphQL.Types;
using Neo.IO.Json;
using Neo.SmartContract;
using System.Linq;

namespace GraphQLPlugin.ModelType
{
    public class InvokeResultType: ObjectGraphType<GraphInvokeResult>
    {
        public InvokeResultType()
        {
            Field(x => x.Script);
            Field(x => x.State);
            Field(x => x.GasConsumed);
            Field("Stack", x => x.Stack, type: typeof(ListGraphType<ContractParameterGraphType>));
        }
    }

    public class ContractParameterGraphType: ObjectGraphType<ContractParameter>
    {
        public ContractParameterGraphType()
        {
            Field(x => x.Type, type: typeof(ContractParameterEnumType));
            Field("Value", x => x.Value.ToString());
        }
    }

     public class RpcStackInputType: InputObjectGraphType
    {
        public RpcStackInputType()
        {
            Field<StringGraphType>("Type");
            Field<StringGraphType>("Value");
        }
    }

    public class ContractParameterEnumType: EnumerationGraphType<ContractParameterType> { }

    public class GraphInvokeResult
    {
        public string Script { get; set; }

        public string State { get; set; }

        public string GasConsumed { get; set; }

        public ContractParameter[] Stack { get; set; }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["script"] = Script;
            json["state"] = State;
            json["gas_consumed"] = GasConsumed;
            json["stack"] = new JArray(Stack.Select(p => p.ToJson()));
            return json;
        }

        public static GraphInvokeResult FromJson(JObject json)
        {
            GraphInvokeResult invokeScriptResult = new GraphInvokeResult();
            invokeScriptResult.Script = json["script"].AsString();
            invokeScriptResult.State = json["state"].AsString();
            invokeScriptResult.GasConsumed = json["gas_consumed"].AsString();
            invokeScriptResult.Stack = ((JArray)json["stack"]).Select(p => ContractParameter.FromJson(p)).ToArray();
            return invokeScriptResult;
        }
    }

    public class RpcStack
    {
        public string Type { get; set; }

        public string Value { get; set; }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["type"] = Type;
            json["value"] = Value;
            return json;
        }

        public static RpcStack FromJson(JObject json)
        {
            RpcStack stackJson = new RpcStack();
            stackJson.Type = json["type"].AsString();
            stackJson.Value = json["value"].AsString();
            return stackJson;
        }
    }
}
