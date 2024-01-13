using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shopapp.entity
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string? Name { get; set; }

        public string Url { get; set; }


        public List<ProductCategory> ProductCategories { get; set; }

    }
}