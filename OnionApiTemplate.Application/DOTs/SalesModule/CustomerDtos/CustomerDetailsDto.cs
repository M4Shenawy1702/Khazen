using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.DOTs.SalesModule.Customer
{
    public class CustomerDetailsDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public CustomerType CustomerType { get; set; } = CustomerType.Individual;
        public bool IsActive { get; set; }
        public ICollection<SalesOrder> Orders { get; set; } = [];
        public ICollection<SalesInvoice> SalesInvoices { get; set; } = [];
    }
}
