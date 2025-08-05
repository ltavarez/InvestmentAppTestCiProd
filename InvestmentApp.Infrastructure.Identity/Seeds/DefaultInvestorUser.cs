using InvestmentApp.Core.Domain.Common.Enums;
using InvestmentApp.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Infrastructure.Identity.Seeds
{
    public static class DefaultInvestorUser
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager)
        {
            AppUser user = new()
            {
                Name = "John",
                LastName = "Doe",
                Email = "investor@email.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                UserName = "basic_investor"
            };

            if (await userManager.Users.AllAsync(u => u.Id != user.Id))
            {
                var entityUser = await userManager.FindByEmailAsync(user.Email);
                if(entityUser == null)
                {
                    await userManager.CreateAsync(user, "123Pa$$word!");
                    await userManager.AddToRoleAsync(user, Roles.Investor.ToString());
                }
            }
       
        }
    }
}
