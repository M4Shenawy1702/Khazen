using Khazen.Application.Common.Interfaces.Authentication;
using System.Security.Cryptography;

namespace Khazen.Infrastructure.Services.Authentication
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        public string Generate()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
    }
}
