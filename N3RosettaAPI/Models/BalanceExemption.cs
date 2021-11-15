using Neo.IO.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Plugins
{
    // BalanceExemption indicates that the balance for an exempt account could change without a corresponding Operation.
    // This typically occurs with staking rewards, vesting balances, and Currencies with a dynamic supply.
    // Currently, it is possible to exempt an account from strict reconciliation by SubAccountIdentifier.Address or by Currency.
    // This means that any account with SubAccountIdentifier.Address would be exempt or any balance of a particular Currency would be exempt, respectively.
    // BalanceExemptions should be used sparingly as they may introduce significant complexity for integrators that attempt to reconcile all account balance changes.
    // If your implementation relies on any BalanceExemptions, you MUST implement historical balance lookup (the ability to query an account balance at any BlockIdentifier).
    public class BalanceExemption
    {
        // SubAccountAddress is the SubAccountIdentifier.
        // Address that the BalanceExemption applies to (regardless of the value of SubAccountIdentifier.Metadata).
        public string SubAccountAddress { get; set; }

        // Currency is composed of a canonical Symbol and Decimals.
        // This Decimals value is used to convert an Amount.Value from atomic units (Satoshis) to standard units (Bitcoins).
        public Currency Currency { get; set; }

        // ExemptionType is used to indicate if the live balance for an account subject to a BalanceExemption could increase above, decrease below, or equal the computed balance.
        public ExemptionType ExemptionType { get; set; }

        public BalanceExemption(string subAccountAddress = null, Currency currency = null, ExemptionType exemptionType = ExemptionType.Unknown)
        {
            SubAccountAddress = subAccountAddress;
            Currency = currency;
            ExemptionType = exemptionType;
        }

        public static BalanceExemption FromJson(JObject json)
        {
            return new BalanceExemption(json.ContainsProperty("sub_account_address") ? json["sub_account_address"].AsString() : null,
                json.ContainsProperty("currency") ? Currency.FromJson(json["currency"]) : null,
                json.ContainsProperty("exemption_type") ? json["exemption_type"].ToExemptionType() : ExemptionType.Unknown);
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            if (!string.IsNullOrEmpty(SubAccountAddress))
                json["sub_account_address"] = SubAccountAddress;
            if (Currency != null)
                json["currency"] = Currency.ToJson();
            if (ExemptionType != ExemptionType.Unknown)
                json["exemption_type"] = ExemptionType.AsString();
            return json;
        }
    }
}
