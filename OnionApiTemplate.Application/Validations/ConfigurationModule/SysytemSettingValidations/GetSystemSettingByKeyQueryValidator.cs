using Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Queries.GetByKey;

namespace Khazen.Application.Validations.ConfigurationModule.SysytemSettingValidations
{
    public class GetSystemSettingByKeyQueryValidator : AbstractValidator<GetSystemSettingByKeyQuery>
    {
        public GetSystemSettingByKeyQueryValidator()
        {
            RuleFor(x => x.Key).NotEmpty().NotNull().WithMessage("Key is required");
        }
    }
}
