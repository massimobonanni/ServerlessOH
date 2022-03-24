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
    public class FileAggregatorOrchestrator
    {
        [FunctionName(nameof(FileAggregatorOrchestrator))]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var input = context.GetInput<Dictionary<FileType, string>>();

            var orders=await context.CallActivityAsync<IEnumerable<OrderBatch>>(nameof(CallExternalServiceActivity), input);

            await context.CallActivityAsync(nameof(SaveJsonToCosmosDBActivity), orders);
        }

    }
}