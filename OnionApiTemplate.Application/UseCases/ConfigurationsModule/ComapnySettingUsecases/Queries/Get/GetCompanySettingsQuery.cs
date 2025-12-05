using Khazen.Application.DOTs.CongifurationModule.CompanySetting;

namespace Khazen.Application.UseCases.ConfigurationsModule.ComapnySettingUsecases.Queries.Get
{
    public record GetCompanySettingsQuery() : IRequest<CompanySettingDto>;
}
