using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.BaseSpecifications.PurchaseModule.SupplierSpecifications
{
    internal class GetSupplierByIdSpecification
        : BaseSpecifications<Supplier>
    {
        public GetSupplierByIdSpecification(Guid id)
            : base(s => s.Id == id)
        {
        }
    }
}
