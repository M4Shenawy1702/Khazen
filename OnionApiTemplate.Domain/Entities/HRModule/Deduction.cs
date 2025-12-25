namespace Khazen.Domain.Entities.HRModule
{
    public class Deduction
        : BaseEntity<Guid>
    {
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        public DateOnly Date { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public bool IsProcessed { get; set; }
        public string? ToggledBy { get; set; }
        public DateTime? ToggledAt { get; set; }
        public void Toggle(string modifiedBy)
        {
            IsDeleted = !IsDeleted;
            ToggledAt = DateTime.UtcNow;
            ToggledBy = modifiedBy;
        }
    }
}
