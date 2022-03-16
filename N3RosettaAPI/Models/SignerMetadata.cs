using Neo;
using Neo.IO.Json;
using Neo.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Plugins
{
    public class SignerMetadata
    {
        public SignerMetadata(string signerAccount, UInt160 signerAccountHash, string[] relatedAccounts, int m)
        {
            if (m > relatedAccounts.Length)
                throw new ArgumentException();

            SignerAccount = signerAccount;
            SignerAccountHash = signerAccountHash;
            RelatedAccounts = relatedAccounts;
            M = m;
        }

        // indicates which script hash this public key belongs to
        public string SignerAccount { get; }
        public UInt160 SignerAccountHash { get; }

        // indicates which keys in the array should be used for the signer
        public string[] RelatedAccounts { get; }
        // for multi-sig m/n, m <= KeyIndexes.Length
        public int M { get; }


        public static SignerMetadata FromJson(JObject json, byte addressVersion)
        {
            int.TryParse(json["m"]?.AsString(), out var m);
            return new SignerMetadata(
                json["signer_account"].AsString(),
                json["signer_account"].AsString().ToUInt160(addressVersion),
                (json["related_accounts"] as JArray).Select(p => p.AsString()).ToArray(),
                m);
        }
    }
}
