using Khazen.Application.Common.Configurations.Khazen.Infrastructure.Security.Recaptcha;
using Khazen.Application.Common.Interfaces.Authentication;
using Khazen.Application.Common.ObjectValues;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Khazen.Application.Common.Services.AuthenticationServices
{
    public class RecaptchaService(HttpClient http, IConfiguration config, IOptions<RecaptchaSettings> recaptchaOptions, ILogger<RecaptchaService> logger) : IRecaptchaService
    {
        private readonly HttpClient _http = http;
        private readonly IConfiguration _config = config;
        private readonly ILogger<RecaptchaService> _logger = logger;
        private readonly RecaptchaSettings _recaptchaOptions = recaptchaOptions.Value;

        public async Task<RecaptchaResponse> VerifyAsync(string token, string email)
        {
            var response = await _http.PostAsync(
                $"{_recaptchaOptions.VerificationUrl}?secret={_recaptchaOptions.SecretKey}&response={token}",
                null);

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<RecaptchaResponse>(json);

            if (result is null)
                throw new BadRequestException("Security validation failed. Please try again.");

            if (result.Score < _recaptchaOptions.MinimumScore)
            {
                _logger.LogWarning("Recaptcha failed for email {Email}. Score: {Score}",
                    email, result.Score);
                throw new BadRequestException("Security validation failed. Please try again.");
            }

            return result ?? new RecaptchaResponse();
        }
    }
}
