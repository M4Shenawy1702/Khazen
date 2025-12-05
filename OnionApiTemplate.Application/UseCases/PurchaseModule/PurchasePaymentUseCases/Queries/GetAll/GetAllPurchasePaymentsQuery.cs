using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots;

namespace Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Queries.GetAll
{
    public record GetAllPurchasePaymentsQuery(PurchasePaymentQueryParameters QueryParameters) : IRequest<PaginatedResult<PurchasePaymentDto>>;
}
