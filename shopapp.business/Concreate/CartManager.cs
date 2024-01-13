using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.business.Abstract;
using shopapp.data.Abstract;
using shopapp.entity;

namespace shopapp.business.Concreate
{
    public class CartManager: ICartService
    {
        private ICartRepository _cartRepository;

        public CartManager(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public void AddToCart(string userId, int productId, int quantity)
        {
            var cart = GetCartByUserId(userId);
            if(cart != null)
            {
                /* eklenmek isteyen ürün sepette varmı(güncelleme) */
                /* eklenmek isteyen ürün sepette var ve yeni kayıt oluştur. */
                var index = cart.CartItems.FindIndex(i=>i.ProductId == productId);
                if(index < 0)
                {
                    cart.CartItems.Add(new CartItem(){
                        ProductId = productId,
                        Quantity  = quantity,
                        CartId = cart.Id
                    });
                }
                else
                {
                    cart.CartItems[index].Quantity += quantity;
                }

                _cartRepository.Update(cart);

            }
        }
        public void ClearCart(int cartId)
        {
            _cartRepository.ClearCart(cartId);
        }

        public void DeleteFromCart(string userId, int productId)
        {
            var cart = GetCartByUserId(userId);
            if(cart != null)
            {
                _cartRepository.DeleteFromCart(cart.Id,productId);
            }
        }

        public Cart GetCartByUserId(string userId)
        {
            return _cartRepository.GetByUserId(userId);
        }
        public void InitializeCart(string userId)
        {
            _cartRepository.Create(new Cart(){
                UserId = userId
            } );
        }
    }
}