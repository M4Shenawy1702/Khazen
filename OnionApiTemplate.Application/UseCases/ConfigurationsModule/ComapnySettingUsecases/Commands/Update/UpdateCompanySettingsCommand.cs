using Khazen.Application.DOTs.CongifurationModule.CompanySetting;

namespace Khazen.Application.UseCases.ConfigurationsModule.ComapnySettingUsecases.Commands.Update
{
    public record UpdateCompanySettingsCommand(UpdateCompanySettingsDto Dto) : IRequest<CompanySettingDto>;
}
