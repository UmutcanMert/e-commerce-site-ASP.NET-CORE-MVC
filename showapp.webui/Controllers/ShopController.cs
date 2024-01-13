using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using shopapp.business.Abstract;
using shopapp.entity;
using showapp.webui.Models;


namespace showapp.webui.Controllers
{
    public class ShopController: Controller
    {
        private IProductService _productService;

        public ShopController(IProductService productService)
        {
            this._productService = productService;
        }   

        // localhost:5656//products/telefon?page=1
        public IActionResult List(string category, int page=1)
        { 
            const int pageSize=10;
            
            var productViewModel = new ProductListViewModel()
            {
                PageInfo = new PageInfo()
                {
                    TotalItems = _productService.GetCountByCategory(category),
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    CurrentCategory = category
                }, 
                Products =_productService.GetProductsByCategory(category,page,pageSize)
            };
            return View(productViewModel);
        }

        public IActionResult Details(string url)
        {
            if(url == null)
            {
                return NotFound();
            }
            Product product = _productService.GetProductDetails(url);

            if(product==null)
            {
                return NotFound();
            }

            return View(new ProductDetailModel{
                Product = product,
                //geriye bir product kategorisi
                Categories = product.ProductCategories.Select(i=> i.Category).ToList()
            });
        }

        public IActionResult Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                // Arama terimi boş ise hata mesajı dönebilir veya başka bir işlem yapabilirsiniz.
                TempData["ErrorMessage"] = "Arama terimi boş bırakılamaz.";
                // Veya başka bir sayfaya yönlendirme
                return Redirect("/home/index");
            }

            var productViewModel = new ProductListViewModel()
            {
                Products =_productService.GetSearchResult(q)
            };
            return View(productViewModel);
        }

    }
}