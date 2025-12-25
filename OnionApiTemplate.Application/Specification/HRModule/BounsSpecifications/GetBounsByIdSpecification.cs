using Khazen.Application.BaseSpecifications;

namespace Khazen.Application.Specification.HRModule.BounsSpecifications
{
    internal class GetBounsByIdSpecification
        : BaseSpecifications<Bonus>
    {
        public GetBounsByIdSpecification(Guid Id)
            : base(c => c.Id == Id)
        {
        }
    }
}
