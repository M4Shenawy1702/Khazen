using Khazen.Application.BaseSpecifications;
using Khazen.Application.Common.QueryParameters;
using Khazen.Domain.Entities.ConfigurationModule;

internal class GetAllSystemSettingsSpec
    : BaseSpecifications<SystemSetting>
{
    public GetAllSystemSettingsSpec(SystemSettingsQueryParameters parameters)
        : base(s => !parameters.Group.HasValue || s.Group == parameters.Group.Value)
    {
        ApplyPagination(parameters.PageSize, parameters.PageIndex);
    }
    public GetAllSystemSettingsSpec()
    : base(s => true)
    {

    }
}
