using Khazen.Application.UseCases.SalesModule.CustomerUsecases.Commands.Delete;

namespace Khazen.Application.Validations.SalesModule.CustomerValidations
{
    public class DeleteCustomerCommandValidator : AbstractValidator<ToggleCustomerCommand>
    {
        public DeleteCustomerCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Customer Id is required.");
            RuleFor(x => x.ModifiedBy)
                .NotEmpty().WithMessage("ModifiedBy is required.");
        }
    }
}
