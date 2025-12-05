using Khazen.Application.DOTs.PurchaseModule.SupplierDtos;

namespace Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Queries.GetById
{
    public record GetSupplierByIdQuery(Guid Id) : IRequest<SupplierDto>;
}
