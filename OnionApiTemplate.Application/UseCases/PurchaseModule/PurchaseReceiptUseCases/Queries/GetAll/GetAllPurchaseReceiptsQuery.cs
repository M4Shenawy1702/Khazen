using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Queries.GetAll
{
    public record GetAllPurchaseReceiptsQuery(PurchaseReceiptsQueryParameters QueryParameters)
        : IRequest<PaginatedResult<PurchaseReceiptDto>>;
}
