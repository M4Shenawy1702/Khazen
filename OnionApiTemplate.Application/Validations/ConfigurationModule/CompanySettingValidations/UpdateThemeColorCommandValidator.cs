using Khazen.Application.UseCases.ConfigurationsModule.ComapnySettingUsecases.Commands.UpdateThemeColor;

namespace Khazen.Application.Validations.ConfigurationModule.CompanySettingValidations
{
    public class UpdateThemeColorCommandValidator : AbstractValidator<UpdateThemeColorCommand>
    {
        public UpdateThemeColorCommandValidator()
        {
            RuleFor(x => x.ThemeColor)
                .NotEmpty().WithMessage("Theme color is required")
                .Matches(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")
                .WithMessage("Theme color must be a valid hex code (e.g., #0d6efd)");
        }
    }
}
