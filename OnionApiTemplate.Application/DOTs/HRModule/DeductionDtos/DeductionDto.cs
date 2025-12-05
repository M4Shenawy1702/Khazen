namespace Khazen.Application.DOTs.HRModule.Deduction
{
    public class DeductionDto
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public string EmployeeName { get; set; }
        public DateOnly Date { get; set; }
        public decimal Amount { get; set; }
    }
}
