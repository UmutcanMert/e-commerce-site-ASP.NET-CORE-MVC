using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using shopapp.entity;
using shopapp.data.Abstract;
using shopapp.business.Abstract;
// using showapp.webui.ViewModels;
using showapp.webui.Models;

namespace showapp.webui.Controllers
{
    public class HomeController:Controller
    {
        private IProductService _productService;

        public HomeController(IProductService productService)
        {
            this._productService = productService;
        }
        public IActionResult Index()
        { 

            var productViewModel = new ProductListViewModel()
            {
                Products =_productService.Getall()
            };
            return View(productViewModel);
        }

        //localhost:5256/home/about
        public IActionResult About()
        {
            return View();
        }
                
        public IActionResult Contact()
        {
            return View("MyView");
        }
    }
}