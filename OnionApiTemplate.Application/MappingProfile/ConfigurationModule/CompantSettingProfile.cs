using Khazen.Application.DOTs.CongifurationModule.CompanySetting;
using Khazen.Application.UseCases.ConfigurationsModule.ComapnySettingUsecases.Commands.Update;
using Khazen.Domain.Entities.ConfigurationModule;

namespace Khazen.Application.MappingProfile.ConfigurationModule
{
    public class CompantSettingProfile : Profile
    {
        public CompantSettingProfile()
        {
            CreateMap<CompanySetting, CompanySettingDto>();

            CreateMap<UpdateCompanySettingsCommand, CompanySetting>();
        }
    }
}

