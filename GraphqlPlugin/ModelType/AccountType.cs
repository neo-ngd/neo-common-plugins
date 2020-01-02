using GraphQL.Types;
using Neo.IO.Json;

namespace GraphQLPlugin.ModelType
{
    public class AccountType : ObjectGraphType<Account>
    {
        public AccountType()
        {
            Field(x => x.Address);
            Field(x => x.HasKey);
            Field(x => x.Label);
            Field(x => x.WatchOnly);
        }
    }
    public class Account
    {
        public string Address { get; set; }

        public bool HasKey { get; set; }

        public string Label { get; set; }

        public bool WatchOnly { get; set; }

        public JObject ToJson()
        {
            JObject account = new JObject();
            account["address"] = Address;
            account["haskey"] = HasKey;
            account["label"] = Label;
            account["watchonly"] = WatchOnly;
            return account;
        }

        public static Account FromJson(JObject json)
        {
            Account account = new Account
            {
                Address = json["address"].AsString(),
                HasKey = json["haskey"].AsBoolean(),
                Label = json["label"]?.AsString(),
                WatchOnly = json["watchonly"].AsBoolean()
            };
            return account;
        }
    }

}
