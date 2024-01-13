using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;

namespace shopapp.data.Abstract
{   
    // sanal bir sınıf oluşturduk bunu concreate içinde dolduracağız.
    public interface IProductRepository: IRepository<Product>
    {
        Product GetProductDetails(string url);

        Product GetByIdWithCategories(int id);

        List<Product> GetProductsByCategory(string name,int page,int pageSize);

        List<Product> GetPopularProducts();

        List<Product> GetSearchResult(string searchString);

        List<Product> GetTop5Products();

        int GetCountByCategory(string category);

        void Update(Product entity,int[] categoryIds);

    }
}