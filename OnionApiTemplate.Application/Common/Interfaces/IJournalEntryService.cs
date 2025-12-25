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
        Task<JournalEntry> GeneratePurchaseJournalEntryAsync(PurchaseInvoice invoice, string CreatedBy, CancellationToken cancellationToken);
        Task<JournalEntry> GenerateReversalPurchaseInvoiceAsync(PurchaseInvoice invoice, string reversedBy, CancellationToken cancellationToken);
        Task<JournalEntry> CreatePurchasePaymentEntryAsync(PurchaseInvoice invoice, string CreatedBy, decimal amount, PaymentMethod method, CancellationToken cancellationToken);
        Task<JournalEntry> CreatePurchasePaymentReversalJournalAsync(PurchasePayment payment, PurchaseInvoice invoice, string reversedBy, CancellationToken cancellationToken);
        Task CreateSalesInvoiceJournalAsync(SalesInvoice salesInvoice, string CreatedBy, CancellationToken cancellationToken);
        Task<JournalEntry> CreateSalesInvoicePaymentJournalAsync(SalesInvoice salesInvoice, string CreatedBy, CreateSalesInvoicePaymentCommand request, IEnumerable<SystemSetting> systemSettings, CancellationToken cancellationToken);
        Task<JournalEntry> CreateReverseSalesPaymentJournalEntry(SalesInvoicePayment payment, string reversedBy, CancellationToken cancellationToken);
        void UpdateBalances(IEnumerable<Account> accounts, JournalEntry entry);
        void ReverseUpdateBalances(IEnumerable<Account> accounts, JournalEntry reversedEntry);
    }
}
