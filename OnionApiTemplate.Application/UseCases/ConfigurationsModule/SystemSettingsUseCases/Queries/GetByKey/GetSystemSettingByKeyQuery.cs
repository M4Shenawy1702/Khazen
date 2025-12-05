using Khazen.Application.DOTs.CongifurationModule.SystemSettingsDots;

namespace Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Queries.GetByKey
{
    public record GetSystemSettingByKeyQuery(string Key) : IRequest<SystemSettingDto>;
}
