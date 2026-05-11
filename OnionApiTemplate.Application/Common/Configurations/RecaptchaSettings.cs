namespace Khazen.Application.Common.Configurations
{
    namespace Khazen.Infrastructure.Security.Recaptcha
    {
        public class RecaptchaSettings
        {
            public bool Enabled { get; set; } = true;
            public const string SectionName = "Recaptcha";
            public string SecretKey { get; set; } = string.Empty;
            public string SiteKey { get; set; } = string.Empty;
            public string VerificationUrl { get; set; } = string.Empty;
            public float MinimumScore { get; set; }
        }
    }
}
