using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.CongifurationModule.SystemSettingsDots
{
    public class UpdateSystemSettingDto
    {
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
        public string Description { get; set; } = null!;
        public SystemSettingGroubType GroubType { get; set; }
        public SystemSettingValueType ValueType { get; set; }
    }
}
