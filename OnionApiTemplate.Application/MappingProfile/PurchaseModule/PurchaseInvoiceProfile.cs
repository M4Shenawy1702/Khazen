using Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Mappings.PurchaseModule
{
    public class PurchaseInvoiceProfile : Profile
    {
        public PurchaseInvoiceProfile()
        {
            CreateMap<PurchaseInvoice, PurchaseInvoiceDetailsDto>()
             .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier!.Name));

            CreateMap<PurchaseInvoiceItem, PurchaseInvoiceItemDto>();

            CreateMap<PurchaseInvoice, PurchaseInvoiceDto>()
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier!.Name));
        }
    }
}
