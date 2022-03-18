using Microsoft.AspNetCore.Mvc;
using Neo.Persistence;
using System.Collections.Generic;

namespace Neo.Plugins
{
    [Produces("application/json")]
    internal partial class RosettaController
    {
        private readonly Dictionary<uint, string> networks = new(){
            {860833102u, "mainnet"},
            {877933390u, "testnet"}
        };
        private readonly NeoSystem system;
        private readonly IStore db;
        private readonly string network;

        public RosettaController(NeoSystem system, IStore db)
        {
            this.system = system;
            this.db = db;
            if (!networks.TryGetValue(system.Settings.Network, out network))
                network = "privatenet";
        }
    }
}
