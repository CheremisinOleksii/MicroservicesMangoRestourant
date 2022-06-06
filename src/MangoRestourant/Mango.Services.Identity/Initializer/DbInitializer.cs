using System;
using Microsoft.AspNetCore.Identity;

using Mango.Services.Identity.DbContects;
using Mango.Services.Identity.Models;
using System.Security.Claims;
using IdentityModel;

namespace Mango.Services.Identity.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;


        public DbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public void Initialize()
        {
            if (roleManager.FindByNameAsync(SD.ADMIN).GetAwaiter().GetResult() != null)
                return;
     
            roleManager.CreateAsync(new IdentityRole(SD.ADMIN)).GetAwaiter().GetResult();
            roleManager.CreateAsync(new IdentityRole(SD.CUSTOMER)).GetAwaiter().GetResult();


            var adminUser = new ApplicationUser
            {
                Email = "admin@gmail.com",
                UserName = "admin@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "1111111111",
                FirstName = "Oleksii",
                LastName = "Cheremisin"
            };

            userManager.CreateAsync(adminUser, "Admin123#").GetAwaiter().GetResult();
            userManager.AddToRoleAsync(adminUser, SD.ADMIN).GetAwaiter().GetResult();

           var temp1 =   userManager.AddClaimsAsync(adminUser, new Claim[] {
                    new Claim(JwtClaimTypes.Name, adminUser.FirstName +" "+adminUser.LastName),
                    new Claim(JwtClaimTypes.GivenName, adminUser.FirstName),
                    new Claim(JwtClaimTypes.FamilyName, adminUser.LastName),
                    new Claim(JwtClaimTypes.Role, SD.ADMIN)
            }).GetAwaiter().GetResult();


            var customerUser = new ApplicationUser
            {
                Email = "customer@gmail.com",
                UserName = "customer@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "1111111111",
                FirstName = "Oleksii",
                LastName = "Customer"
            };

            userManager.CreateAsync(customerUser, "Customer123#").GetAwaiter().GetResult();
            userManager.AddToRoleAsync(customerUser, SD.CUSTOMER).GetAwaiter().GetResult();

            var temp2 = userManager.AddClaimsAsync(customerUser, new Claim[] {
                    new Claim(JwtClaimTypes.Name, customerUser.FirstName +" "+customerUser.LastName),
                    new Claim(JwtClaimTypes.GivenName, customerUser.FirstName),
                    new Claim(JwtClaimTypes.FamilyName, customerUser.LastName),
                    new Claim(JwtClaimTypes.Role, SD.CUSTOMER)
            }).GetAwaiter().GetResult();
        }
    }
}
