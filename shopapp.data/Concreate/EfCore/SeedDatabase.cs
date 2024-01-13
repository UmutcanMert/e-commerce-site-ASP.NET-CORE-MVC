using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;
using Microsoft.EntityFrameworkCore;

namespace shopapp.data.Concreate.EfCore
{
    public static class SeedDatabase
    {
        public static void Seed()
        {
            var context = new ShopContext();

            // eğer tüm migrationlar uygulandıysa test verileri veri tabanına ekleyebiliriz.
            if(context.Database.GetPendingMigrations().Count() == 0 )
            {   
                if(context.Categories.Count() == 0)
                {
                    context.Categories.AddRange(Categories);
                }
                
                if(context.Products.Count() == 0)
                {
                    context.Products.AddRange(Products);
                    context.AddRange(ProductCategories);
                }
            }
            context.SaveChanges();

        }
        private static Category[] Categories= {
            new Category(){Name="Telefon",Url="telefon"},
            new Category(){Name="Bilgisayar",Url="bilgisayar"},
            new Category(){Name="Elektronik",Url="elektronik"},
            new Category(){Name="Beyaz Eşya",Url="beyaz-esya"}


        };

        private static Product[] Products= {
            new Product(){Name="Iphone 13",Url="telefon-iphone-13", Price=95000,ImageUrl="3.jpg",Description="İyi telefon",IsApproved=true},
            new Product(){Name="Samsung S10",Url="telefon-samsung-s10", Price=15000,ImageUrl="4.jpg",Description="İyi telefon",IsApproved=true},
            new Product(){Name="Samsung S9",Url="telefon-samsung-s19", Price=10000,ImageUrl="5.jpg",Description="İyi telefon",IsApproved=false},
            new Product(){Name="Samsung A10",Url="telefon-samsung-a10", Price=9500,ImageUrl="1.jpg",Description="İyi telefon",IsApproved=true},
            new Product(){Name="Iphone X",Url="telefon-iphone-x", Price=20000,ImageUrl="2.jpg",Description="İyi telefon",IsApproved=false},
            new Product(){Name="Oppo",Url="telefon-oppo", Price=7500,ImageUrl="3.jpg",Description="İyi telefon",IsApproved=true},
            new Product(){Name="Huawei Y7",Url="telefon-huwaei-y7", Price=5000,ImageUrl="4.jpg",Description="İyi telefon",IsApproved=true},
            new Product(){Name="Huawei Y9",Url="telefon-huwaei-y9", Price=7500,ImageUrl="5.jpg",Description="İyi telefon",IsApproved=false},
            new Product(){Name="Xiaomi Redmi Note 9",Url="telefon-xiaomi-redmi-note-9", Price=10000,ImageUrl="4.jpg",Description="İyi telefon",IsApproved=true},
            new Product(){Name="Xiaomi Redmi Note 10",Url="telefon-xiaomi-redmi-note-10", Price=17000,ImageUrl="2.jpg",Description="İyi telefon",IsApproved=true}
        };
        
        //db de çoka çok iliski icin asağıdaki yapi tanimlandi.
        private static ProductCategory[] ProductCategories={
            new ProductCategory(){Product=Products[0], Category=Categories[0]},
            new ProductCategory(){Product=Products[0], Category=Categories[2]},
            new ProductCategory(){Product=Products[1], Category=Categories[0]},
            new ProductCategory(){Product=Products[1], Category=Categories[2]},
            new ProductCategory(){Product=Products[2], Category=Categories[0]},
            new ProductCategory(){Product=Products[2], Category=Categories[2]},
            new ProductCategory(){Product=Products[3], Category=Categories[0]},
            new ProductCategory(){Product=Products[3], Category=Categories[2]},

        };
    }   
}