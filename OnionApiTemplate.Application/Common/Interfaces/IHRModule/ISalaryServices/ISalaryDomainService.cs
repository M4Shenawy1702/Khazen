namespace Khazen.Application.Common.Interfaces.IHRModule.ISalaryServices
{
    internal interface ISalaryDomainService
    {
        Salary CreateSalary(Employee employee, DateTime salaryDate, string createdBy, string? notes = null);
    }
}
