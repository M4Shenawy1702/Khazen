using Khazen.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Khazen.Domain.Entities.HRModule
{
    public class Advance : BaseEntity<int>
    {
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        [Required]
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        [Required, MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
        public DateTime? ToggledAt { get; set; }
        public string? ToggledBy { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public void AssertRowVersion(byte[]? requestVersion)
        {
            if (requestVersion is null)
                throw new ConcurrencyException("RowVersion is missing.");

            if (RowVersion is null)
                throw new ConcurrencyException("Entity RowVersion is missing.");

            if (!RowVersion.SequenceEqual(requestVersion))
                throw new ConcurrencyException("Order was modified by another user.");
        }
        public void Toggle(string toggledBy)
        {
            IsDeleted = !IsDeleted;
            ToggledAt = DateTime.UtcNow;
            ToggledBy = toggledBy;
        }
    }
}
