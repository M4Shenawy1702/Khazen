namespace Khazen.Application.Common.Interfaces.Authentication
{
    public interface IJwtTokenGenerator
    {
        Task<string> CreateJWTTokenAsync(ApplicationUser user);
    }
}
