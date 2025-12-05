using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Specification.PurchaseModule.PurchaseReceiptSpecifications
{
    public class GetPurchaseReceiptWithAllIncludesByIdSpec
        : BaseSpecifications<PurchaseReceipt>
    {
        public GetPurchaseReceiptWithAllIncludesByIdSpec(Guid Id)
            : base(pr => pr.Id == Id)
        {
            AddInclude(pr => pr.Items);
            AddInclude("Items.Product");
            AddInclude("Items.Product.WarehouseProducts");
            AddInclude(pr => pr.PurchaseOrder!);
            AddInclude(pr => pr.Invoice!);
            AddInclude(pr => pr.Warehouse!);
        }
    }
}
