using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Talabat.core.Entities.Identity;
using Talabat.core.IServices;

namespace Talabat.service
{
    public class TokenService :ITokenService
    {
        public IConfiguration Configuration { get; }

        public TokenService(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public async Task<string> CreateToken(AppUser user, UserManager<AppUser> userManager)
        {
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.GivenName,user.DisplayName)
            };
            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]));
            var token = new JwtSecurityToken(
                issuer: Configuration["JWT:validIssure"],
                audience: Configuration["JWT:validAudience"],
                expires: DateTime.Now.AddDays(double.Parse(Configuration["JWT:DurationInDays"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)
                ) ;
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
