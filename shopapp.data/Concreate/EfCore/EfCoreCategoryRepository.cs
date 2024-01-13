using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;
using shopapp.data.Abstract;
using Microsoft.EntityFrameworkCore;

namespace shopapp.data.Concreate.EfCore
{
    public class EfCoreCategoryRepository : EfCoreGenericRepository<Category, ShopContext>, ICategoryRepository
    {
        public void DeleteFromCategory(int productId, int categoryId)
        {
            using(var context = new ShopContext())
            {
                var cmd = "delete from productcategory where ProductId=@p0 and CategoryId=@p1";
                context.Database.ExecuteSqlRaw(cmd,productId,categoryId); //db'den siler
            }
        }
        public List<Category> GetPopularCategories()
        {
            throw new NotImplementedException();
        }

        public Category GetByIdWithProducts(int categoryId)
        {
            using(var context = new ShopContext())
            {
                return context.Categories
                            .Where(i=> i.CategoryId == categoryId)
                            .Include(i=>i.ProductCategories)
                            .ThenInclude(i=>i.Product)
                            .FirstOrDefault();
            }
        }
    }
}