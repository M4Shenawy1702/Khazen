using Khazen.Application.DOTs.CongifurationModule.SystemSettingsDots;

namespace Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Commands.UpdateByKey
{
    public record UpdateSystemSettingCommand(int Id, UpdateSystemSettingDto Dto) : IRequest<SystemSettingDto>;
}
