using Neo.IO.Json;
using System;
using System.Linq;

namespace Neo.Plugins
{
    /// <summary>
    /// PublicKeyUsage indicates how the public keys in ConstructionMetadataRequest are used
    /// </summary>
    public class PublicKeyUsage
    {
        // indicates which script hash this public key belongs to
        public UInt160 SignerAccount { get; set; }
        // indicates which keys in the array should be used for the signer
        public int[] KeyIndexs { get; set; }
        // for multi-sig m/n, m <= KeyIndexes.Length
        public int M { get; set; }
        

        public PublicKeyUsage(UInt160 signerAccount, int[] keyIndexes, int m)
        {
            if (m > keyIndexes.Length)
                throw new ArgumentException();

            SignerAccount = signerAccount;
            KeyIndexs = keyIndexes;
            M = m;
        }

        public static PublicKeyUsage FromJson(JObject json)
        {
            return new PublicKeyUsage(UInt160.Parse(json["signer_account"].AsString()),
                (json["key_indexes"] as JArray).Select(p => int.Parse(p.AsString())).ToArray(),
                int.Parse(json["m"].AsString()));
        }
    }
}
