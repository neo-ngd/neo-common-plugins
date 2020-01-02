using GraphQL;
using GraphQL.Types;

namespace Neo.Plugins
{
    public class RootSchema : Schema
    {
        public RootSchema(IDependencyResolver resolver) :
           base(resolver)
        {
            Query = resolver.Resolve<RootQuery>();
            Mutation = resolver.Resolve<RootMutation>();
        }
    }
}
