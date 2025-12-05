using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.CongifurationModule.SystemSettingsDots
{
    public class SystemSettingDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public SystemSettingGroubType Group { get; set; } = SystemSettingGroubType.General;
        public SystemSettingValueType ValueType { get; set; } = SystemSettingValueType.String;
    }
}
