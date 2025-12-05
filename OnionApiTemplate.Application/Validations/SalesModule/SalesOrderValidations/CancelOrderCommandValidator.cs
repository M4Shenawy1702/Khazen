using Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Cancel;

namespace Khazen.Application.Validations.SalesModule.SalesOrderValidations
{
    public class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Order Id is required.");

            RuleFor(x => x.RowVersion)
                .NotNull()
                .WithMessage("RowVersion is required for optimistic concurrency.")
                .Must(v => v.Length > 0)
                .WithMessage("RowVersion cannot be empty.");

            RuleFor(x => x.CanceledBy)
                .NotEmpty()
                .WithMessage("CanceledBy is required.")
                .MaximumLength(100)
                .WithMessage("CanceledBy cannot exceed 100 characters.");
        }
    }
}
