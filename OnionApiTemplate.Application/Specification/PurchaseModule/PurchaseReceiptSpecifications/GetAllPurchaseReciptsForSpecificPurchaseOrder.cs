using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.PurchaseModule;

public class GetAllPurchaseReceiptsForSpecificPurchaseOrder
    : BaseSpecifications<PurchaseReceipt>
{
    public GetAllPurchaseReceiptsForSpecificPurchaseOrder(IEnumerable<Guid> ids)
        : base(r => ids.Contains(r.Id))
    {
        AddInclude(r => r.Items);
    }
}
