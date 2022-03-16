using Neo.IO.Json;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.Wallets;
using System;
using System.Collections.Generic;
using System.Numerics;
using NeoBlock = Neo.Network.P2P.Payloads.Block;

namespace Neo.Plugins
{
    partial class RosettaController
    {
        /// <summary>
        /// Get an array of all AccountBalances for an AccountIdentifier and the BlockIdentifier at which the balance lookup was performed. 
        /// The BlockIdentifier must always be returned because some consumers of account balance data need to know specifically at which block 
        /// the balance was calculated to compare balances they compute from operations with the balance returned by the node. 
        /// It is important to note that making a balance request for an account without populating the SubAccountIdentifier should not 
        /// result in the balance of all possible SubAccountIdentifiers being returned. 
        /// Rather, it should result in the balance pertaining to no SubAccountIdentifiers being returned (sometimes called the liquid balance). 
        /// To get all balances associated with an account, it may be necessary to perform multiple balance requests with unique AccountIdentifiers.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JObject AccountBalance(AccountBalanceRequest request)
        {
            DataCache snapshot = system.StoreView;
            BlockIdentifier respBlockIdentifier;
            if (request.NetworkIdentifier.Blockchain.ToLower() != "neo n3")
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();
            if (request.AccountIdentifier is null)
                return Error.ACCOUNT_IDENTIFIER_INVALID.ToJson();
            if (request.BlockIdentifier is null)
            {
                respBlockIdentifier = new(NativeContract.Ledger.CurrentIndex(system.StoreView), NativeContract.Ledger.CurrentHash(system.StoreView).ToString());
            }
            else if (request.BlockIdentifier.Index is null && request.BlockIdentifier.Hash is null)
            {
                return Error.BLOCK_IDENTIFIER_INVALID.ToJson();
            }
            else if (request.BlockIdentifier.Index is not null)
            {
                var index = (uint)request.BlockIdentifier.Index;
                var hash = NativeContract.Ledger.GetBlockHash(system.StoreView, index);
                if (request.BlockIdentifier.Hash is not null && request.BlockIdentifier.Hash != hash.ToString())
                    return Error.BLOCK_IDENTIFIER_INVALID.ToJson();
                if (Settings.Default.EnableHistoricalBalance)
                    snapshot = new HistoricalDataCache(db, GetStateRoot(index));
                respBlockIdentifier = new(index, hash.ToString());
            }
            else
            {
                if (!UInt256.TryParse(request.BlockIdentifier.Hash, out var hash))
                    return Error.BLOCK_HASH_INVALID.ToJson();
                var block = NativeContract.Ledger.GetBlock(system.StoreView, hash);
                request.BlockIdentifier.Index = block.Index;
                if (Settings.Default.EnableHistoricalBalance)
                    snapshot = new HistoricalDataCache(db, GetStateRoot(block.Index));
                respBlockIdentifier = new(block.Index, request.BlockIdentifier.Hash);
            }
            UInt160 account;
            try
            {
                account = request.AccountIdentifier.Address.ToScriptHash(system.Settings.AddressVersion);
            }
            catch (Exception)
            {
                return Error.ACCOUNT_ADDRESS_INVALID.ToJson();
            }

            Amount[] balances;
            if (request.Currencies != null)
            {
                var tmp = new List<Amount>();
                foreach (var currency in request.Currencies)
                {
                    if (currency.Metadata != null && currency.Metadata.ContainsKey("script_hash"))
                    {
                        if (!UInt160.TryParse(currency.Metadata["script_hash"].AsString(), out UInt160 scriptHash))
                            return Error.CONTRACT_ADDRESS_INVALID.ToJson();
                        Amount balance = GetBalance(snapshot, scriptHash, account);
                        if (balance is null)
                            return Error.VM_FAULT.ToJson();
                        tmp.Add(balance);
                    }
                }
                balances = tmp.ToArray();
            }
            else
                balances = GetNeoAndGas(snapshot, account);
            AccountBalanceResponse response = new(respBlockIdentifier, balances);
            return response.ToJson();
        }

        private Amount[] GetNeoAndGas(DataCache snapshot, UInt160 account)
        {
            var neo = NativeContract.NEO;
            var gas = NativeContract.GAS;
            var neoAmount = new Amount(neo.BalanceOf(snapshot, account).ToString(), new Currency(neo.Symbol, neo.Decimals, new Metadata(new Dictionary<string, JObject>() { { "script_hash", neo.Hash.ToString() } })));
            var gasAmount = new Amount(gas.BalanceOf(snapshot, account).ToString(), new Currency(gas.Symbol, gas.Decimals, new Metadata(new Dictionary<string, JObject>() { { "script_hash", gas.Hash.ToString() } })));
            return new Amount[] { neoAmount, gasAmount };
        }

        private Amount GetBalance(DataCache snapshot, UInt160 scriptHash, UInt160 account)
        {
            byte[] script;
            var metadata = new Metadata(new Dictionary<string, JObject>() { { "script_hash", scriptHash.ToString() } });
            using (ScriptBuilder sb = new())
            {
                sb.EmitDynamicCall(scriptHash, "balanceOf", account);
                sb.EmitDynamicCall(scriptHash, "symbol");
                sb.EmitDynamicCall(scriptHash, "decimals");
                script = sb.ToArray();
            }
            using ApplicationEngine engine = ApplicationEngine.Run(script, snapshot, settings: system.Settings);
            if (engine.State == VMState.HALT)
            {
                byte decimals = (byte)engine.ResultStack.Pop().GetInteger();
                string symbol = engine.ResultStack.Pop().GetString();
                BigInteger balance = engine.ResultStack.Pop().GetInteger();
                return new Amount(balance.ToString(), new Currency(symbol, decimals, metadata));
            }
            return null;
        }

        // account/coins
        // Get an array of all unspent coins for an AccountIdentifier and the BlockIdentifier at which the lookup was performed.
        // If your implementation does not support coins (i.e. it is for an account-based blockchain), you do not need to implement this endpoint.
        // If you implementation does support coins (i.e. it is fro a UTXO-based blockchain), you MUST also complete the /account/balance endpoint.
        // It is important to note that making a coins request for an account without populating the SubAccountIdentifier
        // should not result in the coins of all possible SubAccountIdentifiers being returned.
        // Rather, it should result in the coins pertaining to no SubAccountIdentifiers being returned.
        // To get all coins associated with an account, it may be necessary to perform multiple coin requests with unique AccountIdentifiers.
        // Optionally, an implementation may choose to support updating an AccountIdentifier's unspent coins based on the contents of the mempool.
        // Note, using this functionality breaks any guarantee of idempotency.
    }
}
