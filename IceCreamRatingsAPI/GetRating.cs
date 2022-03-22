using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System.Linq;
using System.Collections.Generic;
namespace BFYOC.Functions
{
    public static class GetRating
    {
        [FunctionName("GetRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get",  Route = "ratings/{id}")] HttpRequest req,
            string id,
            [CosmosDB(
                databaseName: CosmosDBNames.DatabaseName,
                collectionName: CosmosDBNames.Collectionname,
                ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client,
                ILogger log)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(CosmosDBNames.DatabaseName, CosmosDBNames.Collectionname);

            IDocumentQuery<CreateRatingItem> query =
                client.CreateDocumentQuery<CreateRatingItem>(collectionUri,
                        new FeedOptions() { MaxItemCount = 1, EnableCrossPartitionQuery = true })
                .Where(p => p.id == id)
                .AsDocumentQuery();

            var item = (await query.ExecuteNextAsync<CreateRatingItem>()).FirstOrDefault();

            if (item == null)
                return new NotFoundResult();

            return new OkObjectResult(item);

        }
    }
}
