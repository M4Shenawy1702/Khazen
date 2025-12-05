using Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Commands.UpdateByKey;

namespace Khazen.Application.Validations.ConfigurationModule.SysytemSettingValidations
{
    public class UpdateSystemSettingCommandValidator : AbstractValidator<UpdateSystemSettingCommand>
    {
        public UpdateSystemSettingCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0.");

            RuleFor(x => x.Dto.Key)
                .NotEmpty().WithMessage("Key is required.")
                .MaximumLength(100).WithMessage("Key must not exceed 100 characters.");

            RuleFor(x => x.Dto.Value)
                .NotEmpty().WithMessage("Value is required.");

            RuleFor(x => x.Dto.Description)
                .MaximumLength(250).WithMessage("Description must not exceed 250 characters.");

            RuleFor(x => x.Dto.GroubType)
                .IsInEnum().WithMessage("Invalid system setting group type.");

            RuleFor(x => x.Dto.ValueType)
                .IsInEnum().WithMessage("Invalid system setting value type.");
        }
    }
}
