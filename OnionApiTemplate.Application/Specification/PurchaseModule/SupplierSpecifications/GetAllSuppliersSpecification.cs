using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Specification.PurchaseModule.SupplierSpecifications
{
    internal class GetAllSuppliersSpecification
        : BaseSpecifications<Supplier>
    {
        public GetAllSuppliersSpecification()
            : base(s => true)
        {
        }
    }
}
