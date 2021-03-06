using Neo.IO.Json;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.VM;
using System;
using System.IO;
using System.Linq;

namespace Neo.Plugins
{
    partial class GraphService: IGraphService
    {
        private class CheckWitnessHashes : IVerifiable
        {
            private readonly UInt160[] _scriptHashesForVerifying;
            public Witness[] Witnesses { get; set; }
            public int Size { get; }

            public CheckWitnessHashes(UInt160[] scriptHashesForVerifying)
            {
                _scriptHashesForVerifying = scriptHashesForVerifying;
            }

            public void Serialize(BinaryWriter writer)
            {
                throw new NotImplementedException();
            }

            public void Deserialize(BinaryReader reader)
            {
                throw new NotImplementedException();
            }

            public void DeserializeUnsigned(BinaryReader reader)
            {
                throw new NotImplementedException();
            }

            public UInt160[] GetScriptHashesForVerifying(StoreView snapshot)
            {
                return _scriptHashesForVerifying;
            }

            public void SerializeUnsigned(BinaryWriter writer)
            {
                throw new NotImplementedException();
            }
        }

        private JObject GetInvokeResult(byte[] script, IVerifiable checkWitnessHashes = null)
        {
            using ApplicationEngine engine = ApplicationEngine.Run(script, checkWitnessHashes, extraGAS: GraphSettings.Default.MaxGasInvoke);
            JObject json = new JObject();
            json["script"] = script.ToHexString();
            json["state"] = engine.State;
            json["gas_consumed"] = engine.GasConsumed.ToString();
            try
            {
                json["stack"] = new JArray(engine.ResultStack.Select(p => p.ToParameter().ToJson()));
            }
            catch (InvalidOperationException)
            {
                json["stack"] = "error: recursive reference";
            }
            ProcessInvokeWithWallet(json);
            return json;
        }

        public JObject InvokeFunction(UInt160 script_hash, string operation, ContractParameter[] args)
        {
            byte[] script;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                script = sb.EmitAppCall(script_hash, operation, args).ToArray();
            }
            return GetInvokeResult(script);
        }

        public JObject InvokeScript(byte[] script, UInt160[] scriptHashesForVerifying)
        {
            CheckWitnessHashes checkWitnessHashes = null;
            checkWitnessHashes = new CheckWitnessHashes(scriptHashesForVerifying);
            return GetInvokeResult(script, checkWitnessHashes);
        }
    }
}
