using Neo.Cryptography;
using Neo.Cryptography.ECC;
using Neo.IO.Json;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

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


        public static string ToUtf8String(this byte[] text)
        {
            return Encoding.UTF8.GetString(text);
        }


        public static Signer ToSigner(this JObject json, byte addressVersion)
        {
            Signer signer = new();
            var address = json["account"].AsString();
            signer.Account = address.ToUInt160(addressVersion);
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

        public static Amount ToNEOorGASAmount(this long amount, UInt160 tokenhash)
        {
            return tokenhash == NativeContract.NEO.Hash ? amount.ToNEOAmount() :
                 tokenhash == NativeContract.GAS.Hash ? amount.ToGasAmount() : null;
        }

        public static Amount ToNEOorGASAmount(this BigInteger amount, UInt160 tokenhash)
        {
            return tokenhash == NativeContract.NEO.Hash ? amount.ToNEOAmount() :
                 tokenhash == NativeContract.GAS.Hash ? amount.ToGasAmount() : null;
        }

        public static Amount ToNEOAmount(this long amount)
        {
            var gasAmount = new Amount(amount.ToString(), new Currency(NativeContract.NEO.Symbol, NativeContract.NEO.Decimals, new Metadata(new Dictionary<string, JObject>() { { "script_hash", NativeContract.NEO.Hash.ToString() } })));
            return gasAmount;
        }

        public static Amount ToNEOAmount(this BigInteger amount)
        {
            var gasAmount = new Amount(amount.ToString(), new Currency(NativeContract.NEO.Symbol, NativeContract.NEO.Decimals, new Metadata(new Dictionary<string, JObject>() { { "script_hash", NativeContract.NEO.Hash.ToString() } })));
            return gasAmount;
        }


        public static Amount ToGasAmount(this long amount)
        {
            var gasAmount = new Amount(amount.ToString(), new Currency(NativeContract.GAS.Symbol, NativeContract.GAS.Decimals, new Metadata(new Dictionary<string, JObject>() { { "script_hash", NativeContract.GAS.Hash.ToString() } })));
            return gasAmount;
        }

        public static Amount ToGasAmount(this BigInteger amount)
        {
            var gasAmount = new Amount(amount.ToString(), new Currency(NativeContract.GAS.Symbol, NativeContract.GAS.Decimals, new Metadata(new Dictionary<string, JObject>() { { "script_hash", NativeContract.GAS.Hash.ToString() } })));
            return gasAmount;
        }

        /// <summary>
        /// address or UInt160 string =>(UInt160)address hash
        /// </summary>
        /// <param name="address"></param>
        /// <param name="addressVersion"></param>
        /// <returns></returns>
        public static UInt160 ToUInt160(this string address, byte addressVersion)
        {
            return address.StartsWith("0x") ? UInt160.Parse(address) : address.ToScriptHash(addressVersion);
        }

        /// <summary>
        /// Public Key => (UInt160)address hash
        /// </summary>
        /// <param name="publicKeyPoint"></param>
        /// <returns></returns>

        public static UInt160 ToUInt160FromPublicKey(this ECPoint publicKeyPoint)
        {
            var script = Contract.CreateSignatureRedeemScript(publicKeyPoint);
            return script.ToScriptHash();
        }


        /// <summary>
        /// Public Key => (UInt160)address hash
        /// </summary>
        /// <param name="publicKeyHex"></param>
        /// <returns></returns>
        public static UInt160 ToUInt160FromPublicKey(this string publicKeyHex)
        {
            var point = ECPoint.FromBytes(publicKeyHex.HexToBytes(), ECCurve.Secp256r1);
            return point.ToUInt160FromPublicKey();
        }
    }
}
