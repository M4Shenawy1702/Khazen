namespace Khazen.Application.BaseSpecifications.HRModule.DeductionSpecifications
{
    internal class GetAllDeductionsByEmpIdSpecification
        : BaseSpecifications<Deduction>
    {
        public GetAllDeductionsByEmpIdSpecification(Guid EmpId)
            : base(d => d.EmployeeId == EmpId)
        {
            AddInclude(d => d.Employee);
        }
    }
}
