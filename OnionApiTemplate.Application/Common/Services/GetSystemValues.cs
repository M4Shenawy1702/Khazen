using Khazen.Application.Common.Interfaces;
using Khazen.Domain.Entities.ConfigurationModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.Common.Services
{
    internal class GetSystemValues(ILogger<GetSystemValues> logger) : IGetSystemValues
    {
        private readonly ILogger<GetSystemValues> _logger = logger;

        public string GetSettingValue(IEnumerable<SystemSetting> systemSettings, string key)
        {
            var setting = systemSettings.FirstOrDefault(x => x.Key == key);
            if (setting == null)
            {
                _logger.LogError($"System Setting with key: {key} not found");
                throw new NotFoundException<SystemSetting>($"System Setting with key: {key} not found");
            }

            return setting.Value;
        }
        public Guid GetSystemSettingGuid(IEnumerable<SystemSetting> systemSettings, string Key)
        {
            var setting = systemSettings.FirstOrDefault(x => x.Key == Key);
            if (setting == null)
            {
                _logger.LogError($"System Setting with key: {Key} not found");
                throw new NotFoundException<SystemSetting>($"System Setting with key: {Key} not found");
            }
            if (Guid.TryParse(setting.Value, out Guid value))
                return value;
            throw new DomainException($"Missing required system setting: {Key}");
        }
    }
}
