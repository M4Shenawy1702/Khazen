using Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Commands.UpdateValueByKey;

namespace Khazen.Application.Validations.ConfigurationModule.SysytemSettingValidations
{
    public class UpdateSystemSettingValueCommandValidator : AbstractValidator<UpdateSystemSettingValueCommand>
    {
        public UpdateSystemSettingValueCommandValidator()
        {
            RuleFor(x => x.Key)
                .NotEmpty().WithMessage("Key is required.")
                .MaximumLength(100).WithMessage("Key must not exceed 100 characters.");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("Value is required.");

            RuleFor(x => x.ValueType)
                .IsInEnum().WithMessage("Invalid system setting value type.");
        }
    }
}
