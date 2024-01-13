using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using shopapp.data.Abstract;
using shopapp.data.Concreate.EfCore;
using shopapp.business.Abstract;
using shopapp.business.Concreate;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using showapp.webui.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; // Bu satırı ekleyin

namespace shopapp.webui
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //kullanıcı sayfaları için
            services.AddDbContext<ApplicationContext>(options=> options.UseSqlite("Data Source=shopDb"));
            services.AddIdentity<User,IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options=> {
                
                //password
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = true;

                //Lockout: Kullanıcı hesabının kitlenmesi
                options.Lockout.MaxFailedAccessAttempts = 5;
                // kullanıcı 3 dk sonra tekrar giris yapabilir.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                options.Lockout.AllowedForNewUsers = true; //Lockoutun aktif olması

                //user
                //options.User.AllowedUserNameCharacters = "";
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            });

            //cookie ayarlari
            services.ConfigureApplicationCookie(options=> {
                options.LoginPath = "/account/login";
                options.LogoutPath = "/account/logout";
                options.AccessDeniedPath = "/account/accessdenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.Cookie = new CookieBuilder
                {
                    HttpOnly = true, //cookie yi sadece http ile ulasabiliriz.
                    Name =".ShopApp.Security.Cookie",
                    SameSite = SameSiteMode.Strict
                };

            });

            services.AddScoped<ICategoryRepository, EfCoreCategoryRepository>();
            services.AddScoped<IProductRepository, EfCoreProductRepository>();
            services.AddScoped<ICartRepository, EfCoreCartRepository>();


            services.AddScoped<IProductService, ProductManager>();
            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<ICartService, CartManager>();

            //mvc ve razor pages
            services.AddControllersWithViews(); /* view ile controller kullanilacak */
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {   
            app.UseStaticFiles(); // wwwroot

            app.UseStaticFiles(new StaticFileOptions{
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(),"node_modules")),
                    RequestPath = "/modules"
            });

            if (env.IsDevelopment()) // ortam degiskenlerine bakıyor
            {
                SeedDatabase.Seed();
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication(); // surec icerisine bir servis yapısını eklemiş oluyoruz.
            app.UseRouting();
            app.UseAuthorization();
            //localhost:5256/home
            //localhost:5256/home/products
            //localhost:5256/home/index/2

            app.UseEndpoints(endpoints =>
            {
                //asagidakilerin sirasi onemlidir defaulttan yukarıda olmalılar
                endpoints.MapControllerRoute(
                    name:"checkout",
                    pattern: "checkout",
                    defaults: new {controller ="Cart",action="Checkout"}
                );
                
                endpoints.MapControllerRoute(
                    name:"cart",
                    pattern: "cart",
                    defaults: new {controller ="Cart",action="Index"}
                );
                
                endpoints.MapControllerRoute(
                    name:"adminuseredit",
                    pattern: "admin/user/{id?}",
                    defaults: new {controller ="Admin",action="UserEdit"}
                );
                
                endpoints.MapControllerRoute(
                    name:"adminusers",
                    pattern: "admin/user/list",
                    defaults: new {controller ="Admin",action="UserList"}
                );

                endpoints.MapControllerRoute(
                    name:"adminroles",
                    pattern: "admin/role/list",
                    defaults: new {controller ="Admin",action="RoleList"}
                );
                
                
                endpoints.MapControllerRoute(
                    name:"adminrolecreate",
                    pattern: "admin/role/create",
                    defaults: new {controller ="Admin",action="RoleCreate"}
                );

                endpoints.MapControllerRoute(
                    name:"adminroleedit",
                    pattern: "admin/role/{id?}",
                    defaults: new {controller ="Admin",action="RoleEdit"}
                );

                endpoints.MapControllerRoute(
                    name:"adminproducts",
                    pattern: "admin/products",
                    defaults: new {controller ="Admin",action="ProductList"}
                );

                endpoints.MapControllerRoute(
                    name:"adminproductcreate",
                    pattern: "admin/products/create",
                    defaults: new {controller ="Admin",action="ProductCreate"}
                );

                endpoints.MapControllerRoute(
                    name:"adminproductedit",
                    pattern: "admin/products/{id?}",
                    defaults: new {controller ="Admin",action="ProductEdit"}
                );

                endpoints.MapControllerRoute(
                    name:"admincategories",
                    pattern: "admin/categories",
                    defaults: new {controller ="Admin",action="CategoryList"}
                );
                
                endpoints.MapControllerRoute(
                    name:"admincategorycreate",
                    pattern: "admin/categories/create",
                    defaults: new {controller ="Admin",action="CategoryCreate"}
                );

                endpoints.MapControllerRoute(
                    name:"admincategoryedit",
                    pattern: "admin/categories/{id?}",
                    defaults: new {controller ="Admin",action="CategoryEdit"}
                );

                endpoints.MapControllerRoute(
                    name:"search",
                    pattern: "search",
                    defaults: new {controller ="Shop",action="search"}
                );
                
                endpoints.MapControllerRoute(
                    name:"productdetails",
                    pattern: "{url}",
                    defaults: new {controller ="Shop",action="details"}
                );                

                
                endpoints.MapControllerRoute(
                    name:"products",
                    pattern: "products/{category?}",
                    defaults: new {controller ="Shop",action="list"}
                );


                endpoints.MapControllerRoute(
                    name:"default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );

                
            });
        }
    }
}
