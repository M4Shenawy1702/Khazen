using Khazen.Application.DOTs.SalesModule.SalesInvoicesDots;

namespace Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Queries.GetById
{
    public record GetSalesInvoiceByIdQuery(Guid Id) : IRequest<SalesInvoiceDetailsDto>;
}
