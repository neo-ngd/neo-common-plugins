

using Neo.IO.Json;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;

namespace Neo.Plugins
{
    public interface IGraphService
    {
        string GetBestBlockHash();

        JObject GetBlock(JObject key, bool verbose);

        uint GetBlockCount();

        string GetBlockHash(uint height);

        string GetBlockSysFee(uint height);

        JObject GetContractState(UInt160 script_hash);

        JObject GetRawMemPool(bool shouldGetUnverified);

        JObject GetRawTransaction(UInt256 hash, bool verbose);

        JObject GetStorage(UInt160 script_hash, byte[] key);

        uint GetTransactionHeight(UInt256 hash);

        JObject GetValidators();

        int GetConnectionCount();

        JObject GetPeers();

        JObject SendRawTransaction(Transaction tx);

        JObject SubmitBlock(Block block);

        JObject GetVersion();

        JObject InvokeFunction(UInt160 script_hash, string operation, ContractParameter[] args);

        JObject InvokeScript(byte[] script, UInt160[] scriptHashesForVerifying);

        JObject ListPlugins();

        JObject ValidateAddress(string address);

        bool CloseWallet();

        string DumpPrivKey(string address);

        string GetBalance(string assetID);

        string GetNewAddress();

        string GetUnclaimedGas();

        JObject ImportPrivKey(string privkey);

        JObject ListAddress();

        JObject OpenWallet(string path, string password);
    }
}
