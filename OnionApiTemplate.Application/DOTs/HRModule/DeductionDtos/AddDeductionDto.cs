namespace Khazen.Application.DOTs.HRModule.DeductionDtos
{
    public class AddDeductionDto
    {
        public Guid EmployeeId { get; set; }
        public DateOnly Date { get; set; }
        public decimal Amount { get; set; }
        public string? Reason { get; set; }
    }
}
