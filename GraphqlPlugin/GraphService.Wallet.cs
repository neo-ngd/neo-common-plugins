using Akka.Actor;
using Neo.IO;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.Network.P2P;
using Neo.Network.P2P.Payloads;
using Neo.Persistence;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.Wallets;
using Neo.Wallets.NEP6;
using Neo.Wallets.SQLite;
using System;
using System.IO;
using System.Linq;
using System.Numerics;
using static System.IO.Path;

namespace Neo.Plugins
{
    partial class GraphService: IGraphService
    {
        private static Wallet wallet;

        private void CheckWallet()
        {
            if (wallet is null)
                throw new GraphException(-400, "Access denied");
        }

        public bool CloseWallet()
        {
            wallet = null;
            return true;
        }

        public string DumpPrivKey(string address)
        {
            CheckWallet();
            UInt160 scriptHash = address.ToScriptHash();
            WalletAccount account = wallet.GetAccount(scriptHash);
            return account.GetKey().Export();
        }

        public string GetBalance(string assetID)
        {
            CheckWallet();
            UInt160 asset_id = UInt160.Parse(assetID);
            return wallet.GetAvailable(asset_id).Value.ToString();
        }

        public string GetNewAddress()
        {
            CheckWallet();
            WalletAccount account = wallet.CreateAccount();
            if (wallet is NEP6Wallet nep6)
                nep6.Save();
            return account.Address;
        }

        public string GetUnclaimedGas()
        {
            CheckWallet();
            BigInteger gas = BigInteger.Zero;
            using (SnapshotView snapshot = Blockchain.Singleton.GetSnapshot())
                foreach (UInt160 account in wallet.GetAccounts().Select(p => p.ScriptHash))
                {
                    gas += NativeContract.NEO.UnclaimedGas(snapshot, account, snapshot.Height + 1);
                }
            return gas.ToString();
        }

        public JObject ImportPrivKey(string privkey)
        {
            CheckWallet();
            WalletAccount account = wallet.Import(privkey);
            if (wallet is NEP6Wallet nep6wallet)
                nep6wallet.Save();
            return new JObject
            {
                ["address"] = account.Address,
                ["haskey"] = account.HasKey,
                ["label"] = account.Label,
                ["watchonly"] = account.WatchOnly
            };
        }

        public JObject ListAddress()
        {
            CheckWallet();
            return wallet.GetAccounts().Select(p =>
            {
                JObject account = new JObject();
                account["address"] = p.Address;
                account["haskey"] = p.HasKey;
                account["label"] = p.Label;
                account["watchonly"] = p.WatchOnly;
                return account;
            }).ToArray();
        }

        public JObject OpenWallet(string path, string password)
        {
            if (!File.Exists(path)) throw new FileNotFoundException();
            switch (GetExtension(path))
            {
                case ".db3":
                    {
                        wallet = UserWallet.Open(path, password);
                        break;
                    }
                case ".json":
                    {
                        NEP6Wallet nep6wallet = new NEP6Wallet(path);
                        nep6wallet.Unlock(password);
                        wallet = nep6wallet;
                        break;
                    }
                default:
                    throw new NotSupportedException();
            }
            return true;
        }

        private void ProcessInvokeWithWallet(JObject result)
        {
            if (wallet != null)
            {
                Transaction tx = wallet.MakeTransaction(result["script"].AsString().HexToBytes());
                ContractParametersContext context = new ContractParametersContext(tx);
                wallet.Sign(context);
                if (context.Completed)
                    tx.Witnesses = context.GetWitnesses();
                else
                    tx = null;
                result["tx"] = tx?.ToArray().ToHexString();
            }
        }

        public JObject SignAndRelay(Transaction tx)
        {
            ContractParametersContext context = new ContractParametersContext(tx);
            wallet.Sign(context);
            if (context.Completed)
            {
                tx.Witnesses = context.GetWitnesses();
                system.LocalNode.Tell(new LocalNode.Relay { Inventory = tx });
                return tx.ToJson();
            }
            else
            {
                return context.ToJson();
            }
        }
    }
}
