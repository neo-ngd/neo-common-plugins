using Microsoft.AspNetCore.Mvc;
using Neo.Cryptography.MPTTrie;
using Neo.Persistence;
using Neo.SmartContract;

namespace Neo.Plugins
{
    [Produces("application/json")]
    internal partial class RosettaController
    {
        private readonly NeoSystem system;
        private readonly IStore db;

        public RosettaController(NeoSystem system, IStore db)
        {
            this.system = system;
            this.db = db;
        }
    }
}
