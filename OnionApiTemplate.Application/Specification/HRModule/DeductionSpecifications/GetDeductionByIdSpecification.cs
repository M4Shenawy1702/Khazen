namespace Khazen.Application.BaseSpecifications.HRModule.DeductionSpecifications
{
    internal class GetDeductionByIdSpecification
        : BaseSpecifications<Deduction>
    {
        public GetDeductionByIdSpecification(int Id)
            : base(d => d.Id == Id)
        {
            AddInclude(d => d.Employee);
        }
    }
}
