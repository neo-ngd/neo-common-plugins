using Microsoft.AspNetCore.Mvc;

namespace Neo.Plugins
{
    [Produces("application/json")]
    internal partial class RosettaController
    {
        private readonly NeoSystem system;

        public RosettaController(NeoSystem system)
        {
            this.system = system;
        }
    }
}
