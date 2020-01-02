using GraphQL.Types;
using Neo.Network.P2P.Payloads;
using Neo.IO.Json;
using Neo.IO;
using System.Linq;
using System.Collections.Generic;
using Neo.SmartContract;
using Neo.Ledger;
using System;
using GraphQL;
using GraphQLPlugin.ModelType;

namespace Neo.Plugins
{
    public class RootMutation : ObjectGraphType
    {
        public RootMutation(IGraphService queryService)
        {
            Field<BooleanGraphType>("txbroadcasting",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "hex" }
            ), resolve: context =>
            {
                try
                {
                    var hex = context.GetArgument<string>("hex");
                    var tx = hex.HexToBytes().AsSerializable<Transaction>();
                    return queryService.SendRawTransaction(tx).AsBoolean();
                }
                catch (Exception ex)
                {
                    context.Errors.Add(new ExecutionError(ex.Message));
                    return false;
                }
            });

            Field<BooleanGraphType>("blockrelaying",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "hex" }
            ), resolve: context =>
            {
                try
                {
                    var hex = context.GetArgument<string>("hex");
                    var block = hex.HexToBytes().AsSerializable<Block>();
                    return queryService.SubmitBlock(block).AsBoolean();
                }
                catch (Exception ex)
                {
                    context.Errors.Add(new ExecutionError(ex.Message));
                    return false;
                }
            });
        }
    }

    public class RootQuery : ObjectGraphType
    {
        public RootQuery(IGraphService queryService)
        {
            Field<StringGraphType>("bestblockhash", resolve:
            context => queryService.GetBestBlockHash());

            Field<BlockType>("block",
                arguments: new QueryArguments(
                    new QueryArgument<GraphQLPlugin.ModelType.UIntGraphType> { Name = "index" },
                    new QueryArgument<StringGraphType> { Name = "hash" }
            ), resolve: context =>
            {
                string hash = context.GetArgument<string>("hash");
                uint index = context.GetArgument<uint>("index");
                bool isVerbose = true;
                try
                {
                    if (hash != null)
                    {
                        return GraphBlock.FromJson(queryService.GetBlock(hash, isVerbose));
                    }
                    else
                    {
                        return GraphBlock.FromJson(queryService.GetBlock(index, isVerbose));
                    }
                }
                catch (Exception ex)
                {
                    context.Errors.Add(new ExecutionError(ex.Message));
                    return false;
                }
            });

            Field<IntGraphType>("blockcount", resolve: context =>
            {
                return queryService.GetBlockCount();
            });

            Field<StringGraphType>("blocksysfee",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<GraphQLPlugin.ModelType.UIntGraphType>> { Name = "index" }
                ), resolve: context =>
            {
                var index = context.GetArgument<uint>("index");
                return queryService.GetBlockSysFee(index);
            });

            Field<TransactionsType>("transaction",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "txid" }
                ), resolve: context =>
            {
                var txid = UInt256.Parse(context.GetArgument<string>("txid"));
                bool verbose = true;
                return GraphTransaction.FromJson(queryService.GetRawTransaction(txid, verbose));
            });

            Field<GraphQLPlugin.ModelType.UIntGraphType>("transactionheight",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "txid" }
              ), resolve: context =>
            {
                var txid = UInt256.Parse(context.GetArgument<string>("txid"));
                return queryService.GetTransactionHeight(txid);
            });

            Field<ContractsType>("contract",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "scripthash" }
            ), resolve: context =>
            {
                var script_hash = UInt160.Parse(context.GetArgument<string>("scripthash"));
                return ContractState.FromJson(queryService.GetContractState(script_hash));
            });

            Field<ListGraphType<ValidatorsType>>("validators", resolve: context =>
            {
                return ((JArray)queryService.GetValidators()).Select(p => GraphValidator.FromJson(p)).ToArray();
            });

            Field<PeersType>("peers", resolve: context =>
            {
                return GraphPeers.FromJson(queryService.GetPeers());
            });

            Field<VersionType>("version", resolve: context =>
            {
                return queryService.GetVersion();
            });

            Field<IntGraphType>("connectioncount", resolve: context =>
            {
                return queryService.GetConnectionCount();
            });

            Field<ListGraphType<PluginsType>>("plugins", resolve: context =>
            {
                return ((JArray)queryService.ListPlugins()).Select(p => GraphPlugin.FromJson(p)).ToArray();
            });

            Field<BooleanGraphType>("addressverification",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "address" }
           ), resolve: context =>
           {
               var address = context.GetArgument<string>("address");
               return queryService.ValidateAddress(address)["isvalid"].AsBoolean();
           });

            Field<InvokeResultType>("scriptinvocation", //
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "script" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "hashes" }
            ), resolve: context =>
            {
                byte[] script = context.GetArgument<string>("script").HexToBytes();
                var hashes = context.GetArgument<List<string>>("scripthashes")?.ToArray();
                UInt160[] scriptHashesForVerifying = null;
                if (hashes != null && hashes.Length > 0)
                {
                    scriptHashesForVerifying = hashes.Select(u => UInt160.Parse(u)).ToArray();
                }
                return GraphInvokeResult.FromJson(queryService.InvokeScript(script, scriptHashesForVerifying));
            });

            Field<InvokeResultType>("functioninvocation", //
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "scripthash" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "operation" },
                    new QueryArgument<ListGraphType<RpcStackInputType>> { Name = "params" }
            ), resolve: context =>
            {
                UInt160 script_hash = UInt160.Parse(context.GetArgument<string>("scripthash"));
                string operation = context.GetArgument<string>("operation");
                ContractParameter[] args = context.GetArgument<List<RpcStack>>("params")?.Select(p => ContractParameter.FromJson(p.ToJson()))?.ToArray() ?? new ContractParameter[0];
                return GraphInvokeResult.FromJson(queryService.InvokeFunction(script_hash, operation, args));
            });

            Field<StringGraphType>("getstorage",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "scripthash" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "key" }
            ), resolve: context =>
            {
                var scriptHash = UInt160.Parse(context.GetArgument<string>("scripthash"));
                var key = context.GetArgument<string>("key").HexToBytes();
                return queryService.GetStorage(scriptHash, key)?.AsString();
            });

            Field<BooleanGraphType>("closewallet", resolve: context =>
            {
                return queryService.CloseWallet();
            });

            Field<StringGraphType>("dumpprivkey",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "address" }
            ), resolve: context =>
            {
                var address = context.GetArgument<string>("address");
                return queryService.DumpPrivKey(address);
            });

            Field<StringGraphType>("getbalance",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "assetID" }
            ), resolve: context =>
            {
                var assetID = context.GetArgument<string>("assetID");
                return queryService.GetBalance(assetID);
            });

            Field<StringGraphType>("newaddress", resolve: context =>
            {
                return queryService.GetNewAddress();
            });

            Field<StringGraphType>("unclaimedgas", resolve: context =>
            {
                return queryService.GetUnclaimedGas();
            });

            Field<AccountType>("importprivkey",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "privkey" }
            ), resolve: context =>
            {
                var privkey = context.GetArgument<string>("privkey");
                return Account.FromJson(queryService.ImportPrivKey(privkey));
            });

            Field<ListGraphType<AccountType>>("listaddress", resolve: context =>
           {
               return ((JArray)queryService.ListAddress()).Select(p => Account.FromJson(p)).ToArray();
           });

            Field<BooleanGraphType>("openwallet",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "path" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "password" }
            ), resolve: context =>
            {
                var path = context.GetArgument<string>("path");
                var password = context.GetArgument<string>("password");
                return queryService.OpenWallet(path, password).AsBoolean();
            });
        }
    }
}
