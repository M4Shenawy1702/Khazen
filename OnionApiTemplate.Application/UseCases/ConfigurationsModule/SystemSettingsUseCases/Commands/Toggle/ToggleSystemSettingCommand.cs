namespace Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Commands.Delete
{
    public record ToggleSystemSettingCommand(string Key) : IRequest<bool>;
}
