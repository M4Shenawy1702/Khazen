using Khazen.Application.DOTs.SalesModule.SalesInvoicesDots;

namespace Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Commands.Update
{
    public record UpdateSalesInvoiceCommand(Guid Id, UpdateSalesInvoiceDto Dto, byte[] RowVersion, string CurrentUserId) : IRequest<SalesInvoiceDto>;
}
