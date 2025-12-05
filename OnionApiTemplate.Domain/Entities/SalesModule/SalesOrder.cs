using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Khazen.Domain.Entities.SalesModule
{
    public class SalesOrder : BaseEntity<Guid>
    {
        public DateTime OrderDate { get; private set; } = DateTime.UtcNow;
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;

        public decimal SubTotalAmount { get; private set; }
        [NotMapped]
        public decimal DiscountAmount => CalculateDiscountAmount();
        [NotMapped]
        public decimal TaxAmount => CalculateTaxAmount();
        public decimal GrandTotal { get; private set; }

        public DiscountType DiscountType { get; set; } = DiscountType.FixedAmount;
        public decimal DiscountValue { get; set; } = 0;
        public decimal TaxRate { get; set; } = 0;

        public DateTime? EstimatedShipDate { get; set; }
        public DateTime? ActualShipDate { get; private set; }
        public string? TrackingNumber { get; private set; }

        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public string CustomerNameSnapshot { get; set; } = string.Empty;
        public string? CanceledBy { get; private set; }
        public DateTime? CanceledAt { get; private set; }
        public string? ShippedBy { get; private set; }
        public DateTime? ShippedAt { get; private set; }
        public string? DeliveredBy { get; private set; }
        public DateTime? DeliveredAt { get; private set; }

        public ICollection<SalesOrderItem> Items { get; set; } = [];
        public Guid? InvoiceId { get; set; }
        public SalesInvoice? Invoice { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; private set; }
        public void AssertRowVersion(byte[]? requestVersion)
        {
            if (requestVersion is null)
                throw new ConcurrencyException("RowVersion is missing.");

            if (RowVersion is null)
                throw new ConcurrencyException("Entity RowVersion is missing.");

            if (!RowVersion.SequenceEqual(requestVersion))
                throw new ConcurrencyException("Order was modified by another user.");
        }

        public void AddItem(SalesOrderItem item)
        {
            Items.Add(item);
            CalculateTotals();
        }
        public void ReserveStock(WarehouseProduct wp, int quantity)
        {
            if (quantity > wp.QuantityInStock - wp.ReservedQuantity)
                throw new BadRequestException($"Insufficient stock for Product {wp.ProductId}");

            wp.ReservedQuantity += quantity;
        }

        public void CalculateTotals()
        {
            SubTotalAmount = Items.Sum(i => i.SubTotal);
            GrandTotal = SubTotalAmount - CalculateDiscountAmount() + CalculateTaxAmount();
            if (Customer != null && string.IsNullOrEmpty(CustomerNameSnapshot))
                CustomerNameSnapshot = $"{Customer.Name} {Customer.Address}";
        }

        private decimal CalculateDiscountAmount() =>
            DiscountType == DiscountType.FixedAmount
                ? DiscountValue
                : SubTotalAmount * (DiscountValue / 100);

        private decimal CalculateTaxAmount() =>
            (SubTotalAmount - DiscountAmount) * (TaxRate / 100);

        private decimal CalculateGrandTotal() =>
            SubTotalAmount - DiscountAmount + TaxAmount;

        public void MarkAsConfirmed()
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be confirmed.");
            CalculateTotals();
            Status = OrderStatus.Confirmed;
        }

        public void MarkAsShipped(string shippedBy, string? trackingNumber = null)
        {
            if (Status != OrderStatus.Confirmed)
                throw new InvalidOperationException("Order must be confirmed before shipping.");
            Status = OrderStatus.Shipped;
            ActualShipDate = DateTime.UtcNow;
            ShippedBy = shippedBy;
            ShippedAt = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(trackingNumber))
                TrackingNumber = trackingNumber;
        }

        public void MarkAsDelivered(string deliveredBy)
        {
            if (Status != OrderStatus.Shipped)
                throw new BadRequestException($"Only shipped orders can be marked as delivered.");
            Status = OrderStatus.Delivered;
            DeliveredBy = deliveredBy;
            DeliveredAt = DateTime.UtcNow;
        }

        public void MarkAsCanceled(string cancelledBy)
        {
            if (Status is OrderStatus.Shipped or OrderStatus.Delivered)
                throw new BadRequestException($"Order with status {Status} cannot be cancelled.");
            Status = OrderStatus.Cancelled;
            CanceledBy = cancelledBy;
            CanceledAt = DateTime.UtcNow;
        }
    }
}
