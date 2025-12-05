using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDtss;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseOrderUseCases.Queries.GetAll
{
    public record GetAllPurchaseOrdersQuery(PurchaseOrdersQueryParameters QueryParameters) : IRequest<PaginatedResult<PurchaseOrderDto>>;
}
