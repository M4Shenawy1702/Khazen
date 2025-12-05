namespace Khazen.Domain.Entities.HRModule
{
    public class PerformanceReview
        : BaseEntity<Guid>
    {
        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public Guid ReviewerId { get; set; }
        public Employee? Reviewer { get; set; }

        public DateOnly ReviewDate { get; set; }

        public string Comments { get; set; } = string.Empty;
        public int Rate { get; set; }
        public string ActionPlan { get; set; } = string.Empty;
        public void Toggle(string modifiedBy)
        {
            IsDeleted = !IsDeleted;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = modifiedBy;
        }
    }
}
