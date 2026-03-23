using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.SalesModule.Customer
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string CreatedBy { get; set; }
        public string? CurrentUserId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public string UserId { get; set; }
        public CustomerType CustomerType { get; set; }

        public decimal TotalInvoiced { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal OutstandingBalance { get; set; }

        public string? ToggledBy { get; set; }
        public DateTime? ToggledAt { get; set; }
    }
}
