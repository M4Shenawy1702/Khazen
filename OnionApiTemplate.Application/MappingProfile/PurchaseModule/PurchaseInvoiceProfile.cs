using Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Mappings.PurchaseModule
{
    public class PurchaseInvoiceProfile : Profile
    {
        public PurchaseInvoiceProfile()
        {
            CreateMap<PurchaseInvoice, PurchaseInvoiceDto>().ReverseMap();
            CreateMap<PurchaseInvoice, PurchaseInvoiceDetailsDto>().ReverseMap();

            CreateMap<PurchaseInvoiceItem, PurchaseInvoiceItemDto>().ReverseMap();


        }
    }
}
