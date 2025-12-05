using Khazen.Application.UseCases.InventoryModule.BrandUseCases.Commands.Update;

namespace Khazen.Application.Validations.InventoryModule.BrandValidations
{
    public class UpdateBrandCommandValidator :
        AbstractValidator<UpdateBrandCommand>
    {
        public UpdateBrandCommandValidator()
        {
            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Brand name is required.")
                .MaximumLength(100).WithMessage("Brand name must not exceed 100 characters.");
            RuleFor(x => x.Dto.LogoUrl)
                .NotEmpty().WithMessage("Logo URL is required.")
                .Must(BeAValidUrl).WithMessage("Logo URL must be a valid URL.");
            RuleFor(x => x.Dto.WebsiteUrl)
                .NotEmpty().WithMessage("Website URL is required.")
                .Must(BeAValidUrl).WithMessage("Website URL must be a valid URL.");
            RuleFor(x => x.Dto.ContactEmail)
                .NotEmpty().WithMessage("Contact email is required.")
                .EmailAddress().WithMessage("Contact email must be a valid email address.");
        }
        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
