namespace Khazen.Application.DOTs.HRModule.Employee
{
    public class UpdateEmployeeDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public DateOnly HireDate { get; set; }
        public JobTitle JobTitle { get; set; }
        public decimal BaseSalary { get; set; }
    }
}
