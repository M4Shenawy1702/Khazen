using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.ConfigurationModule;

namespace Khazen.Application.Specification.ConfigurationModule.SystemSettingSpecifications
{
    public class GetSystemSettingByIdSpec
        : BaseSpecifications<SystemSetting>
    {
        public GetSystemSettingByIdSpec(int Id)
            : base(s => s.Id == Id)
        {
        }
    }
}
