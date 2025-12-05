using Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Create;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Entities.ConfigurationModule;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Common.Interfaces
{
    public interface IJournalEntryService
    {
        Task<JournalEntry> CreateSalaryJournalEntryAsync(Employee employee, Salary salary, string createdBy, CancellationToken cancellationToken = default);
        Task<JournalEntry> GeneratePurchaseJournalEntryAsync(PurchaseInvoice invoice, CancellationToken cancellationToken);
        Task<JournalEntry> GenerateReversalPurchaseInvoiceAsync(PurchaseInvoice invoice, CancellationToken cancellationToken);
        Task<JournalEntry> CreatePurchasePaymentEntryAsync(PurchaseInvoice invoice, decimal amount, PaymentMethod method, CancellationToken cancellationToken);
        Task<JournalEntry> CreatePrchasePaymentReversalJournalAsync(PurchasePayment payment, PurchaseInvoice invoice, CancellationToken cancellationToken);
        Task CreateSalesInvoiceJournalAsync(SalesInvoice salesInvoice, CancellationToken cancellationToken);
        Task<JournalEntry> CreateSalesInvoicePaymentJournalAsync(SalesInvoice salesInvoice, CreateSalesInvoicePaymentCommand request, IEnumerable<SystemSetting> systemSettings, CancellationToken cancellationToken);
        Task<JournalEntry> CreateReverseSalesPaymenttJournalEntry(SalesInvoicePayment payment, CancellationToken cancellationToken);
    }
}
