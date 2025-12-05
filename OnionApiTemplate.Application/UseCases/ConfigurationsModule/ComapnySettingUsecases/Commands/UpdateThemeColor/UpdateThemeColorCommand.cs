using Khazen.Application.DOTs.CongifurationModule.CompanySetting;

namespace Khazen.Application.UseCases.ConfigurationsModule.ComapnySettingUsecases.Commands.UpdateThemeColor
{
    public record UpdateThemeColorCommand(string ThemeColor) : IRequest<CompanySettingDto>;
}
