using Khazen.Application.DOTs.CongifurationModule.SystemSettingsDots;

namespace Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Commands.Create
{
    public record CreateSystemSettingCommand(CreateSystemSettingDto Dto)
        : IRequest<SystemSettingDto>;
}
