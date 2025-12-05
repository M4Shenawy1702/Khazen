using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.UsersModule;

namespace Khazen.Domain.Entities.SalesModule
{
    public class Customer : BaseEntity<Guid>
    {
        public Customer(string name, string address, string userId, CustomerType customerType, string createdBy)
        {
            Name = name;
            Address = address;
            UserId = userId;
            CustomerType = customerType;

            CreatedAt = DateTime.UtcNow;
            CreatedBy = createdBy;
        }
        public void Modify(string name, string address, CustomerType customerType, string modifiedBy)
        {
            Name = name;
            Address = address;
            CustomerType = customerType;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = modifiedBy;
        }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;
        public string UserId { get; set; } = null!;

        public CustomerType CustomerType { get; set; } = CustomerType.Individual;

        public ICollection<SalesOrder> Orders { get; set; } = [];
        public ICollection<SalesInvoice> Invoices { get; set; } = [];

        public decimal TotalInvoiced => Invoices.Sum(i => i.GrandTotal);
        public decimal TotalPaid => Invoices.Sum(i => i.TotalPaid);
        public decimal OutstandingBalance => TotalInvoiced - TotalPaid;

        public void Toggle(string modifiedBy)
        {
            IsDeleted = !IsDeleted;
            ModifiedBy = modifiedBy;
            ModifiedAt = DateTime.UtcNow;
        }
    }
}
