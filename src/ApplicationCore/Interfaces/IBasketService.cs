using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IBasketService
    {
        Task AddItemToBasket(int basketId, int productId, int quantity);

        Task<int> BasketItemsCount(int basketId);
    }
}
