using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.Specification.SalesModule.OrderPaymentSpecification
{
    public class GetSalesOrderPaymentByIdSpecification
        : BaseSpecifications<SalesInvoicePayment>
    {
        public GetSalesOrderPaymentByIdSpecification(Guid Id)
            : base(sp => sp.Id == Id)
        {
            AddInclude(sp => sp.SalesInvoice!);
            AddInclude(sp => sp.JournalEntry!);
            //AddInclude(sp => sp.SafeTransactions!);
            AddInclude(sp => sp.ReversalJournalEntry!);
        }
    }
}
