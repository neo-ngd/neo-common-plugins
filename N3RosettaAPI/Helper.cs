using Neo.Cryptography;
using Neo.Cryptography.ECC;
using Neo.IO.Json;
using Neo.Network.P2P.Payloads;
using System;
using System.Linq;

namespace Neo.Plugins
{
    internal static class Helper
    {
        public static string TrimStart(this string target, string trimString)
        {
            if (string.IsNullOrEmpty(trimString)) return target;

            string result = target;
            while (result.StartsWith(trimString))
            {
                result = result.Substring(trimString.Length);
            }

            return result;
        }

        public static string IntToHash160String(this int value)
        {
            return new UInt160(Crypto.Hash160(BitConverter.GetBytes(value))).ToString();
        }

        public static string IntToHash256String(this int value)
        {
            return new UInt160(Crypto.Hash256(BitConverter.GetBytes(value))).ToString();
        }

        public static string AsString(this SignatureType type)
        {
            switch (type)
            {
                case SignatureType.Ecdsa:
                    return "ecdsa";
                case SignatureType.EcdsaRecovery:
                    return "ecdsa_recovery";
                case SignatureType.Ed25519:
                    return "ed25519";
                default:
                    return default(SignatureType).AsString();
            }
        }

        public static SignatureType ToSignatureType(this JObject json)
        {
            switch (json.AsString())
            {
                case "ecdsa":
                    return SignatureType.Ecdsa;
                case "ecdsa_recovery":
                    return SignatureType.EcdsaRecovery;
                case "ed25519":
                    return SignatureType.Ed25519;
                default:
                    throw new ArgumentException();
            }
        }

        public static string AsString(this CurveType type)
        {
            switch (type)
            {
                case CurveType.Secp256k1:
                    return "secp256k1";
                case CurveType.Secp256r1:
                    return "secp256r1";
                case CurveType.Edwards25519:
                    return "edwards25519";
                default:
                    return default(CurveType).AsString();
            }
        }

        public static CurveType ToCurveType(this JObject json)
        {
            switch (json.AsString())
            {
                case "secp256k1":
                    return CurveType.Secp256k1;
                case "secp256r1":
                    return CurveType.Secp256r1;
                case "edwards25519":
                    return CurveType.Edwards25519;
                default:
                    throw new ArgumentException();
            }
        }

        public static string AsString(this CoinAction action)
        {
            switch (action)
            {
                case CoinAction.CoinCreated:
                    return "coin_created";
                case CoinAction.CoinSpent:
                    return "coin_spent";
                default:
                    return default(CoinAction).AsString();
            }
        }

        public static CoinAction ToCoinAction(this JObject json)
        {
            switch (json.AsString())
            {
                case "coin_created":
                    return CoinAction.CoinCreated;
                case "coin_spent":
                    return CoinAction.CoinSpent;
                default:
                    throw new ArgumentException();
            }
        }

        public static string AsString(this ExemptionType type)
        {
            switch (type)
            {
                case ExemptionType.GreateOrEqual:
                    return "great_or_equal";
                case ExemptionType.LessOrEqual:
                    return "less_or_equal";
                case ExemptionType.Dynamic:
                    return "dynamic";
                default:
                    return default(ExemptionType).AsString();
            }
        }

        public static ExemptionType ToExemptionType(this JObject json)
        {
            switch (json.AsString())
            {
                case "great_or_equal":
                    return ExemptionType.GreateOrEqual;
                case "less_or_equal":
                    return ExemptionType.LessOrEqual;
                case "dynamic":
                    return ExemptionType.Dynamic;
                default:
                    return ExemptionType.Unknown;
            }
        }

        //public static string ToDescription(this Enum val)
        //{
        //    var type = val.GetType();

        //    var memberInfo = type.GetMember(val.ToString());

        //    var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

        //    if (attributes == null || attributes.Length != 1)
        //    {
        //        // if not defined, return ToString()
        //        return val.ToString();
        //    }

        //    return (attributes.Single() as DescriptionAttribute).Description;
        //}

        //public static Enum FromDescription(this JObject json)
        //{

        //}

        public static Signer ToSigner(this JObject json)
        {
            Signer signer = new();
            signer.Account = UInt160.Parse(json["account"].AsString());
            WitnessScope scopes = json["scopes"].TryGetEnum<WitnessScope>();
            signer.Scopes = scopes;
            if (scopes == WitnessScope.CustomContracts)
            {
                signer.AllowedContracts = (json["allowedcontracts"] as JArray).ToList().Select(p => UInt160.Parse(p.AsString())).ToArray();
            }
            if (scopes == WitnessScope.CustomGroups)
            {
                signer.AllowedGroups = (json["allowedgroups"] as JArray).ToList().Select(p => ECPoint.Parse(p.AsString(), ECCurve.Secp256r1)).ToArray();
            }
            return signer;
        }
    }
}
