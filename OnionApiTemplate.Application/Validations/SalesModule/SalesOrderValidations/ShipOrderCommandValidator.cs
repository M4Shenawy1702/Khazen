using Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Ship;

namespace Khazen.Application.Validations.SalesModule.SalesOrderValidations
{
    public class ShipOrderCommandValidator : AbstractValidator<ShipOrderCommand>
    {
        public ShipOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Order Id is required.");

            RuleFor(x => x.Dto.TrackingNumber)
                .NotEmpty().WithMessage("Tracking number is required.")
                .MaximumLength(100).WithMessage("Tracking number cannot exceed 100 characters.");
        }
    }
}
