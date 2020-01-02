using System;

namespace Neo.Plugins
{
    class GraphException: Exception
    {
        public GraphException(int code, string message) : base(message)
        {
            HResult = code;
        }
    }
}
