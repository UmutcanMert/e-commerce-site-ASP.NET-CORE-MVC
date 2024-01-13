using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;

namespace shopapp.data.Abstract
{
    public interface ICategoryRepository: IRepository<Category>
    {
        List<Category> GetPopularCategories();
        
        Category GetByIdWithProducts(int categoryId);
    
        void DeleteFromCategory(int productId, int categoryId);
    }
}