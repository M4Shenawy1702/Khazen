namespace Khazen.Application.BaseSpecifications.HRModule.AdvanceSpecifications
{
    internal class GetAdvanceByIdSpecification
        : BaseSpecifications<Advance>
    {
        public GetAdvanceByIdSpecification(int Id)
            : base(a => a.Id == Id)
        {
            AddInclude(a => a.Employee);
        }
    }
}
