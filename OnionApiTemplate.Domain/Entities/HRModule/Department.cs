namespace Khazen.Domain.Entities.HRModule
{
    public class Department
        : BaseEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<Employee> Employees { get; set; } = [];

        public void Toggle(string modifiedBy)
        {
            IsDeleted = !IsDeleted;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = modifiedBy;
        }
    }
}