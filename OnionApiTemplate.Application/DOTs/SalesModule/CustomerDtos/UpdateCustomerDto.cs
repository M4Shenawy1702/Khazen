using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.SalesModule.Customer
{
    public class UpdateCustomerDto
    {
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public CustomerType CustomerType { get; set; } = CustomerType.Individual;
    }
}
