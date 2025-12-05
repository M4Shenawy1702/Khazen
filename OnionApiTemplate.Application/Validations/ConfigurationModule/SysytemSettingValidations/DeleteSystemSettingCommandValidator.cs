using Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Commands.Delete;

namespace Khazen.Application.Validations.ConfigurationModule.SysytemSettingValidations
{
    public class DeleteSystemSettingCommandValidator : AbstractValidator<ToggleSystemSettingCommand>
    {
        public DeleteSystemSettingCommandValidator()
        {
            RuleFor(x => x.Key)
                .NotEmpty().WithMessage("SystemSetting Key is required.")
                .MaximumLength(100).WithMessage("SystemSetting Key cannot exceed 100 characters.");
        }
    }
}
