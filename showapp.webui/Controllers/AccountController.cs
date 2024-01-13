using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using showapp.webui.Identity;
using showapp.webui.Models;
using System.Threading.Tasks;
using shopapp.business.Abstract;

namespace showapp.webui.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AccountController: Controller
    {
        private UserManager<User> _userManager;
        
        private SignInManager<User> _signInManager; // cookie olaylarını kontrol edecek
        
        private ICartService _cartService;
        public AccountController(UserManager<User> userManager,SignInManager<User> signInManager,ICartService cartService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _cartService = cartService;
        }

        public IActionResult Login(string ReturnUrl = null)
        {

            return View(new LoginModel()
            {
                ReturnUrl = ReturnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user == null)
            {
                ModelState.AddModelError("","Bu kullanıcı adı bulunamadı.");
                return View(model);
            }

            if(!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("","Lütfen email hesabınıza gelen linkle hesabınızı onaylayınız.");
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(user,model.Password,false,false);
            
            if(result.Succeeded)
            {
                // Başarılı giriş durumunda TempData'ye mesaj ekleyin
                TempData["Message"] = "Oturum açıldı!";
                
                return Redirect(model.ReturnUrl??"~/"); // returnurl yoksa uygulamanın ana sayfasına gider.
            }

            ModelState.AddModelError("","Girilen Kullanıcı adı veya parola yanlış.");
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAsync(RegisterModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user,model.Password);
            if(result.Succeeded)
            {
                //generate token
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);//token db e kaydolur.
                var url = Url.Action("ConfirmEmail","Account",new {
                    userId = user.Id,
                    token = code
                });
                Console.WriteLine(url);
                //email
                
                TempData["Message"] = "Hesap Başarıyla Oluşturuldu";
                
                return RedirectToAction("Login","Account");
            }
            ModelState.AddModelError("","Bilinmeyen bir hata oluştu.Tekrar Deneyiniz.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); //cookie'yi tarayıcıdan siler ve logout olur.
            
            TempData["Message"] = "Oturum başarıyla kapatıldı!";
            return Redirect("~/");
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if(userId == null || token == null)
            {
                TempData["message"] = "Geçersiz Token";
                return View();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if(user != null)
            {

                var result = await _userManager.ConfirmEmailAsync(user,token); // iki verileni karsılastırır eğer varsa confirmemail true olur.
                if(result.Succeeded)
                {
                    _cartService.InitializeCart(user.Id);
        
                    TempData["message"] = "Hesabınız Onaylandı";
                    return View();
                }
            }
            TempData["message"] = "Hesabınız onaylanmadı";
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            if(string.IsNullOrEmpty(Email))
            {
                return View();   
            }
            var user = await _userManager.FindByEmailAsync(Email);

            if(user == null)
            {
                return View();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            
            //generate token
            var url = Url.Action("ResetPassword","Account",new {
                userId = user.Id,
                token = code
            });
            Console.WriteLine(url);
            return View();
        }

        public IActionResult ResetPassword(string userId, string token)
        {   
            if(userId == null || token== null)
            {
                return RedirectToAction("Home","Index");
            }

            var model = new ResetPasswordModel {Token=token};

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if( user == null)
            {
                return RedirectToAction("Home","Index");
            }

            var result = await _userManager.ResetPasswordAsync(user,model.Token,model.Password);
            if(result.Succeeded)
            {
                return RedirectToAction("Login","Account");
            }

            return View(model);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}