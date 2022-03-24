using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static BFYOC.Functions.FileAggregatorEntity;

namespace BFYOC.Functions
{
    public class CallExternalServiceActivity
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly IConfiguration configuration;

        public CallExternalServiceActivity(IHttpClientFactory clientFactory,
            IConfiguration configuration)
        {
            this.clientFactory = clientFactory;
            this.configuration = configuration;

        }

        [FunctionName(nameof(CallExternalServiceActivity))]
        public async Task<IEnumerable<OrderBatch>> Run([ActivityTrigger] IDictionary<FileType, string> files, ILogger log)
        {
            var retList = new List<OrderBatch>();

            var httpClient = this.clientFactory.CreateClient();
            try
            {
                var payload = new
                {
                    orderHeaderDetailsCSVUrl = "https://soh.blob.core.windows.net/six/XXXXXXXXXXXXXX-OrderHeaderDetails.csv",
                    orderLineItemsCSVUrl = "https://soh.blob.core.windows.net/six/XXXXXXXXXXXXXX-OrderLineItems.csv",
                    productInformationCSVUrl = "https://soh.blob.core.windows.net/six/XXXXXXXXXXXXXX-ProductInformation.csv"
                };

                var postContent = new StringContent(JsonConvert.SerializeObject(payload));

                var response = await httpClient.PostAsync(
                    $"https://serverlessohmanagementapi.trafficmanager.net/api/order/combineOrderContent",
                    postContent);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var orderBatch = new OrderBatch()
                    {
                        OrderHeaderDetailsCSVUrl = files[FileType.OrderHeaderDetails],
                        OrderLineItemsCSVUrl = files[FileType.OrderLineItems],
                        ProductInformationCSVUrl = files[FileType.ProductInformation],
                        Timestamp = DateTimeOffset.UtcNow,
                        Id = Guid.NewGuid().ToString()
                    };

                    retList.Add(orderBatch);
                }
            }
            catch (Exception ex)
            {
            }
            return retList;
        }

    }
}