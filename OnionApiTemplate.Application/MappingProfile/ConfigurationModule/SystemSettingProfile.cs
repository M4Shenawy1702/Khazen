using Khazen.Application.DOTs.CongifurationModule.SystemSettingsDots;
using Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Commands.Create;
using Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Commands.UpdateByKey;
using Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Commands.UpdateValueByKey;
using Khazen.Domain.Entities.ConfigurationModule;

namespace Khazen.Application.MappingProfile.ConfigurationModule
{
    public class SystemSettingProfile : Profile
    {
        public SystemSettingProfile()
        {
            CreateMap<SystemSetting, SystemSettingDto>();
            CreateMap<CreateSystemSettingCommand, SystemSetting>();
            CreateMap<UpdateSystemSettingCommand, SystemSetting>();
            CreateMap<UpdateSystemSettingValueCommand, SystemSetting>();
        }
    }
}
