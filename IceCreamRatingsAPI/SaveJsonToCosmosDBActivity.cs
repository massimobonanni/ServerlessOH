using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using static BFYOC.Functions.FileAggregatorEntity;

namespace BFYOC.Functions
{
    public class SaveJsonToCosmosDBActivity
    {

        [FunctionName(nameof(SaveJsonToCosmosDBActivity))]
        public async Task Run([ActivityTrigger] IEnumerable<OrderBatch> orders,
            [CosmosDB(
                databaseName: CosmosDBNames.DatabaseName,
                collectionName: CosmosDBNames.OrderBatchesCollectionName,
                ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<OrderBatch> orderBatchesItems,
            ILogger log)
        {
            foreach (var order in orders)
            {
                await orderBatchesItems.AddAsync(order);
            }
        }

        
    }
}