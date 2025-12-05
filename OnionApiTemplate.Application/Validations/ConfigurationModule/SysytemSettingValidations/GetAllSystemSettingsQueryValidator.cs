using Khazen.Application.UseCases.ConfigurationsModule.SystemSettingsUseCases.Queries.GetAll;

namespace Khazen.Application.Validations.ConfigurationModule.SysytemSettingValidations
{
    public class GetAllSystemSettingsQueryValidator
        : AbstractValidator<GetAllSystemSettingsQuery>
    {
        public GetAllSystemSettingsQueryValidator()
        {
            RuleFor(x => x.Parameters.PageIndex)
                .GreaterThan(0)
                .WithMessage("PageIndex must be greater than 0.");

            RuleFor(x => x.Parameters.PageSize)
                .GreaterThan(0).WithMessage("PageSize must be greater than 0.")
                .LessThanOrEqualTo(50).WithMessage("PageSize cannot exceed 50.");
        }
    }
}
