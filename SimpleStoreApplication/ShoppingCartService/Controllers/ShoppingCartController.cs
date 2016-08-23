using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Common;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace ShoppingCartService.Controllers
{
    [RoutePrefix("api/carts")]
    public class ShoppingCartController : ApiController
    {
        private readonly IReliableStateManager stateManager;

        public ShoppingCartController(IReliableStateManager stateManager)
        {
            this.stateManager = stateManager;
        }

        [HttpPost]
        [Route("")]
        public async Task AddItem(ShoppingCartItem item)
        {
            var cart = await stateManager.GetOrAddAsync<IReliableDictionary<string, ShoppingCartItem>>("myCart");

            using (var transaction = stateManager.CreateTransaction())
            {
                await cart.AddOrUpdateAsync(transaction, item.ProductName, item, (k, v) => item);
                await transaction.CommitAsync();
            }
        }

        [HttpDelete]
        [Route("")]
        public async Task DeleteItem(string productName)
        {
            var cart = await stateManager.GetOrAddAsync<IReliableDictionary<string, ShoppingCartItem>>("myCart");

            using (var transaction = stateManager.CreateTransaction())
            {
                var item = await cart.TryGetValueAsync(transaction, productName);

                if (item.HasValue)
                {
                    await cart.TryRemoveAsync(transaction, productName);
                }

                await transaction.CommitAsync();
            }
        }

        [HttpGet]
        [Route("")]
        public async Task<List<ShoppingCartItem>> GetItems()
        {
            var cart = await stateManager.GetOrAddAsync<IReliableDictionary<string, ShoppingCartItem>>("myCart");
            var items = new List<ShoppingCartItem>();

            using (var transaction = stateManager.CreateTransaction())
            {
                var enumerator = (await cart.CreateEnumerableAsync(transaction)).GetAsyncEnumerator();

                var cancelSource = new CancellationTokenSource();

                while (await enumerator.MoveNextAsync(cancelSource.Token))
                {
                    items.Add(enumerator.Current.Value);
                }

                await transaction.CommitAsync();
            }

            return items;
        }
    }
}
