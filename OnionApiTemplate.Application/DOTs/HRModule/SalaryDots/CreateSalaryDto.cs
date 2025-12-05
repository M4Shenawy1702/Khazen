namespace Khazen.Application.DOTs.HRModule.SalaryDots
{
    public class CreateSalaryDto
    {
        public Guid EmployeeId { get; set; }
        public DateTime SalaryDate { get; set; }
        public string? Notes { get; set; }
    }
}
