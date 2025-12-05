using Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Deliverd;

namespace Khazen.Application.Validations.SalesModule.SalesOrderValidations
{
    public class DeliverOrderCommandValidator : AbstractValidator<DeliverOrderCommand>
    {
        public DeliverOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Order Id is required.");
        }
    }
}
