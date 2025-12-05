using Khazen.Application.DOTs.PurchaseModule.SupplierDtos;

namespace Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Queries.GetAll
{
    public record GetAllSuppliersQuery() : IRequest<List<SupplierDto>>;
}
