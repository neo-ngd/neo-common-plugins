using GraphQL.Types;
using Neo.Ledger;
using Neo.SmartContract.Manifest;
using Neo.IO.Json;
using System;

namespace GraphQLPlugin.ModelType
{
    public class ContractsType : ObjectGraphType<ContractState>
    {
        public ContractsType()
        {
            Field("Hash", x => x.ScriptHash.ToString());
            Field("Script", x => Convert.ToBase64String(x.Script));
            Field(x => x.Manifest, type: typeof(ManifestType));
        }
    }

    public class ManifestType : ObjectGraphType<ContractManifest>
    {
        public ManifestType()
        {
            Field("Groups", x => x.Groups, type: typeof(ListGraphType<ContractGroupType>));
            Field("Features", x => x.Features, type: typeof(ContractFeaturesType));
            Field("Abi", x => x.Abi, type: typeof(ContractAbiType));
            Field("Permissions", x => x.Permissions, type: typeof(ListGraphType<PermissionType>));
            Field("Trusts", x => x.Trusts, type: typeof(ListGraphType<StringGraphType>));
            Field("SafeMethods", x => x.SafeMethods, type: typeof(ListGraphType<StringGraphType>));
        }
    }

    public class ContractGroupType : ObjectGraphType<ContractGroup>
    {
        public ContractGroupType()
        {
            Field("PubKey", x => x.PubKey.ToString());
            Field("Signature", x => Convert.ToBase64String(x.Signature));
        }
    }

    public class ContractFeaturesType : EnumerationGraphType<ContractFeatures> { }

    public class ContractAbiType : ObjectGraphType<ContractAbi>
    {
        public ContractAbiType()
        {
            Field("Hash", x => x.Hash.ToString());
            Field(x => x.EntryPoint, type: typeof(MethodDescriptorType));
            Field("Methods", x => x.Methods, type: typeof(ListGraphType<MethodDescriptorType>));
            Field("Events", x => x.Events, type: typeof(ListGraphType<EventDescriptorType>));
        }
    }

    public class MethodDescriptorType : ObjectGraphType<ContractMethodDescriptor>
    {
        public MethodDescriptorType()
        {
            Field(x => x.Name);
            Field("Parameters", x => x.Parameters, type: typeof(ListGraphType<ParameterDefinitionType>));
            Field("ReturnType", x => x.ReturnType.ToString());
        }
    }

    public class EventDescriptorType : ObjectGraphType<ContractEventDescriptor>
    {
        public EventDescriptorType()
        {
            Field(x => x.Name);
            Field("Parameters", x => x.Parameters, type: typeof(ListGraphType<ParameterDefinitionType>));
        }
    }

    public class ParameterDefinitionType : ObjectGraphType<ContractParameterDefinition>
    {
        public ParameterDefinitionType()
        {
            Field(x => x.Name);
            Field("Type", x => x.Type.ToString());
        }
    }

    public class PermissionType : ObjectGraphType<ContractPermission>
    {
        public PermissionType()
        {
            Field("Contract", x => (JString)x.Contract.ToJson(), type: typeof(StringGraphType));
            Field(x => x.Methods, type: typeof(ListGraphType<StringGraphType>));
        }
    }
}
