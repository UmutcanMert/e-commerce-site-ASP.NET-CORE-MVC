using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using shopapp.entity;
using shopapp.business.Abstract;

namespace showapp.webui.ViewComponents
{
    [ViewComponent(Name = "Categories")]
    public class CategoriesViewComponents: ViewComponent
    {
        private ICategoryService _categoryService;

        public CategoriesViewComponents(ICategoryService categoryService)
        {
            this._categoryService = categoryService;
        }

        public IViewComponentResult Invoke()
        {   
            if(RouteData.Values["category"] != null)
                ViewBag.SelectedCategory = RouteData?.Values["category"];
            
            return View(_categoryService.Getall());
            
        }
    }
}