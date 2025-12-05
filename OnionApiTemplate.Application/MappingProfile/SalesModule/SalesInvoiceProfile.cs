using Khazen.Application.DOTs.SalesModule.SalesInvoicesDots;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.MappingProfile.SalesModule
{
    public class SalesInvoiceProfile : Profile
    {
        public SalesInvoiceProfile()
        {
            CreateMap<SalesInvoice, SalesInvoiceDto>();

            CreateMap<SalesInvoice, SalesInvoiceDetailsDto>();

        }
    }
}
