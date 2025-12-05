using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Specification.PurchaseModule.PurchaseReceiptSpecifications
{
    public class GetPurchaseReceiptWithItemsByIdSpec
        : BaseSpecifications<PurchaseReceipt>
    {
        public GetPurchaseReceiptWithItemsByIdSpec(Guid Id)
            : base(pr => pr.Id == Id)
        {
            AddInclude(pr => pr.Items);
        }
    }
}
