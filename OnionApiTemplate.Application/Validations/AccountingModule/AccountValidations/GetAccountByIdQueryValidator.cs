using Khazen.Application.UseCases.AccountingModule.AccountUseCases.Queries.GetById;

namespace Khazen.Application.Validations.AccountingModule.AccountValidations
{
    public class GetAccountByIdQueryValidator : AbstractValidator<GetAccountByIdQuery>
    {
        public GetAccountByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Account Id is required.");
        }
    }
}
