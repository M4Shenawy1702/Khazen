namespace Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Create
{
    public class CreateSalesOrderCommandValidator : AbstractValidator<CreateSalesOrderCommand>
    {
        public CreateSalesOrderCommandValidator()
        {
            RuleFor(x => x.Dto.CustomerId)
                .NotEmpty().WithMessage("CustomerId is required.");

            RuleFor(x => x.Dto.EstimatedShipDate)
                .GreaterThan(DateTime.UtcNow)
                .When(x => x.Dto.EstimatedShipDate.HasValue)
                .WithMessage("Estimated ship date must be in the future.");

            RuleFor(x => x.Dto.SalesOrderItems)
                .NotNull().WithMessage("At least one sales order item is required.")
                .Must(items => items.Any())
                .WithMessage("At least one sales order item is required.");

            RuleForEach(x => x.Dto.SalesOrderItems)
                .ChildRules(items =>
                {
                    items.RuleFor(i => i.ProductId)
                        .NotEmpty().WithMessage("ProductId is required.");

                    items.RuleFor(i => i.Quantity)
                        .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

                    items.RuleFor(i => i.DiscountType)
                        .IsInEnum().WithMessage("Discount type is invalid.");

                    items.RuleFor(i => i.DiscountValue)
                        .GreaterThanOrEqualTo(0).WithMessage("Discount must be greater than or equal to 0.");
                });
        }
    }
}
