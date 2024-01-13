using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using shopapp.entity;

namespace showapp.webui.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage ="Kategori adı zorunludur.")]
        [StringLength(100,MinimumLength =5, ErrorMessage ="Kategori icin 5-100 arası karakter girin")]
        public string? Name { get; set; }

        [Required(ErrorMessage ="Kategori url alanı zorunludur.")]
        [StringLength(100,MinimumLength =5, ErrorMessage ="Url icin 5-100 arası karakter girin")]
        public string Url { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
    }
}