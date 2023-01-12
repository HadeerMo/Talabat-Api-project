using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.core.Entities.Identity;

namespace Talabat.repository.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    DisplayName = "Hadeer mohammed",
                    Email = "Hadeer@gmail.com",
                    UserName = "Hadeer",
                    PhoneNumber = "012863134"
                };
                await userManager.CreateAsync(user,"P@ssw0rd");
            }
        }
    }
}
