using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;

namespace shopapp.business.Abstract
{
    public interface IProductService
    {
        Product GetById(int id);

        Product GetByIdWithCategories(int id);
        Product GetProductDetails(string url);

        List<Product> GetProductsByCategory(string name,int page,int pageSize);

        List<Product> Getall();

        List<Product> GetSearchResult(string searchString);

        void Create(Product entity);

        void Update(Product entity);

        void Update(Product entity, int[] categoryIds);

        void Delete(Product entity);

        int GetCountByCategory(string category);
    }
}