namespace Khazen.Application.DOTs.HRModule.BonusDtos
{
    public class BonusDetailsDto
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public decimal Amount { get; set; }

        public string Reason { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
