using Khazen.Application.DOTs.SalesModule.SalesInvoicesDots;

namespace Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Commands.Post
{
    public record PostSalesInvoiceCommand(Guid Id, string PostedBy) : IRequest<SalesInvoiceDetailsDto>;
}
