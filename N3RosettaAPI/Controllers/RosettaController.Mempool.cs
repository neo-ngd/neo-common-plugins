using Microsoft.AspNetCore.Mvc;
using Neo.IO.Json;
using Neo.Ledger;
using Neo.Wallets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using NeoTransaction = Neo.Network.P2P.Payloads.Transaction;

namespace Neo.Plugins
{
    partial class RosettaController
    {
        /// <summary>
        /// Get all Transaction Identifiers in the mempool
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JObject Mempool(NetworkRequest request)
        {
            if (request.NetworkIdentifier.Blockchain.ToLower() != "neo n3")
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();

            NeoTransaction[] neoTxes = system.MemPool.ToArray();
            TransactionIdentifier[] transactionIdentifiers = neoTxes.Select(p => new TransactionIdentifier(p.Hash.ToString())).ToArray();
            MempoolResponse response = new MempoolResponse(transactionIdentifiers);
            return response.ToJson();
        }

        /// <summary>
        /// Get a transaction in the mempool by its Transaction Identifier. 
        /// This is a separate request than fetching a block transaction (/block/transaction) because some blockchain nodes need to know that a transaction query 
        /// is for something in the mempool instead of a transaction in a block. 
        /// Transactions may not be fully parsable until they are in a block (ex: may not be possible to determine the fee to pay before a transaction is executed). 
        /// On this endpoint, it is ok that returned transactions are only estimates of what may actually be included in a block.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public JObject MempoolTransaction(MempoolTransactionRequest request)
        {
            if (request.NetworkIdentifier.Blockchain.ToLower() != "neo n3")
                return Error.NETWORK_IDENTIFIER_INVALID.ToJson();
            // check tx
            if (request.TransactionIdentifier == null)
                return Error.TX_IDENTIFIER_INVALID.ToJson();
            if (!UInt256.TryParse(request.TransactionIdentifier.Hash, out UInt256 txHash))
                return Error.TX_HASH_INVALID.ToJson();
            NeoTransaction neoTx = system.MemPool.ToArray().FirstOrDefault(p => p.Hash == txHash);
            if (neoTx == default(NeoTransaction))
                return Error.TX_NOT_FOUND.ToJson();

            Transaction tx = ConvertTx(neoTx);
            MempoolTransactionResponse response = new MempoolTransactionResponse(tx);
            return response.ToJson();
        }

        // cannot get operations from transaction in mempool, since some transfer may occur when a contract dynamic call takes place
        private Transaction ConvertTx(NeoTransaction neoTx)
        {
            Dictionary<string, JObject> metadata = new Dictionary<string, JObject>();

            metadata.Add("size", neoTx.Size);
            metadata.Add("version", neoTx.Version);
            metadata.Add("nonce", neoTx.Nonce);
            metadata.Add("sysfee", neoTx.SystemFee);
            metadata.Add("netfee", neoTx.NetworkFee);
            metadata.Add("sender", neoTx.Sender.ToAddress(system.Settings.AddressVersion));
            metadata.Add("script", Convert.ToBase64String(neoTx.Script));
            metadata.Add("signers", neoTx.Signers.Select(p => p.ToJson()).ToArray());
            metadata.Add("attributes", neoTx.Attributes.Select(p => p.ToJson()).ToArray());
            metadata.Add("witnesses", neoTx.Witnesses.Select(p => p.ToJson()).ToArray());

            return new Transaction(new TransactionIdentifier(neoTx.Hash.ToString()), Array.Empty<Operation>(), new Metadata(metadata));
        }
    }
}
