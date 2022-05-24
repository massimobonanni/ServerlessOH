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
using Microsoft.Extensions.Configuration;

namespace BFYOC.Functions
{
    public class GetRating
    {
        private readonly IConfiguration configuration;

        public GetRating(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [FunctionName("GetRating")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ratings/{id}")] HttpRequest req,
            string id,
            [CosmosDB(
                databaseName: "%DatabaseName%",
                collectionName: "%CollectionName%",
                ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client,
                ILogger log)
        {
            var databaseName = this.configuration["DatabaseName"];
            var collectionName = this.configuration["CollectionName"];

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);

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
