using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.CongifurationModule.SystemSettingsDots;

namespace Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Queries.GetAll
{
    public record GetAllSystemSettingsQuery(SystemSettingsQueryParameters Parameters) : IRequest<PaginatedResult<SystemSettingDto>>;
}
