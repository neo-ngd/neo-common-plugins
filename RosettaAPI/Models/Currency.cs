using Neo.IO.Json;
using Neo.Ledger;
using Neo.SmartContract;
using Neo.VM;
using System;
using System.Collections.Generic;

namespace Neo.Plugins
{
    // The Currency class is composed of a canonical Symbol and Decimals. This Decimals value is used to
    // convert an Amount.Value from atomic units (Satoshis) to standard units (Bitcoins).
    public class Currency
    {
        // Canonical symbol associated with a currency.
        public string Symbol { get; set; }

        // Number of decimal places in the standard unit representation of the amount. For example, BTC
        // has 8 decimals. Note that it is not possible to represent the value of some currency in
        // atomic units that is not base 10.
        public int Decimals { get; set; }

        // Any additional information related to the currency itself. For example, it would be useful to
        // populate this object with the contract address of an NEP-5 token.
        public Metadata Metadata { get; set; }

        public Currency(UInt256 assetId)
        {
            Symbol = assetId == Blockchain.GoverningToken.Hash ? "NEO" : assetId == Blockchain.UtilityToken.Hash ? "GAS" : throw new NotSupportedException();
            Decimals = assetId == Blockchain.GoverningToken.Hash ? Blockchain.GoverningToken.Precision : assetId == Blockchain.UtilityToken.Hash ? Blockchain.UtilityToken.Precision : throw new NotSupportedException();
            Metadata = new Metadata(new Dictionary<string, JObject>
            {
                {"token_type",  assetId == Blockchain.GoverningToken.Hash ? "Governing Token" : assetId == Blockchain.UtilityToken.Hash ? "Utility Token" : throw new NotSupportedException()}
            });
        }

        public Currency(UInt160 assetId)
        {
            byte[] script;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitAppCall(assetId, "symbol");
                sb.EmitAppCall(assetId, "decimals");
                script = sb.ToArray();
            }

            using (ApplicationEngine engine = ApplicationEngine.Run(script, testMode: true))
            {
                if (engine.State.HasFlag(VMState.FAULT))
                    throw new Exception("engine faulted");
                byte decimals = (byte)engine.ResultStack.Pop().GetBigInteger();
                string symbol = engine.ResultStack.Pop().GetString();
                Symbol = symbol;
                Decimals = decimals;
                Metadata = new Metadata(new Dictionary<string, JObject>
                {
                    {"token_type", "NEP5 Token"}
                });
            }
        }

        public Currency(string symbol, int decimals, Metadata metadata = null)
        {
            Symbol = symbol;
            Decimals = decimals;
            Metadata = metadata;
        }

        public static Currency FromJson(JObject json)
        {
            return new Currency(json["symbol"].AsString(),
                (int)json["decimals"].AsNumber(),
                json.ContainsProperty("metadata") ? Metadata.FromJson(json["metadata"]) : null);
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["symbol"] = Symbol;
            json["decimals"] = Decimals.ToString();
            if (Metadata != null && Metadata.ToJson() != null)
                json["metadata"] = Metadata.ToJson();
            return json;
        }
    }
}