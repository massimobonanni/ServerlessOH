using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BFYOC.Functions
{
    public  class CreateRating
    {
        private readonly IBackEndService backEndService;

        public CreateRating(IBackEndService service)
        {
            this.backEndService = service;
        }

        [FunctionName("CreateRating")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "ratings")] HttpRequest req,
            [CosmosDB(
                databaseName: CosmosDBNames.DatabaseName,
                collectionName: CosmosDBNames.Collectionname,
                ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<CreateRatingItem> ratingItems,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<CreateRatingModel>(requestBody);
           
            // Check the UserID 
            if (!await this.backEndService.CheckUserId(data.userId))
                return new NotFoundObjectResult("I don't know that user");

            // Check for ProductID
            if (!await this.backEndService.CheckProductId(data.productId))
                return new NotFoundObjectResult("You are using an amazing product but it doesn't exist");

            // Check the rating value
            if (data.rating<0 || data.rating>5)
                return new BadRequestObjectResult("Rating value out of range");

            // Create Object to save in Database
            var item = new CreateRatingItem(data);
            await ratingItems.AddAsync(item);
            
            return new OkObjectResult(item);
        }
    }

    public class CreateRatingModel
    {
        public string userId { get; set; }
        public string productId { get; set; }
        public string locationName { get; set; }
        public int rating { get; set; }
        public string userNotes { get; set; }
    }

    public class CreateRatingItem
    {
        public CreateRatingItem()
        {
            //this.id=Guid.NewGuid().ToString();
            //this.timeStamp=DateTimeOffset.Now;
        }

        public CreateRatingItem(CreateRatingModel source)
        {
            this.id=Guid.NewGuid().ToString();
            this.timeStamp=DateTimeOffset.Now;
            this.locationName=source.locationName;
            this.productId=source.productId;
            this.rating=source.rating;
            this.userId=source.userId;
            this.userNotes=source.userNotes;
        }

        public string id { get; set; }
        public DateTimeOffset timeStamp { get; set; }
        public string userId { get; set; }
        public string productId { get; set; }
        public string locationName { get; set; }
        public int rating { get; set; }
        public string userNotes { get; set; }
    }
}
