using GraphQL.Types;
using Neo.Network.P2P.Payloads;
using Neo;
using System;

namespace GraphQLPlugin.ModelType
{
    public class WitnessesType : ObjectGraphType<Witness>
    {
        public WitnessesType()
        {
            Field("Invocation", x => Convert.ToBase64String(x.InvocationScript));
            Field("Verification", x => Convert.ToBase64String(x.VerificationScript));
        }
    }
}
