namespace Khazen.Application.Common.Interfaces.IHRModule
{
    public interface ISalaryCalculationService
    {
        decimal CalculateTotalDeductions(Employee employee, DateTime salaryDate);
        decimal CalculateTotalBonuses(Employee employee, DateTime salaryDate);
        decimal CalculateTotalAdvances(Employee employee, DateTime salaryDate);
        decimal CalculateNetSalary(Employee employee, DateTime salaryDate);
    }
}
