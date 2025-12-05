using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.SalesModule.SalesOrderPaymentDtos
{
    public class UpdateSalesInvoicePaymentDto
    {
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
