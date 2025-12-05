namespace Khazen.Application.DOTs.HRModule.SalaryDots
{
    internal class SalaryDetailsDto
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;

        public DateTime SalaryDate { get; set; }

        public decimal BasicSalary { get; private set; }
        public decimal TotalBonus { get; private set; }
        public decimal TotalDeduction { get; private set; }
        public decimal TotalAdvance { get; private set; }
        public decimal NetSalary { get; private set; }

        public string Notes { get; private set; } = string.Empty;
    }
}
