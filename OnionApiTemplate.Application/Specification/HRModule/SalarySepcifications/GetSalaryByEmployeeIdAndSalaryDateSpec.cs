namespace Khazen.Application.BaseSpecifications.HRModule.SalarySepcifications
{
    internal class GetSalaryByEmployeeIdAndSalaryDateSpec
        : BaseSpecifications<Salary>
    {
        public GetSalaryByEmployeeIdAndSalaryDateSpec(Guid EmployeeId, DateTime SalaryDate)
            : base(s => s.EmployeeId == EmployeeId
                   && s.SalaryDate.Month == SalaryDate.Month)
        {
        }
    }
}
