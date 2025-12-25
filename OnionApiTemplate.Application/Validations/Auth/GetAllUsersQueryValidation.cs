using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Queries.GetAll;

namespace Khazen.Application.Validations.Auth
{
    public class GetAllUsersQueryValidation : AbstractValidator<GetAllUsersQuery>
    {
        private const int MaxPageSize = 100;

        public GetAllUsersQueryValidation()
        {

            RuleFor(x => x.Parameters.PageIndex)
                .GreaterThan(0)
                    .WithErrorCode("PAGINATION_001")
                    .WithMessage("PageIndex must be a positive integer.");

            RuleFor(x => x.Parameters.PageSize)
                .InclusiveBetween(1, MaxPageSize)
                    .WithErrorCode("PAGINATION_002")
                    .WithMessage($"PageSize must be between 1 and {MaxPageSize}.");

            RuleFor(x => x.Parameters.UserType)
                .Must(BeValidUserType)
                    .When(x => x.Parameters.UserType.HasValue)
                    .WithErrorCode("QUERY_USER_001")
                    .WithMessage("Invalid UserType specified.");
        }

        private bool BeValidUserType(UserType? userType)
        {
            if (!userType.HasValue)
            {
                return true;
            }

            return Enum.IsDefined(typeof(UserType), userType.Value);
        }
    }
}