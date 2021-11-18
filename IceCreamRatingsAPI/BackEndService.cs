using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;

namespace BFYOC.Functions
{
    public class BackEndService : IBackEndService
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly IConfiguration configuration;

        public BackEndService(IHttpClientFactory clientFactory,
            IConfiguration configuration)
        {
            this.clientFactory = clientFactory;
            this.configuration = configuration;

        }

        public async Task<bool> CheckProductId(string productId)
        {
            var httpClient = this.clientFactory.CreateClient();
            try
            {
                var response = await httpClient.GetAsync($"{GetBaseUrl()}/GetProduct?productId={productId}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var product = JsonConvert.DeserializeObject<ProductDto>(responseContent);
                    return !string.IsNullOrWhiteSpace(product.productId);
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public async Task<bool> CheckUserId(string userId)
        {
            var httpClient = this.clientFactory.CreateClient();
            try
            {
                var response = await httpClient.GetAsync($"{GetBaseUrl()}/GetUser?userId={userId}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<UserDto>(responseContent);
                    return !string.IsNullOrWhiteSpace(user.userId);
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        private string GetBaseUrl()
        {
            var baseUrl = this.configuration.GetValue<string>("BackEndServiceUrl");
            return baseUrl;
        }

        public class ProductDto
        {
            public string productId { get; set; }
            public string productName { get; set; }
            public string productDescription { get; set; }
        }
        public class UserDto
        {
            public string userId { get; set; }
            public string userName { get; set; }
            public string fullName { get; set; }
        }
    }
}