﻿using Neo.IO.Json;
using System.Linq;

namespace Neo.Plugins
{
    public class ConstructionPayloadsRequest
    {
        public NetworkIdentifier NetworkIdentifier { get; set; }
        public Operation[] Operations { get; set; }
        public Metadata Metadata { get; set; }

        public PublicKey[] PublicKeys { get; set; }

        public ConstructionPayloadsRequest(NetworkIdentifier networkIdentifier, Operation[] operations, Metadata metadata = null, PublicKey[] publicKeys = null)
        {
            NetworkIdentifier = networkIdentifier;
            Operations = operations;
            Metadata = metadata;
            PublicKeys = publicKeys;
        }

        public static ConstructionPayloadsRequest FromJson(JObject json)
        {
            return new ConstructionPayloadsRequest(NetworkIdentifier.FromJson(json["network_identifier"]),
                (json["operations"] as JArray).Select(p => Operation.FromJson(p)).ToArray(),
                json.ContainsProperty("metadata") ? Metadata.FromJson(json["metadata"]) : null,
                (json["public_keys"] as JArray).Select(p => PublicKey.FromJson(p)).ToArray());
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["network_identifier"] = NetworkIdentifier.ToJson();
            json["operations"] = Operations.Select(p => p.ToJson()).ToArray();
            if (Metadata != null && Metadata.ToJson() != null)
                json["metadata"] = Metadata.ToJson();
            return json;
        }
    }
}
