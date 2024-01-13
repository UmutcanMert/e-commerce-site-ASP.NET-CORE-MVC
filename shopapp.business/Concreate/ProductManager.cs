using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using shopapp.business.Abstract;
using shopapp.data.Abstract;
using shopapp.data.Concreate.EfCore;
using shopapp.entity;

namespace shopapp.business.Concreate
{
    public class ProductManager : IProductService
    {
        private IProductRepository _productRepository;

        public ProductManager(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public void Create(Product entity)
        {
            // is kurallarini uygula
           _productRepository.Create(entity); //veritabanına ekler.
        }

        public void Delete(Product entity)
        {
            //is kurallari
            _productRepository.Delete(entity);
        }

        public List<Product> Getall()
        {
            return _productRepository.Getall();
        }

        public Product GetById(int id)
        {
            return _productRepository.GetById(id);
        }

        public int GetCountByCategory(string category)
        {
            return _productRepository.GetCountByCategory(category);
        }
        public  Product GetProductDetails(string url)
        {
            return _productRepository.GetProductDetails(url);
        }

        public List<Product> GetProductsByCategory(string name,int page,int pageSize)
        {
            return _productRepository.GetProductsByCategory(name, page, pageSize);
        }

        public List<Product> GetSearchResult(string searchString)
        {
            return _productRepository.GetSearchResult(searchString);
        }

        public Product GetByIdWithCategories(int id)
        {
            return _productRepository.GetByIdWithCategories(id); 
        }
        public void Update(Product entity)
        {
            _productRepository.Update(entity);
        }

        public void Update(Product entity,int[] categoryIds)
        {
            _productRepository.Update(entity,categoryIds);
        }
        
        public string ErrorMessage { get; set; }
        
        public bool Validation(Product entity)
        {
            var isValid = true;

            if(string.IsNullOrEmpty(entity.Name))
            {
                ErrorMessage += "ürün ismi girmelisiniz.\n";
            }

            if(entity.Price < 0)
            {
                ErrorMessage += "ürün fiyatı  negatif olamaz.\n";
            }
            return isValid;
        }
    }
}