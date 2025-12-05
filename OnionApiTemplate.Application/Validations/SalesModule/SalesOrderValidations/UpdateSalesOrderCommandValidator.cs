using Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Commands.Update;
using Khazen.Domain.Common.Enums;

public class UpdateSalesOrderCommandValidator : AbstractValidator<UpdateSalesOrderCommand>
{
    public UpdateSalesOrderCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Order Id is required.");

        RuleFor(x => x.Dto)
            .NotNull().WithMessage("Order details are required.");

        RuleFor(x => x.Dto.CustomerId)
            .NotEmpty().WithMessage("Customer Id is required.");

        RuleFor(x => x.ModifiedBy)
            .NotEmpty().WithMessage("ModifiedBy is required.");

        RuleFor(x => x.RowVersion)
            .NotEmpty().WithMessage("RowVersion is required.");

        RuleFor(x => x.Dto.SalesOrderItems)
           .NotEmpty().WithMessage("Order must contain at least one item.");

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

        RuleFor(x => x.Dto)
            .Must(order =>
            {
                if (order.DiscountType == DiscountType.Percentage && order.DiscountValue > 100)
                    return false;
                return true;
            })
            .WithMessage("Percentage discount cannot exceed 100%.");

        RuleFor(x => x.Dto.EstimatedShipDate)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.Dto.EstimatedShipDate.HasValue)
            .WithMessage("Estimated ship date must be in the future.");
    }
}
