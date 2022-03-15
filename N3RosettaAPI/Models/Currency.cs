using Neo.IO.Json;
using Neo.Ledger;
using Neo.SmartContract;
using Neo.SmartContract.Native;
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

        public static Currency GAS => new(NativeContract.GAS.Symbol,
            NativeContract.GAS.Decimals,
            new(new()
            {
                { "script_hash", NativeContract.GAS.Hash.ToString() }
            }));
        public static Currency NEO => new(NativeContract.NEO.Symbol,
            NativeContract.NEO.Decimals,
            new(new()
            {
                { "script_hash", NativeContract.NEO.Hash.ToString() }
            }));

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
            json["decimals"] = Decimals;
            if (Metadata != null && Metadata.ToJson() != null)
                json["metadata"] = Metadata.ToJson();
            return json;
        }

        public bool Equals(Currency other)
        {
            if (other is null) return false;
            return Symbol == other.Symbol
                && Decimals == other.Decimals
                && Metadata.ContainsKey("script_hash")
                && other.Metadata.ContainsKey("script_hash")
                && Metadata["script_hash"].AsString() == other.Metadata["script_hash"].AsString();
        }
    }
}
