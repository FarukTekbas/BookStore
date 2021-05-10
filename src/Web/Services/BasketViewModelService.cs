using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using ApplicationCore.Specifications;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Services
{
    public class BasketViewModelService : IBasketViewModelService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAsyncRepository<Basket> _basketRepository;
        private readonly IBasketService _basketService;
        private readonly IAsyncRepository<Product> _productRepository;

        public BasketViewModelService(IHttpContextAccessor httpContextAccessor, IAsyncRepository<Basket> basketRepository, IBasketService basketService, IAsyncRepository<Product> productRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _basketRepository = basketRepository;
            _basketService = basketService;
            _productRepository = productRepository;
        }

        //returns basket items count.Returns  0 if basket does not exist.
        public async Task<BasketItemsCountViewModel> GetBasketItemsCountViewModel()
        {
            string buyerId = GetBuyerId();
            var vm = new BasketItemsCountViewModel();
            if (buyerId == null) return vm;
            var spec = new BasketSpecification(buyerId);
            var basket = await _basketRepository.FirstOrDefaultAsync(spec);
            if (basket == null) return vm;
            vm.BasketItemsCount = await _basketService.BasketItemsCount(basket.Id);
            return vm;

        }


        public async Task<BasketViewModel> GetBasketViewModel()
        {
            int basketId = await GetOrCreateBasketIdAsync();
            var specBasket = new BasketWithItemsSpecification(basketId);//sepeti öğeleriyle getirdik
            var basket = await _basketRepository.FirstOrDefaultAsync(specBasket);
            var productsIds = basket.Items.Select(x => x.ProductId).ToArray();//sepetteki öğelerin ilişkili olduğu urunleri getirdik
            var specProducts = new ProductsSpecification(productsIds);
            var products = await _productRepository.ListAsync(specProducts);
            var basketItems = new List<BasketItemViewModel>();//sepet ögelerini ürünbilgileriyle birlikte BasketItemViewModel nesnelerini oluşturduk ve listeye ekledik
            foreach (var item in basket.Items.OrderBy(x => x.Id))
            {
                var product = products.First(x => x.Id == item.ProductId);
                basketItems.Add(new BasketItemViewModel()
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    ProductName = product.Name,
                    Price = product.Price,
                    PictureUri = product.PictureUri
                });
            }
            return new BasketViewModel()//sepet ögelerii ile basketViewModeli oluşturduk. Sepet sayfasında göstereceğimiz şeyleri.
            {
                Id = basketId,
                BuyerId = basket.BuyerId,
                Items = basketItems
            };
        }

        public string GetBuyerId()
        {
            var context = _httpContextAccessor.HttpContext;
            var user = context.User;
            var anonId = context.Request.Cookies[Constants.BASKET_COOKIE_NAME];
            return user.FindFirstValue(ClaimTypes.NameIdentifier) ?? anonId;



        }

        public async Task<int> GetOrCreateBasketIdAsync()
        {
            var buyerId = GetOrCreateBuyerId();
            var spec = new BasketSpecification(buyerId);
            var basket = await _basketRepository.FirstOrDefaultAsync(spec);

            if (basket == null)
            {
                basket = new Basket() { BuyerId = buyerId };
                basket = await _basketRepository.AddAsync(basket);
            }
            return basket.Id;
        }

        public string GetOrCreateBuyerId()
        {
            var context = _httpContextAccessor.HttpContext;
            var user = context.User;

            // return user id if user is logged in
            if (user.Identity.IsAuthenticated)
            {
                return user.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            else
            {
                // return anonymous user id if a basket cookie exists
                if (context.Request.Cookies.ContainsKey(Constants.BASKET_COOKIE_NAME))
                {
                    return context.Request.Cookies[Constants.BASKET_COOKIE_NAME];
                }
                // create and return an anonymous user id
                else
                {
                    string newBuyerId = Guid.NewGuid().ToString();
                    var cookieOptions = new CookieOptions()
                    {
                        IsEssential = true,
                        Expires = DateTime.Now.AddYears(10)
                    };
                    context.Response.Cookies.Append(Constants.BASKET_COOKIE_NAME, newBuyerId, cookieOptions);
                    return newBuyerId;
                }
            }
        }

        public async Task TransferBasketsAsync(string userId)
        {
            var context = _httpContextAccessor.HttpContext;
            //Transfer Baskets
            var anonId = context.Request.Cookies[Constants.BASKET_COOKIE_NAME];
            if (!string.IsNullOrEmpty(anonId))
                await _basketService.TransferBasketAsync(anonId, userId);
            context.Response.Cookies.Delete(Constants.BASKET_COOKIE_NAME);
        }
    }
}
