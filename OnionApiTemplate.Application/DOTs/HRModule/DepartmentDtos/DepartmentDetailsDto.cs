namespace Khazen.Application.DOTs.HRModule.Department
{
    public class DepartmentDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public ICollection<EmployeeDto> Employees { get; set; } = [];
    }
}
