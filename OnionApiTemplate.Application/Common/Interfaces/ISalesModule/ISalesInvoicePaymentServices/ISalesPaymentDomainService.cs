using Khazen.Application.DOTs.SalesModule.SalesOrderPaymentDtos;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Common.Interfaces.ISalesModule.ISalesInvoicePaymentServices
{
    internal interface ISalesPaymentDomainService
    {
        void ValidatePaymentAmount(decimal Amount, SalesInvoice salesInvoice);
        SalesInvoicePayment CreatePayment(SalesInvoice salesInvoice, CreateSalesInvoicePaymentDto Dto, string userId);
    }
}
