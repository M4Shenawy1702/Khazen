namespace Khazen.Application.DOTs.HRModule.AdvanceDtos
{
    public class AdvanceDetailsDto
    {
        public int Id { get; set; }
        public Guid EmployeeName { get; set; }
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; }
        public string Reason { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
