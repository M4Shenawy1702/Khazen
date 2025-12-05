using Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Confirm;

namespace Khazen.Application.Validations.SalesModule.SalesOrderValidations
{
    public class ConfirmOrderCommandValidator : AbstractValidator<ConfirmOrderCommand>
    {
        public ConfirmOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Order Id is required.");
        }
    }
}
