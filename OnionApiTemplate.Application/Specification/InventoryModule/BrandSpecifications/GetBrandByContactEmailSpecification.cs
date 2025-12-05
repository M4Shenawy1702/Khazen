using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.Specification.InventoryModule.BrandSpecifications
{
    internal class GetBrandByContactEmailSpecification
        : BaseSpecifications<Brand>
    {
        public GetBrandByContactEmailSpecification(string ContactEmail)
            : base(B => B.ContactEmail == ContactEmail)
        {
        }
    }
}
