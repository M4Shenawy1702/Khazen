using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.SalesModule.Customer
{
    public class CreateCustomerDto
    {
        public string Name { get; set; } = string.Empty;
        public CustomerType CustomerType { get; set; } = CustomerType.Individual;
        public string Address { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string? CompanyName { get; set; }
        public string? TaxNumber { get; set; }
    }
}
