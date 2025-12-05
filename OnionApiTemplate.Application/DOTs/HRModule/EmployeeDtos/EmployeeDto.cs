namespace Khazen.Application.DOTs.HRModule.Employee
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public bool IsActive { get; set; }
    }
}
