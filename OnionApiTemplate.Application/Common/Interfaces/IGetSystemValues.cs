using Khazen.Domain.Entities.ConfigurationModule;

namespace Khazen.Application.Common.Interfaces
{
    public interface IGetSystemValues
    {
        string GetSettingValue(IEnumerable<SystemSetting> systemSettings, string key);
        Guid GetSystemSettingGuid(IEnumerable<SystemSetting> systemSettings, string Key);
    }
}
