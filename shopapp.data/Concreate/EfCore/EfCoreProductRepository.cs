using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shopapp.entity;
using shopapp.data.Abstract;

namespace shopapp.data.Concreate.EfCore
{
    public class EfCoreProductRepository : EfCoreGenericRepository<Product, ShopContext>, IProductRepository
    {
        public Product GetByIdWithCategories(int id) // ürünlerde kategorileri gosterme
        {
            using(var context = new ShopContext())
            {
                return context.Products
                                .Where(i=>i.ProductId == id)
                                .Include(i=>i.ProductCategories)
                                .ThenInclude(i=>i.Category)
                                .FirstOrDefault(); // ilk buldugunu gösterir çünkü tek bir product bilgisi gösteriyoruz
            }
        }

        public int GetCountByCategory(string category)
        {
           using(var context = new ShopContext())
           {
                var products = context.Products.AsQueryable(); //bütün listeyi alıcam. Sonra aşagida kriter eklicez.

                if(!string.IsNullOrEmpty(category))
                {
                    products = products
                                    .Include(i=>i.ProductCategories)
                                    .ThenInclude(i=>i.Category)
                                    .Where(i=>i.ProductCategories.Any(a=> a.Category.Url == category)); //herhangi bir kayıt var mı yok mu
                }
                return products.Count(); 
           }
            
        }
        public List<Product> GetPopularProducts()
        {
            using(var context = new ShopContext())
            {
                return context.Products.ToList();
            }
        }
        public Product GetProductDetails(string url)
        {
            using(var context = new ShopContext())
            {
                return context.Products
                                .Where(i=>i.Url == url )
                                .Include(i=>i.ProductCategories)
                                .ThenInclude(i=>i.Category)
                                .FirstOrDefault(); //bulduğun ilk kaydi getir
            }   
        }
        public List<Product> GetProductsByCategory(string name,int page ,int pageSize)
        {
           using(var context = new ShopContext())
           {
                var products = context.Products.AsQueryable(); //bütün listeyi alıcam. Sonra aşagida kriter eklicez.

                if(!string.IsNullOrEmpty(name))
                {
                    products = products
                                    .Include(i=>i.ProductCategories)
                                    .ThenInclude(i=>i.Category)
                                    .Where(i=>i.ProductCategories.Any(a=> a.Category.Url == name)); //herhangi bir kayıt var mı yok mu
                }
                return products.Skip((page-1)*pageSize).Take(pageSize).ToList(); 
           }
        }

        public List<Product> GetSearchResult(string searchString)
        {
           using(var context = new ShopContext())
           {
                var products = context
                    .Products
                    .Where(i=>i.IsApproved && (i.Name.ToLower().Contains(searchString.ToLower()) || i.Description.ToLower().Contains(searchString.ToLower())))
                    .AsQueryable(); //bütün listeyi alıcam. Sonra aşagida kriter eklicez.

                
                return products.ToList(); 
           } 
        }
        public List<Product> GetTop5Products()
        {
            throw new NotImplementedException();
        }

        public void Update(Product entity,int[] categoryIds)
        {
            using(var context = new ShopContext())
            {
                var product = context.Products
                                        .Include(i=>i.ProductCategories)
                                        .FirstOrDefault(i=>i.ProductId == entity.ProductId);

                if(product != null)
                {
                    product.Name = entity.Name;
                    product.Price = entity.Price;
                    product.Description = entity.Description;
                    product.Url = entity.Url;
                    product.ImageUrl = entity.ImageUrl;
                    product.IsApproved = entity.IsApproved;

                    product.ProductCategories = categoryIds.Select(categ_id=>new ProductCategory()
                    {
                        ProductId = entity.ProductId,
                        CategoryId = categ_id

                    }).ToList();
                    
                    context.SaveChanges(); // degsisiklikler veritabanına aktarır.
                }
            }
        }

    }
}