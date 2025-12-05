using Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Commands.Delete;

namespace Khazen.Application.Validations.PurchaseModule.PurchaseOrderValidations
{
    public class DeletePurchaseOrderCommandValidator : AbstractValidator<TogglePurchaseOrderCommand>
    {
        public DeletePurchaseOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("PurchaseOrder Id is required.")
                .NotEqual(Guid.Empty).WithMessage("PurchaseOrder Id cannot be empty GUID.");
        }
    }
}
