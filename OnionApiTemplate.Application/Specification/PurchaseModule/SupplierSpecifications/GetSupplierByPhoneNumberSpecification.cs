using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.BaseSpecifications.PurchaseModule.SupplierSpecifications
{
    internal class GetSupplierByPhoneNumberSpecification
        : BaseSpecifications<Supplier>
    {
        public GetSupplierByPhoneNumberSpecification(string PhoneNumber)
    : base(s => s.PhoneNumber == PhoneNumber && s.IsActive == true)
        {
        }
    }
}
