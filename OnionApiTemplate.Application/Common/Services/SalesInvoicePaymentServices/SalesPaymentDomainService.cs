using Khazen.Application.Common.Interfaces.ISalesModule.ISalesInvoicePaymentServices;
using Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Create;
using Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Delete;
using Khazen.Domain.Entities;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.Common.Services.SalesInvoicePaymentServices
{
    internal class SalesPaymentDomainService(ILogger<SalesPaymentDomainService> logger) : ISalesPaymentDomainService
    {
        private readonly ILogger<SalesPaymentDomainService> _logger = logger;

        public void ValidatePaymentAmount(decimal amount, SalesInvoice salesInvoice)
        {
            if (amount <= 0)
                throw new BadRequestException("Payment amount must be greater than zero.");

            var alreadyPaid = salesInvoice.Payments
                .Where(p => !p.IsReversed)
                .Sum(p => p.Amount);

            var remainingAmount = Math.Round(salesInvoice.GrandTotal - alreadyPaid, 2);

            if (amount > remainingAmount)
            {
                _logger.LogWarning(
                    "Payment validation failed. Requested={Amount}, Remaining={Remaining}, Invoice={InvoiceId}",
                    amount,
                    remainingAmount,
                    salesInvoice.Id
                );

                throw new BadRequestException(
                    $"Payment amount exceeds the invoice total. Remaining amount is {remainingAmount}."
                );
            }

            _logger.LogInformation(
                "Payment validated successfully. Amount={Amount}, Remaining={Remaining}, Invoice={InvoiceId}",
                amount,
                remainingAmount,
                salesInvoice.Id
            );
        }

        public SalesInvoicePayment CreatePayment(SalesInvoice salesInvoice, CreateSalesInvoicePaymentCommand command)
        {
            return new SalesInvoicePayment
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = command.CreatedBy ?? "System",
                SalesInvoiceId = salesInvoice.Id,
                Amount = command.Dto.Amount,
                PaymentDate = DateTime.UtcNow,
                Method = command.Dto.Method,
                SafeTransactions = new List<SafeTransaction>(),
                Notes = command.Dto.Notes
            };
        }
        public void ReversePayment(DeleteSalesInvoicePaymentCommand request, SalesInvoicePayment payment)
        {
            if (request.RowVersion != null)
                payment.AssertRowVersion(request.RowVersion);
            else
            {
                _logger.LogWarning("RowVersion not provided for payment {Id}", request.Id);
                throw new BadRequestException("RowVersion not provided.");
            }
            if (payment.IsReversed)
            {
                _logger.LogWarning("Attempt to reverse an already reversed payment {Id}", request.Id);
                throw new BadRequestException("Payment is already reversed/deleted.");
            }

            payment.Reverse();
        }
    }
}
