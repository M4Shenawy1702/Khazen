namespace Khazen.Domain.Entities.HRModule
{
    public class Department
        : BaseEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ToggledBy { get; set; }
        public DateTime? ToggledAt { get; set; }
        public ICollection<Employee> Employees { get; set; } = [];

        public void Toggle(string modifiedBy)
        {
            IsDeleted = !IsDeleted;
            ToggledAt = DateTime.UtcNow;
            ToggledBy = modifiedBy;
        }
    }
}