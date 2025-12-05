using Khazen.Application.DOTs.SalesModule.SalesOrderPaymentDtos;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.MappingProfile.SalesModule
{
    public class SalesInvoicePaymentProfile : Profile
    {
        public SalesInvoicePaymentProfile()
        {
            CreateMap<CreateSalesInvoicePaymentDto, SalesInvoicePayment>();
            CreateMap<UpdateSalesInvoicePaymentDto, SalesInvoicePayment>();
            CreateMap<SalesInvoicePayment, SalesInvoicePaymentDto>();

        }
    }
}
