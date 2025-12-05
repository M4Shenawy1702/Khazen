namespace Khazen.Infrastructure.Common.Setting
{
    public class JWTConfigurations
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double DurationInMinutes { get; set; }

    }
}
