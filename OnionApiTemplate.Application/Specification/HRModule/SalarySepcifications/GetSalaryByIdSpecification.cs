namespace Khazen.Application.BaseSpecifications.HRModule.SalarySepcifications
{
    internal class GetSalaryByIdSpecification
        : BaseSpecifications<Salary>
    {
        public GetSalaryByIdSpecification(Guid Id)
            : base(s => s.Id == Id)
        {
            AddInclude(s => s.Employee!);
        }
    }
}
