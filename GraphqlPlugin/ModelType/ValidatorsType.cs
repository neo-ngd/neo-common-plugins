using GraphQL.Types;
using Neo.IO.Json;
using System.Numerics;

namespace GraphQLPlugin.ModelType
{
    public class ValidatorsType: ObjectGraphType<GraphValidator>
    {
        public ValidatorsType()
        {
            Field(x => x.PublicKey);
            Field("Votes", x => x.Votes.ToString());
            Field(x => x.Active);
        }
    }

    public class GraphValidator
    {
        public string PublicKey { get; set; }

        public BigInteger Votes { get; set; }

        public bool Active { get; set; }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json["publickey"] = PublicKey;
            json["votes"] = Votes.ToString();
            json["active"] = Active;
            return json;
        }

        public static GraphValidator FromJson(JObject json)
        {
            GraphValidator validator = new GraphValidator();
            validator.PublicKey = json["publickey"].AsString();
            validator.Votes = BigInteger.Parse(json["votes"].AsString());
            validator.Active = json["active"].AsBoolean();
            return validator;
        }
    }
}
