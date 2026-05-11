namespace Khazen.Application.DOTs.HRModule.Department
{
    public class DepartmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = true;
    }
}
