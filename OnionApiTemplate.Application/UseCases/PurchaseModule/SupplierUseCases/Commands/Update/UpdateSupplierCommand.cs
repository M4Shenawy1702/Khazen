using Khazen.Application.DOTs.PurchaseModule.SupplierDtos;

namespace Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Commands.Update
{
    public record UpdateSupplierCommand(Guid Id, UpdateSupplierDto Dto, string ModifiedBy) : IRequest<SupplierDto>;
}
