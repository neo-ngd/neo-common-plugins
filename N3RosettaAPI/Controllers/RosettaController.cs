using Microsoft.AspNetCore.Mvc;
using Neo.IO.Data.LevelDB;

namespace Neo.Plugins
{
    [Produces("application/json")]
    internal partial class RosettaController
    {
        private readonly NeoSystem system;
        private readonly DB db;

        public RosettaController(NeoSystem system, DB db)
        {
            this.system = system;
            this.db = db;
        }
    }
}
