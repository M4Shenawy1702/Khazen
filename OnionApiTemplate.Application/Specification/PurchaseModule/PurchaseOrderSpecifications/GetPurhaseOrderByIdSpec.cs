using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Specification.PurchaseModule.PurchaseOrderSpecifications
{
    public class GetPurhaseOrderByIdSpec
        : BaseSpecifications<PurchaseOrder>
    {
        public GetPurhaseOrderByIdSpec(Guid Id)
            : base(po => po.Id == Id)
        {
            AddInclude(po => po.Items);
            AddInclude(po => po.PurchaseReceipts);
            AddInclude("Items.Product");
            AddInclude("PurchaseReceipts.Items.Product");
        }
    }
}
