namespace Neo.Plugins
{
    /// <summary>
    /// Operation type now only has "Transfer". More TBD.
    /// </summary>
    public class OperationType
    {
        public const string Transfer = "Transfer";

        public static readonly string[] AllowedOperationTypes = new string[] { Transfer };
    }
}
