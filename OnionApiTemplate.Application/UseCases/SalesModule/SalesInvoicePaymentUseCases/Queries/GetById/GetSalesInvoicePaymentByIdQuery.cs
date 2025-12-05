using Khazen.Application.DOTs.SalesModule.SalesInvoicesDots;

namespace Khazen.Application.UseCases.SalesModule.SalesOrderPaymentUseCases.Queries.GetById
{
    public record GetSalesInvoicePaymentByIdQuery(Guid Id) : IRequest<SalesInvoiceDetailsDto>;
}