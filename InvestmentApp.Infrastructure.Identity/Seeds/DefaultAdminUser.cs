using InvestmentApp.Core.Domain.Common.Enums;
using InvestmentApp.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Infrastructure.Identity.Seeds
{
    public static class DefaultAdminUser
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager)
        {
            AppUser user = new()
            {
                Name = "Jane",
                LastName = "Doe",
                Email = "addmin@email.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                UserName = "admin"
            };

            if (await userManager.Users.AllAsync(u => u.Id != user.Id))
            {
                var entityUser = await userManager.FindByEmailAsync(user.Email);
                if(entityUser == null)
                {
                    await userManager.CreateAsync(user, "123Pa$$word!");
                    await userManager.AddToRoleAsync(user, Roles.Admin.ToString());
                }
            }
       
        }
    }
}
