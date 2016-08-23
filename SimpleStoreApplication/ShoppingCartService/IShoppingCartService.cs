using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace ShoppingCartService
{
    public interface IShoppingCartService
    {
        Task AddItem(ShoppingCartItem item);
        Task DeleteItem(string productName);
        Task<List<ShoppingCartItem>> GetItems(CancellationToken cancellationToken);
    }
}
