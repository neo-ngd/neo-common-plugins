using Microsoft.AspNetCore.Mvc;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.Network.P2P;
using System.Collections.Generic;
using System.Linq;
using NeoBlock = Neo.Network.P2P.Payloads.Block;

namespace Neo.Plugins
{
    partial class RosettaController
    {
        [HttpPost("/network/list")]
        public JObject NetworkList(MetadataRequest request)
        {
            var magic = ProtocolSettings.Default.Magic;
            var network = magic == 7630401 ? "mainnet" : magic == 1953787457 ? "testnet" : "privatenet";
            NetworkIdentifier networkIdentifier = new NetworkIdentifier("neo", network);
            NetworkListResponse networkListResponse = new NetworkListResponse(new NetworkIdentifier[] { networkIdentifier });
            return networkListResponse.ToJson();
        }

        [HttpPost("/network/options")]
        public JObject NetworkOptions(NetworkRequest request)
        {
            Version version = new Version(RosettaApiSettings.Default.RosettaVersion, LocalNode.UserAgent);
            Allow allow = new Allow(OperationStatus.AllowedStatuses, OperationType.AllowedOperationTypes, Error.AllowedErrors, false);
            NetworkOptionsResponse networkOptionsResponse = new NetworkOptionsResponse(version, allow);
            return networkOptionsResponse.ToJson();
        }

        [HttpPost("/network/status")]
        public JObject NetworkStatus(NetworkRequest request)
        {
            long currentHeight = Blockchain.Singleton.Height;
            NeoBlock currentBlock = Blockchain.Singleton.GetBlock(Blockchain.Singleton.CurrentBlockHash);
            if (currentBlock == null)
                return Error.BLOCK_NOT_FOUND.ToJson();

            string currentBlockHash = currentBlock.Hash.ToString();
            long currentBlockTimestamp = currentBlock.Timestamp * 1000;

            BlockIdentifier currentBlockIdentifier = new BlockIdentifier(currentHeight, currentBlockHash);
            BlockIdentifier genesisBlockIdentifier = new BlockIdentifier(Blockchain.GenesisBlock.Index, Blockchain.GenesisBlock.Hash.ToString());

            var connected = LocalNode.Singleton.GetRemoteNodes().Select(p => new Peer(p.GetHashCode().IntToHash160String(),
                new Metadata(new Dictionary<string, JObject>
                {
                    { "connected", true.ToString().ToLower() },
                    { "address", p.Listener.ToString() },
                    { "height", p.LastBlockIndex.ToString() }
                })
            ));

            var unconnected = LocalNode.Singleton.GetUnconnectedPeers().Select(p => new Peer(p.GetHashCode().IntToHash160String(),
                new Metadata(new Dictionary<string, JObject>
                {
                    { "connected", false.ToString().ToLower() },
                    { "address", p.ToString() }
                })
            ));

            Peer[] peers = connected.Concat(unconnected).ToArray();
            NetworkStatusResponse response = new NetworkStatusResponse(currentBlockIdentifier, currentBlockTimestamp, genesisBlockIdentifier, peers);
            return response.ToJson();
        }
    }
}
