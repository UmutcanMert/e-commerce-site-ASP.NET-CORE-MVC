using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;

namespace showapp.webui.Models
{
    public class ProductModel
    {
        public List<Category> SelectedCategories { get; set; } = new List<Category>();
        public int ProductId { get; set; }
        
        //ozellikler hemen altlarındaki fonksiyonlar icin olur.
        [Display(Name ="Name",Prompt ="Ürün ismi giriniz")]
        [Required(ErrorMessage ="İsim zorunlu bir alan")]
        [StringLength(60,MinimumLength =4, ErrorMessage ="Ürün listesi 5-10 karakter arasında olmalıdır")]
        public string Name { get; set; }

        
        [Required(ErrorMessage ="Fiyat zorunlu bir alan")]
        [Range(1,500000,ErrorMessage ="Fiyat için 1-ile 500000 arasında bir değer giriniz.")]
        public double? Price { get; set; }


        [Required(ErrorMessage ="Açıklama zorunlu bir alan")]
        [StringLength(100,MinimumLength =5, ErrorMessage ="Ürün listesi 5-100 karakter arasında olmalıdır")]
        public string Description { get; set; }


        //[Required(ErrorMessage ="ImageUrl zorunlu bir alan")]
        public string ImageUrl { get; set; }


        [Required(ErrorMessage ="Url zorunlu bir alan")]
        public string Url { get; set; } 

        public bool IsApproved { get; set; }



    }
}