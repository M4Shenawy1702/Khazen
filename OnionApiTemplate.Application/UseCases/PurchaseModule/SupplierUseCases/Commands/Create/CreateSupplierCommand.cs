using Khazen.Application.DOTs.PurchaseModule.SupplierDtos;

namespace Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Commands.Create
{
    public record CreateSupplierCommand(CreateSupplierDto Dto, string CurrentUserId) : IRequest<SupplierDto>;
}
