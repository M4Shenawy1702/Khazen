using Khazen.Application.DOTs.SalesModule.SalesOrderPaymentDtos;

namespace Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Create
{
    public record CreateSalesInvoicePaymentCommand(CreateSalesInvoicePaymentDto Dto, string CreatedBy) : IRequest<SalesInvoicePaymentDto>;
}
