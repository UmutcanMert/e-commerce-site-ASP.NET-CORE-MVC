using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using shopapp.business.Abstract;
using showapp.webui.Identity;
using Microsoft.AspNetCore.Identity;
using showapp.webui.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using shopapp.entity;

namespace showapp.webui.Controllers
{
    [Authorize]
    public class CartController: Controller
    {
        private ICartService _cartService;

        private UserManager<User> _userManager;

        public CartController(ICartService cartService,UserManager<User> userManager)
        {   
            _cartService = cartService;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));
            
            return View(new CartModel(){
                CardId = cart.Id,
                CartItems = cart.CartItems.Select(i=>new CartItemModel()
                {
                    CartItemId = i.Id,
                    ProductId = i.ProductId,
                    Name = i.Product.Name,
                    Price = (double)i.Product.Price,
                    ImageUrl = i.Product.ImageUrl,
                    Quantity = i.Quantity
                }).ToList()
            });
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var userId = _userManager.GetUserId(User);
            
            _cartService.AddToCart(userId,productId,quantity);

            return RedirectToAction("Index");
        }

        [HttpPost]

        public IActionResult DeleteFromCart(int productId)
        {
            var userId = _userManager.GetUserId(User);
            _cartService.DeleteFromCart(userId,productId);

            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            var userId = _userManager.GetUserId(User);

            var cart = _cartService.GetCartByUserId(userId);
            
            /* sepeti temizle */
            ClearCart(cart);
            return View(); /* Bir sayfa gözükücek*/
        }

        public void ClearCart(Cart cart)
        {
            _cartService.ClearCart(cart.Id);
        }
    }
}