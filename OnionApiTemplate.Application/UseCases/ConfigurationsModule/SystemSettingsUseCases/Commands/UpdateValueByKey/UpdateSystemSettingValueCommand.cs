using Khazen.Application.DOTs.CongifurationModule.SystemSettingsDots;
using Khazen.Domain.Common.Enums;

namespace Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Commands.UpdateValueByKey
{
    public record UpdateSystemSettingValueCommand(string Key, string Value, SystemSettingValueType ValueType)
        : IRequest<SystemSettingDto>;
}
