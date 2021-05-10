using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Controllers
{
    public class BasketController : Controller
    {
        private readonly IBasketViewModelService _basketViewModelService;
        private readonly IBasketService _basketService;

        public BasketController(IBasketViewModelService basketViewModelService, IBasketService basketService)
        {
            _basketViewModelService = basketViewModelService;
            _basketService = basketService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _basketViewModelService.GetBasketViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToBasket(int productId, int quantity)
        {
            var basketId = await _basketViewModelService.GetOrCreateBasketIdAsync();
            await _basketService.AddItemToBasket(basketId, productId, quantity);

            return Json(await _basketViewModelService.GetBasketItemsCountViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBasketItem(int basketItemId)
        {
            var basketId = await _basketViewModelService.GetOrCreateBasketIdAsync();
            await _basketService.DeleteBasketItem(basketId, basketItemId);
            return PartialView("_BasketPartial", await _basketViewModelService.GetBasketViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBasketItem(int basketItemId, int quantity)
        {
            if (quantity < 1)
                return BadRequest("The quantity cannot be less than 1.");

            var basketId = await _basketViewModelService.GetOrCreateBasketIdAsync();
            await _basketService.UpdateBasketItem(basketId, basketItemId, quantity);
            return PartialView("_BasketPartial", await _basketViewModelService.GetBasketViewModel());
        }
    }
}
