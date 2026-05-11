using Microsoft.Extensions.Configuration;

namespace Khazen.Application.Common.Configurations
{
    public static class SystemSettings
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string GetValue(string key, string defaultValue = "")
        {
            if (_configuration == null)
                return defaultValue;

            return _configuration[key] ?? defaultValue;
        }

        public static T GetValue<T>(string key, T defaultValue = default)
        {
            if (_configuration == null)
                return defaultValue;

            var value = _configuration[key];
            if (value == null)
                return defaultValue;

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        public static T GetSection<T>(string sectionName) where T : new()
        {
            if (_configuration == null)
                return new T();

            return _configuration.GetSection(sectionName).Get<T>() ?? new T();
        }
    }
}
