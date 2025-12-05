namespace Khazen.Application.DOTs.HRModule.AdvanceDtos
{
    public class AdvanceDto
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public decimal Amount { get; set; }
    }
}
