using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.SalesModule.SalesOrderPaymentDtos
{
    public class CreateSalesInvoicePaymentDto
    {
        public Guid SalesInvoiceId { get; set; }
        public decimal Amount { get; set; }
        public string? Notes { get; set; }
        public PaymentMethod Method { get; set; }
    }
}
