using Khazen.Application.Common.Interfaces.Authentication;
using Khazen.Domain.Entities.UsersModule;
using Khazen.Infrastructure.Common.Setting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Khazen.Infrastructure.Services.Authentication
{
    public class JwtTokenGenerator(IOptions<JWTConfigurations> jwtOptions, UserManager<ApplicationUser> userManager)
        : IJwtTokenGenerator
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly JWTConfigurations _jwtOptions = jwtOptions.Value;
        public async Task<string> CreateJWTTokenAsync(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }
            .Union(userClaims)
            .Union(userRoles.Select(role => new Claim("role", role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key!));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.DurationInMinutes),
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
