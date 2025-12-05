namespace Khazen.Application.DOTs.HRModule.AdvanceDtos
{
    public class AddAdvanceDto
    {
        public Guid EmployeeId { get; set; }
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
