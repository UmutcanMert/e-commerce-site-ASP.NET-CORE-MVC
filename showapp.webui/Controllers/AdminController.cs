using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using shopapp.business.Abstract;
using shopapp.entity;
using showapp.webui.Models;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using showapp.webui.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
// using showapp.webui.ViewModels;

namespace showapp.webui.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController: Controller
    {
        private IProductService _productService;

        private ICategoryService _categoryService;

        private RoleManager<IdentityRole> _roleManager;

        private UserManager<User> _userManager;

        public AdminController(IProductService productService, ICategoryService categoryService,RoleManager<IdentityRole> roleManager,UserManager<User> userManager)
        {
            _productService = productService;
            _categoryService = categoryService;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult UserList()
        {
            return View(_userManager.Users);
        }


        public async Task<IActionResult> RoleEdit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            var members = new List<User>();
            var nonmembers = new List<User>();

            foreach( var user in _userManager.Users)
            {
                var list = await _userManager.IsInRoleAsync(user,role.Name)?members:nonmembers;
                list.Add(user);
            }
            
            var model = new RoleDetails()
            {
                Role = role,
                Members = members,
                NonMembers = nonmembers
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleEditModel model)
        {
            if(ModelState.IsValid)
            {
                foreach (var userId in model.IdsToAdd?? new string[]{})
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if(user != null)
                    {
                        var result = await _userManager.AddToRoleAsync(user, model.RoleName);
                        if(!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("",error.Description);
                            }
                        }
                    }
                    
                }

                foreach (var userId in model.IdsToDelete ?? new string[]{})
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if(user != null)
                    {
                        var result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                        if(!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("",error.Description);
                            }
                        }
                    }
                    
                }
            }
            else
            {
                 // Model doğrulama başarısızsa hataları kontrol et
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        var errorMessage = error.ErrorMessage;
                        var exception = error.Exception;
                        // Hata mesajını loglama veya işlem yapma

                        Console.WriteLine($"ModelState Error: {errorMessage}");

                    }
                }
            }
            return Redirect("/admin/role/"+model.RoleId);
        }
        public IActionResult RoleList()
        {
            return View(_roleManager.Roles);
        }

        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(model.Name)); 
                if(result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("",error.Description);
                    }
                }
            }
            return View();
        }

        public IActionResult ProductList()
        {
            return View(new ProductListViewModel()
            {
                Products = _productService.Getall()
            });
        }

        public IActionResult CategoryList()
        {
            return View(new CategoryListViewModel()
            {
                Categories = _categoryService.Getall()
            });
        }

        [HttpGet]
        public IActionResult ProductCreate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ProductCreate(ProductModel model)
        {   
        
            if(ModelState.IsValid)
            {
                var entity = new Product()
                {
                    Name = model.Name,
                    Url = model.Url,
                    Price = model.Price,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl
                };
                _productService.Create(entity);

                TempData["message"]=$"{entity.Name} isimli ürün eklendi.";

                return RedirectToAction("ProductList");
            }

            return View(model); // productcreate sayfasına gider yine.
        }

        [HttpGet]
        public IActionResult CategoryCreate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CategoryCreate(CategoryModel model)
        {   
            if(ModelState.IsValid)
            {
                var entity = new Category()
                {
                    Name = model.Name,
                    Url = model.Url,
                };
                _categoryService.Create(entity);

                TempData["message"]=$"{entity.Name} isimli kategori eklendi.";

                return RedirectToAction("CategoryList");
            }
            else
            {
                return View(model);
            }

        }

        [HttpGet]
        public IActionResult ProductEdit(int? id)
        {   
            if(id == null )
            {
                return NotFound();
            }

            var entity = _productService.GetByIdWithCategories((int)id);
            if(entity == null)
            {
                return NotFound();
            }
            var model = new ProductModel() //veritabanından bilgileri alıyoruz
            {
                ProductId = entity.ProductId,
                Name = entity.Name,
                Url = entity.Url,
                Price = entity.Price,
                ImageUrl = entity.ImageUrl,
                Description = entity.Description,
                IsApproved = entity.IsApproved,
                SelectedCategories = entity.ProductCategories.Select(i=>i.Category).ToList()
                
            };

            ViewBag.Categories = _categoryService.Getall(); // bütün kategori bilgileri geliyor.

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductModel model, int[] categoryIds, IFormFile? file)
        {   
            if(ModelState.IsValid)
            {
                var entity = _productService.GetById(model.ProductId);
                if(entity == null)
                {
                    return NotFound();
                }
                entity.Name = model.Name;
                entity.Price = model.Price;
                entity.Url = model.Url;
               // entity.ImageUrl = model.ImageUrl;
                entity.Description = model.Description;
                entity.IsApproved = model.IsApproved;

                if(file != null) //resim upload, ımageurl kısmına. veritabanına resim ismi gider.
                {
                    entity.ImageUrl = file.FileName;
                    
                    var extension = Path.GetExtension(file.FileName); //gelen dosyanın uzantısını aliyor. 
                    var randomName = string.Format($"{Guid.NewGuid()}{extension}"); // resim ismi icin benzersiz isimler verir.
                    
                    entity.ImageUrl = randomName;
                    //altta resmin tam yolunu alarak ilgili klasore ekliyoruz.
                    var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\images",randomName);

                    using(var stream = new FileStream(path,FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                else
                {
                   // entity.ImageUrl = model.ImageUrl;
                }
                _productService.Update(entity, categoryIds); //veri tabanına güncellenmiş halde gönderiyoruz.
                
                TempData["message"]=$"{entity.Name} isimli ürün güncellendi.";  //mesaj gönderilir.

                return RedirectToAction("ProductList");
            }else
            {
                ViewBag.Categories = _categoryService.Getall(); // bütün kategori bilgileri geliyor.

                return View(model);
            }

        }

        [HttpGet]
        public IActionResult CategoryEdit(int? id)
        {   
            if(id == null )
            {
                return NotFound();
            }

            var entity = _categoryService.GetByIdWithProducts((int)id);
            if(entity == null)
            {
                return NotFound();
            }
            var model = new CategoryModel() //veritabanından bilgileri alıyoruz
            {
                CategoryId = entity.CategoryId,
                Name = entity.Name,
                Url = entity.Url,
                Products = entity.ProductCategories.Select(p=>p.Product).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult CategoryEdit(CategoryModel model)
        {   
            if(ModelState.IsValid)
            {
                var entity = _categoryService.GetById(model.CategoryId);
                if(entity == null)
                {
                    return NotFound();
                }
                entity.Name = model.Name;
                entity.Url = model.Url;

                _categoryService.Update(entity); //veri tabanına güncellenmiş halde gönderiyoruz.
                
                TempData["message"]=$"{entity.Name} isimli kategory güncellendi.";  //mesaj gönderilir.

                return RedirectToAction("CategoryList");
            }
            else
            {   
                return View(model);
            }

        }
        
        public IActionResult DeleteProduct(int productId)
        {
            var entity = _productService.GetById(productId);
            
            if(entity != null)
            {
                _productService.Delete(entity);
            }

            TempData["message"]=$"{entity.Name} isimli ürün silindi.";
            
            return RedirectToAction("ProductList");
        }

        public IActionResult DeleteCategory(int categoryId)
        {
            var entity = _categoryService.GetById(categoryId);
            
            if(entity != null)
            {
                _categoryService.Delete(entity);
            }

            TempData["message"]=$"{entity.Name} isimli kategori silindi.";
            
            return RedirectToAction("CategoryList");
        }

        [HttpPost]
        public IActionResult DeleteFromCategory(int productId,int categoryId)
        {
            _categoryService.DeleteFromCategory(productId,categoryId);

            return Redirect("/admin/categories/"+categoryId);
        }
    }
}