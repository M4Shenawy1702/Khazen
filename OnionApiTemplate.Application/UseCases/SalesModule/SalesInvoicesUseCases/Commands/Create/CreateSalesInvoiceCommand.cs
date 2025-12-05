using Khazen.Application.DOTs.SalesModule.SalesInvoicesDots;

namespace Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Commands.Create
{
    public record CreateSalesInvoiceCommand(CreateSalesInvoiceDto Dto, string CreatedBy) : IRequest<SalesInvoiceDto>;
}
