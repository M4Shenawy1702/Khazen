namespace Khazen.Application.BaseSpecifications.HRModule.DeductionSpecifications
{
    internal class GetDeductionByIdSpecification
        : BaseSpecifications<Deduction>
    {
        public GetDeductionByIdSpecification(Guid Id)
            : base(d => d.Id == Id)
        {
            AddInclude(d => d.Employee);
        }
    }
}
