using Khazen.Application.DOTs.Auth;
using System.Text.Json.Serialization;

namespace Khazen.Application.UseCases.AuthenticationModule.Common
{
    public record AuthResponse
    {
        public AuthResponse(ApplicationUserDto userDto, string token, string refreshToken, DateTime refreshTokenExpiration)
        {
            UserDto = userDto;
            Token = token;
            RefreshToken = refreshToken;
            RefreshTokenExpiration = refreshTokenExpiration;
        }
        public ApplicationUserDto UserDto { get; init; }
        public string Token { get; init; }
        [JsonIgnore]
        public string RefreshToken { get; init; }
        public DateTime RefreshTokenExpiration { get; init; }
    }
}
