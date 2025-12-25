using Khazen.Application.BaseSpecifications;

namespace Khazen.Application.Specification.HRModule.DeductionSpecifications
{
    internal class GetAllDeductionsCountSpecification : BaseSpecifications<Deduction>
    {
        public GetAllDeductionsCountSpecification()
           : base(a => true)
        {
        }
    }
}
