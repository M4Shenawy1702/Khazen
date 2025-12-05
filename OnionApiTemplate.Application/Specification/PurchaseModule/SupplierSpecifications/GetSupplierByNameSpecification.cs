using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.BaseSpecifications.PurchaseModule.SupplierSpecifications
{
    internal class GetSupplierByNameSpecification
        : BaseSpecifications<Supplier>
    {
        public GetSupplierByNameSpecification(string name)
            : base(s => s.Name == name && s.IsActive == true)
        {
        }
    }
}
