namespace Khazen.Application.BaseSpecifications.HRModule.SalarySepcifications
{
    internal class GetSalaryWithEmployeeByIdSpec
        : BaseSpecifications<Salary>
    {
        public GetSalaryWithEmployeeByIdSpec(Guid salaryId)
            : base(s => s.Id == salaryId)
        {
            AddInclude(s => s.Employee!);
        }
    }
}
