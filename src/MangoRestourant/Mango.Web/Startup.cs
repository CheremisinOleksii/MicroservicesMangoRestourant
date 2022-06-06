using System;

using Mango.Web.Services;
using Mango.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Hosting;

namespace Mango.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient<IProductService, ProductService>();
            services.AddHttpClient<ICartService, CartService>();
            services.AddHttpClient<ICouponService, CouponService>();

            SD.ProductApiBase = Configuration["ServiceUrl:ProductApi"];
            SD.ShopingCartApiBase = Configuration["ServiceUrl:ShopingCartApi"];
            SD.CouponApiBase = Configuration["ServiceUrl:CouponApi"];

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICouponService, CouponService>();

            services.AddControllersWithViews();

            services.AddAuthentication(opt => {
                opt.DefaultScheme = "Cookies";
                opt.DefaultChallengeScheme = "oidc";
            }).AddCookie("Cookies", c => c.ExpireTimeSpan = TimeSpan.FromMinutes(10))
            .AddOpenIdConnect("oidc", opt => {
                opt.Authority = Configuration["ServiceUrl:IdentityApi"];
                opt.GetClaimsFromUserInfoEndpoint = true;
                opt.ClientId = "mango";
                opt.ClientSecret = "secret";
                opt.ResponseType = "code";
                opt.ClaimActions.MapJsonKey("role", "role", "role");
                opt.ClaimActions.MapJsonKey("sub", "sub", "sub");
                opt.TokenValidationParameters.NameClaimType = "name";
                opt.TokenValidationParameters.RoleClaimType = "role";
                opt.Scope.Add("mango");
                opt.SaveTokens = true;

            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
