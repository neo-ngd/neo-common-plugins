using Akka.Actor;
using Microsoft.AspNetCore.Mvc;
using Neo.IO.Json;
using Neo.Network.P2P;
using Neo.SmartContract.Native;
using System;
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
            NetworkIdentifier networkIdentifier = new NetworkIdentifier("neo n3", network);
            NetworkListResponse networkListResponse = new NetworkListResponse(new NetworkIdentifier[] { networkIdentifier });
            return networkListResponse.ToJson();
        }

        [HttpPost("/network/options")]
        public JObject NetworkOptions(NetworkRequest request)
        {
            if (request.NetworkIdentifier?.Blockchain?.ToLower() != "neo n3")
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();
            if (request.NetworkIdentifier?.Network?.ToLower() != network)
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();

            Version version = new Version(Settings.Default.RosettaVersion, LocalNode.UserAgent);
            Allow allow = new Allow(OperationStatus.AllowedStatuses, OperationType.AllowedOperationTypes, Error.AllowedErrors, false,
                Array.Empty<string>(), Array.Empty<BalanceExemption>(), false, 1468595301000);
            NetworkOptionsResponse networkOptionsResponse = new NetworkOptionsResponse(version, allow);
            return networkOptionsResponse.ToJson();
        }

        [HttpPost("/network/status")]
        public JObject NetworkStatus(NetworkRequest request)
        {
            if (request.NetworkIdentifier?.Blockchain?.ToLower() != "neo n3")
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();
            if (request.NetworkIdentifier?.Network?.ToLower() != network)
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();

            var snapshot = system.StoreView;
            uint currentHeight = NativeContract.Ledger.CurrentIndex(snapshot);
            NeoBlock currentBlock = NativeContract.Ledger.GetBlock(snapshot, currentHeight);
            if (currentBlock == null)
                return Error.BLOCK_NOT_FOUND.ToJson();

            string currentBlockHash = currentBlock.Hash.ToString();
            long currentBlockTimestamp = (long)currentBlock.Timestamp;

            BlockIdentifier currentBlockIdentifier = new BlockIdentifier(currentHeight, currentBlockHash);
            BlockIdentifier genesisBlockIdentifier = new BlockIdentifier(system.GenesisBlock.Index, system.GenesisBlock.Hash.ToString());

            var localNode = system.LocalNode.Ask<LocalNode>(new LocalNode.GetInstance()).Result;

            var connected = localNode.GetRemoteNodes().Select(p => new Peer(p.GetHashCode().IntToHash160String(),
                new Metadata(new Dictionary<string, JObject>
                {
                    { "connected", true.ToString().ToLower() },
                    { "address", p.Listener.ToString() },
                    { "height", p.LastBlockIndex }
                })
            ));

            var unconnected = localNode.GetUnconnectedPeers().Select(p => new Peer(p.GetHashCode().IntToHash160String(),
                new Metadata(new Dictionary<string, JObject>
                {
                    { "unconnected", false.ToString().ToLower() },
                    { "address", p.ToString() }
                })
            ));

            Peer[] peers = connected.Concat(unconnected).ToArray();
            NetworkStatusResponse response = new NetworkStatusResponse(currentBlockIdentifier, currentBlockTimestamp, genesisBlockIdentifier, peers);
            return response.ToJson();
        }
    }
}
