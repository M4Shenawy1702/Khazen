using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.ConfigurationModule;

namespace Khazen.Application.Specification.ConfigurationModule.SystemSettingSpecifications
{
    public class GetSystemSettingByKeySpec : BaseSpecifications<SystemSetting>
    {
        public GetSystemSettingByKeySpec(string key)
            : base(s => s.Key == key)
        {
        }
    }
}
