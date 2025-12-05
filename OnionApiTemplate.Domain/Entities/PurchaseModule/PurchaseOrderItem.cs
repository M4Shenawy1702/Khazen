using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khazen.Domain.Entities.PurchaseModule
{
    public class PurchaseOrderItem : BaseEntity<Guid>
    {
        public Guid PurchaseOrderId { get; private set; }
        public PurchaseOrder? PurchaseOrder { get; private set; }

        public Guid ProductId { get; private set; }
        public Product? Product { get; private set; }

        public int Quantity { get; private set; }
        public decimal ExpectedUnitPrice { get; private set; }

        [NotMapped]
        public decimal TotalPrice => Quantity * ExpectedUnitPrice;

        protected PurchaseOrderItem() { }

        public PurchaseOrderItem(Guid productId, int quantity, decimal expectedUnitPrice)
        {
            if (productId == Guid.Empty)
                throw new DomainException("ProductId cannot be empty.", nameof(productId));
            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than 0.", nameof(quantity));
            if (expectedUnitPrice <= 0)
                throw new DomainException("ExpectedUnitPrice must be greater than 0.", nameof(expectedUnitPrice));

            ProductId = productId;
            Quantity = quantity;
            ExpectedUnitPrice = expectedUnitPrice;
        }

    }
}
