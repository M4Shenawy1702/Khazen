using Khazen.Domain.Common.Enums;

namespace Khazen.Domain.Entities.ConfigurationModule
{
    public class SystemSetting : BaseEntity<int>
    {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public SystemSettingGroubType Group { get; set; } = SystemSettingGroubType.General;
        public SystemSettingValueType ValueType { get; set; } = SystemSettingValueType.String;
    }

}
