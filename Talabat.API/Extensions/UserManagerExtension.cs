using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using Talabat.core.Entities.Identity;

namespace Talabat.API.Extensions
{
    public static class UserManagerExtension
    { 
        public static async Task<AppUser> FindUserWithAddressByEmailAsyc(this UserManager<AppUser> userManager,ClaimsPrincipal User)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var userWithAdress = await userManager.Users.Include(u => u.Address).SingleOrDefaultAsync(u => u.Email == email);
            return userWithAdress;
        }
    }
}
