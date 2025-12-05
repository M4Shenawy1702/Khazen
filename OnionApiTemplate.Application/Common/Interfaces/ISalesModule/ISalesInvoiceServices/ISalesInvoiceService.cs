using Khazen.Application.DOTs.SalesModule.SalesInvoicesDots;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Common.Interfaces.ISalesModule.ISalesInvoiceServices
{
    internal interface ISalesInvoiceService
    {
        Task<SalesInvoice> CreateSalesInvoice(SalesOrder salesOrder, Customer customer, CreateSalesInvoiceDto dto, string CreatedBy, CancellationToken cancellationToken);
    }
}
