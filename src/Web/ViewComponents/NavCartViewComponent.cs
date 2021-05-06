using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Interfaces;

namespace Web.ViewComponents
{
    public class NavCartViewComponent : ViewComponent
    {
        private readonly IBasketViewModelService _basketViewModelService;

        public NavCartViewComponent(IBasketViewModelService basketViewModelService)
        {
            _basketViewModelService = basketViewModelService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var basketId = await _basketViewModelService.GetOrCreateBasketIdAsync();
            var vm = await _basketViewModelService.GetBasketItemsCountViewModel(basketId);
            return View(vm);
        }
    }
}
