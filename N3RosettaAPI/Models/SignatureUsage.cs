using Neo.IO.Json;
using System;
using System.Linq;

namespace Neo.Plugins
{
    /// <summary>
    /// SignatureUsage indicates how the signatures in ConstructionCombineRequest are used
    /// </summary>
    public class SignatureUsage
    {
        // indicates which script hash this public key belongs to
        public UInt160 SignerAccount { get; set; }
        // indicates which keys in the array should be used for the signer
        public int[] SignatureIndexs { get; set; }
        // for multi-sig m/n, m <= KeyIndexes.Length
        public int M { get; set; }
        

        public SignatureUsage(UInt160 signerAccount, int[] keyIndexes, int m)
        {
            if (m > keyIndexes.Length)
                throw new ArgumentException();

            SignerAccount = signerAccount;
            SignatureIndexs = keyIndexes;
            M = m;
        }

        public static SignatureUsage FromJson(JObject json)
        {
            return new SignatureUsage(UInt160.Parse(json["signer_account"].AsString()),
                (json["signature_indexes"] as JArray).Select(p => int.Parse(p.AsString())).ToArray(),
                int.Parse(json["m"].AsString()));
        }
    }
}
