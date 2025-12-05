using Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Create;
using Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Delete;
using Khazen.Domain.Entities;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Entities.ConfigurationModule;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Common.Interfaces.ISalesModule
{
    internal interface ISafeTransactionService
    {
        Task ApplySalesPaymentTransactionAsync(
                                        SalesInvoicePayment payment,
                                        JournalEntry journalEntry,
                                        CreateSalesInvoicePaymentCommand request,
                                        string invoiceNumber,
                                        IEnumerable<SystemSetting> systemSettings,
                                        CancellationToken cancellationToken);

        Task SalesPaymentSafeReversal(DeleteSalesInvoicePaymentCommand request,
                                      SalesInvoicePayment payment,
                                      JournalEntry reversalJournal,
                                      IGenericRepository<Safe, Guid> safeRepo,
                                      CancellationToken cancellationToken);
    }
}
