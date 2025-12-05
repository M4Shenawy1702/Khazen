using Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Queries.GetById;

namespace Khazen.Application.Validations.PurchaseModule.PurchaseOrderValidations
{
    public class GetPurchaseOrderByIdQueryValidator : AbstractValidator<GetPurchaseOrderByIdQuery>
    {
        public GetPurchaseOrderByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("PurchaseOrder Id is required.");
        }
    }
}
