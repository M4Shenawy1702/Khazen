using Khazen.Application.DOTs.PurchaseModule.SupplierDtos;
using Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Commands.Create;
using Khazen.Application.UseCases.PurchaseModule.SupplierUseCases.Commands.Update;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Mappings
{
    public class SupplierProfile : Profile
    {
        public SupplierProfile()
        {
            CreateMap<Supplier, SupplierDto>();
            CreateMap<CreateSupplierCommand, Supplier>();
            CreateMap<UpdateSupplierCommand, Supplier>();
        }
    }
}
