using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.BaseSpecifications.PurchaseModule.SupplierSpecifications
{
    internal class GetSupplierByEmailSpecification
        : BaseSpecifications<Supplier>
    {
        public GetSupplierByEmailSpecification(string Email)
        : base(s => s.Email == Email && s.IsActive == true)
        {
        }
    }
}
