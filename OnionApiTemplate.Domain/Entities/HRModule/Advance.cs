using System.ComponentModel.DataAnnotations;

namespace Khazen.Domain.Entities.HRModule
{
    public class Advance
            : BaseEntity<int>
    {
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        [Required]
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [Required, MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
        public void Toggle(string modifiedBy)
        {
            IsDeleted = !IsDeleted;
            ModifiedAt = DateTime.UtcNow;
            ModifiedBy = modifiedBy;
        }
    }
}
