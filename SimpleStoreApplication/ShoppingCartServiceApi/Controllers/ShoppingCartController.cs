using System;
using System.Collections.Generic;
using System.Fabric;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using Common;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;

namespace ShoppingCartServiceApi.Controllers
{
    [RoutePrefix("api/carts")]
    public class ShoppingCartController : ApiController
    {
        private readonly Uri serviceUri;
        private readonly FabricClient fabricClient;
        private readonly HttpCommunicationClientFactory communicationFactory;

        public ShoppingCartController()
        {
            serviceUri = new Uri(FabricRuntime.GetActivationContext().ApplicationName + "/ShoppingCartService");

            fabricClient = new FabricClient();

            communicationFactory = new HttpCommunicationClientFactory(new ServicePartitionResolver(() => fabricClient));
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> AddItem(ShoppingCartItem item)
        {
            ShoppingCartItem result = null;

                var partitionClient =
                    new ServicePartitionClient<HttpCommunicationClient>(
                        communicationFactory,
                        serviceUri,
                        new ServicePartitionKey(1));

                await partitionClient.InvokeWithRetryAsync(
                    async client =>
                    {
                        var response = await client.HttpClient.PostAsync(
                            new Uri(client.Url, "api/carts"), 
                            new ObjectContent(
                                typeof(ShoppingCartItem),
                                item,
                                new JsonMediaTypeFormatter()));

                        result = await response.Content.ReadAsAsync<ShoppingCartItem>();
                    });

            return Created("", result);
        }

        [HttpGet]
        [Route("")]
        public async Task<List<ShoppingCartItem>> GetItems()
        {
            var items = new List<ShoppingCartItem>();

            var partitionClient =
                new ServicePartitionClient<HttpCommunicationClient>(
                    communicationFactory,
                    serviceUri,
                    new ServicePartitionKey(2));

            await partitionClient.InvokeWithRetryAsync(
                async client =>
                {
                    var response = await client.HttpClient.GetAsync(new Uri(client.Url, "api/carts"));
                    items = await response.Content.ReadAsAsync<List<ShoppingCartItem>>();

                });

            return items;
        }
    }
}
