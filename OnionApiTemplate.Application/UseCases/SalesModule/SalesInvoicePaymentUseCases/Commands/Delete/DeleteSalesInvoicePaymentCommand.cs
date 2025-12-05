using Khazen.Application.DOTs.SalesModule.SalesOrderPaymentDtos;

namespace Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Delete
{
    public record DeleteSalesInvoicePaymentCommand(Guid Id, byte[] RowVersion, string DeletedBy) : IRequest<SalesInvoicePaymentDto>;
}
