using Khazen.Application.UseCases.ConfigurationsModule.ComapnySettingUsecases.Commands.Update;

namespace Khazen.Application.Validations.ConfigurationModule.CompanySettingValidations
{
    public class UpdateCompanySettingsCommandValidator : AbstractValidator<UpdateCompanySettingsCommand>
    {
        public UpdateCompanySettingsCommandValidator()
        {
            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Company name is required")
                .MaximumLength(200).WithMessage("Company name must not exceed 200 characters");

            RuleFor(x => x.Dto.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(500).WithMessage("Address must not exceed 500 characters");

            RuleFor(x => x.Dto.DomainName)
                .NotEmpty().WithMessage("Domain name is required")
                .Matches(@"^[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
                .WithMessage("Invalid domain format");

            RuleFor(x => x.Dto.Phone)
                .NotEmpty().WithMessage("Phone is required")
                .Matches(@"^\+?[0-9]{7,15}$")
                .WithMessage("Invalid phone number format");

            RuleFor(x => x.Dto.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address");

            RuleFor(x => x.Dto.LogoUrl)
                .MaximumLength(500).WithMessage("Logo URL must not exceed 500 characters");

            RuleFor(x => x.Dto.DefaultLanguage)
                .NotEmpty().WithMessage("Default language is required")
                .Matches(@"^[a-z]{2}-[A-Z]{2}$").WithMessage("Language must be in format like en-US");

            RuleFor(x => x.Dto.CurrencyCode)
                .NotEmpty().WithMessage("Currency code is required")
                .Length(3).WithMessage("Currency code must be a valid ISO code (3 letters)");

            RuleFor(x => x.Dto.CurrencySymbol)
                .NotEmpty().WithMessage("Currency symbol is required")
                .MaximumLength(5).WithMessage("Currency symbol must not exceed 5 characters");

            RuleFor(x => x.Dto.ThemeColor)
                .NotEmpty().WithMessage("Theme color is required")
                .Matches(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")
                .WithMessage("Theme color must be a valid hex code");

            RuleFor(x => x.Dto.DefaultTaxRate)
                .GreaterThanOrEqualTo(0).WithMessage("Default tax rate cannot be negative")
                .LessThanOrEqualTo(1).WithMessage("Default tax rate should be between 0 and 1 (e.g., 0.15 for 15%)");

            RuleFor(x => x.Dto.FiscalYearStart)
                .NotEmpty().WithMessage("Fiscal year start is required");
        }
    }
}
