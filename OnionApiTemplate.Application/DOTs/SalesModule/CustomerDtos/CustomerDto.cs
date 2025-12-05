namespace Khazen.Application.DOTs.SalesModule.Customer
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string CustomerType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
