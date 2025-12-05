using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.ConfigurationModule;

namespace Khazen.Application.Specification.ConfigurationModule.SystemSettingSpecifications
{
    internal class GetAllSystemSettingsCountSpec
        : BaseSpecifications<SystemSetting>
    {
        public GetAllSystemSettingsCountSpec(SystemSettingsQueryParameters parameters)
            : base(s => (parameters.Group.HasValue || s.Group == parameters.Group))
        {
        }
    }
}
