using Neo.Cryptography;
using Neo.IO.Json;
using Neo.Network.P2P.Payloads;
using System;

namespace Neo.Plugins
{
    internal static class Helper
    {
        //public static readonly RegisterTransaction NEO = Blockchain.GoverningToken;
        //public static readonly RegisterTransaction GAS = Blockchain.UtilityToken;

        public static string IntToHash160String(this int value)
        {
            return new UInt160(Crypto.Default.Hash160(BitConverter.GetBytes(value))).ToString();
        }

        public static string IntToHash256String(this int value)
        {
            return new UInt160(Crypto.Default.Hash256(BitConverter.GetBytes(value))).ToString();
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

        public static TransactionType ToTransactionType(this string type)
        {
            switch (type)
            {
                case "ClaimTransaction":
                    return TransactionType.ClaimTransaction;
                case "ContractTransaction":
                    return TransactionType.ContractTransaction;
                case "InvocationTransaction":
                    return TransactionType.InvocationTransaction;
                case "StateTransaction":
                    return TransactionType.StateTransaction;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
