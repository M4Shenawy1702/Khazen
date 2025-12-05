namespace Khazen.Domain.Entities.HRModule
{
    public class Bonus
        : BaseEntity<int>
    {
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        public DateOnly Date { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;

        public void Toggle(string modifiedBy)
        {
            IsDeleted = !IsDeleted;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = modifiedBy;
        }
    }
}
