namespace Khazen.Domain.Entities.HRModule
{
    public class PerformanceReview
        : BaseEntity<Guid>
    {
        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public DateOnly ReviewDate { get; set; }

        public string Comments { get; set; } = string.Empty;
        public int Rate { get; set; }
        public string ActionPlan { get; set; } = string.Empty;
        public string? ToggledBy { get; set; }
        public DateTime? ToggledAt { get; set; }

        public void Toggle(string toggleBy)
        {
            IsDeleted = !IsDeleted;
            ToggledAt = DateTime.UtcNow;
            ToggledBy = toggleBy;
        }
    }
}
