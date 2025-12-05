namespace Khazen.Application.DOTs.HRModule.DeductionDtos
{
    public class DeductionDetailsDto
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string EmployeeName { get; set; }
        public DateOnly Date { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public string CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
