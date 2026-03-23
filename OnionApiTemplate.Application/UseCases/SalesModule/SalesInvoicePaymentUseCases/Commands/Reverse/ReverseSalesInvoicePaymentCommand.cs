using Khazen.Application.DOTs.SalesModule.SalesOrderPaymentDtos;

namespace Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Delete
{
    public record ReverseSalesInvoicePaymentCommand(Guid Id, byte[] RowVersion, string CurrentUserId) : IRequest<SalesInvoicePaymentDto>;
}
