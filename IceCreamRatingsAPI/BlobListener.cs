// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Ice_Cream_Ratings_API
{
    public static class BlobListener
    {
        [FunctionName("BlobListener")]
        public static void Run([EventGridTrigger] EventGridEvent eventGridEvent,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            log.LogInformation(eventGridEvent.Data.ToString());

            var subject = eventGridEvent.Subject;
            var fileName = subject.Split(@"/").Last();
            var prefix = fileName.Split('-').First();

            var jData = eventGridEvent.Data as JObject;
            var fileUrl = jData.Property("url").ToString();

            var entityId = new EntityId(nameof(FileAggregatorEntity), prefix);
            client.SignalEntityAsync(entityId, nameof(FileAggregatorEntity.FileReceived), fileUrl);
        }
    }
}
