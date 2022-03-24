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
    public class GetRatings
    {
        private readonly IBackEndService backEndService;

        public GetRatings(IBackEndService service)
        {
            this.backEndService = service;
        }

        [FunctionName("GetRatings")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "ratings")] HttpRequest req,
            [CosmosDB(
                databaseName: CosmosDBNames.DatabaseName,
                collectionName: CosmosDBNames.RatingsCollectionName,
                ConnectionStringSetting = "CosmosDBConnection" )] DocumentClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string userId = req.Query["userId"];

            if (!await this.backEndService.CheckUserId(userId))
                return new BadRequestObjectResult("I don't know that user");

            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(CosmosDBNames.DatabaseName, CosmosDBNames.RatingsCollectionName);

            IDocumentQuery<CreateRatingItem> query = 
                client.CreateDocumentQuery<CreateRatingItem>(collectionUri)
                .Where(p => p.userId == userId)
                .AsDocumentQuery();

            var returnList = new List<CreateRatingItem>();
            
            while (query.HasMoreResults)
            {
                foreach (var result in await query.ExecuteNextAsync<CreateRatingItem>())
                {
                    returnList.Add(result);
                }
            }

            return new OkObjectResult(returnList);
        }

    }
}
