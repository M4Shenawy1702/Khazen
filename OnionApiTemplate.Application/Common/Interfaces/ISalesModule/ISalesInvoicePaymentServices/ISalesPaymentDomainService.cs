using Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Create;
using Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Delete;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Common.Interfaces.ISalesModule.ISalesInvoicePaymentServices
{
    internal interface ISalesPaymentDomainService
    {
        void ValidatePaymentAmount(decimal Amount, SalesInvoice salesInvoice);
        SalesInvoicePayment CreatePayment(SalesInvoice salesInvoice, CreateSalesInvoicePaymentCommand Command);
        void ReversePayment(DeleteSalesInvoicePaymentCommand request, SalesInvoicePayment payment);
    }
}
