using Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseInvoice;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

public class PurchaseInvoiceStockCostService : IPurchaseInvoiceStockCostService
{
    private readonly ILogger<PurchaseInvoiceStockCostService> _logger;

    public PurchaseInvoiceStockCostService(ILogger<PurchaseInvoiceStockCostService> logger)
    {
        _logger = logger;
    }

    public Task UpdateStockAndCostAsync(PurchaseInvoice invoice, Guid warehouseId, CancellationToken cancellationToken = default)
    {
        foreach (var item in invoice.Items)
        {
            var product = item.Product ?? throw new NotFoundException<Product>(item.ProductId);

            var oldPrice = product.AvgPrice;
            var oldQty = product.WarehouseProducts.FirstOrDefault(w => w.WarehouseId == warehouseId)?.QuantityInStock ?? 0;
            var newQty = item.Quantity;
            var newPrice = item.UnitPrice;

            var totalQty = oldQty + newQty;

            product.AvgPrice = totalQty > 0
                ? ((oldQty * oldPrice) + (newQty * newPrice)) / totalQty
                : newPrice;

            product.PurchasePrice = item.UnitPrice;

            // Margin (TODO: get from system settings)
            var marginPercent = 0.20m;
            product.SellingPrice = Math.Round(item.UnitPrice * (1 + marginPercent), 2);

            var warehouseProduct = product.WarehouseProducts.FirstOrDefault(w => w.WarehouseId == warehouseId);
            if (warehouseProduct == null)
            {
                warehouseProduct = new WarehouseProduct
                {
                    ProductId = product.Id,
                    WarehouseId = warehouseId,
                    QuantityInStock = newQty
                };
                product.WarehouseProducts.Add(warehouseProduct);
            }
            else
            {
                warehouseProduct.QuantityInStock += newQty;
            }

            _logger.LogDebug(
                "Invoice {InvoiceNo}: Updated Product {ProductId}: AvgPrice={AvgPrice}, SellingPrice={SellingPrice}, WarehouseStock={Stock}",
                invoice.InvoiceNumber, product.Id, product.AvgPrice, product.SellingPrice,
                product.WarehouseProducts.First(w => w.WarehouseId == warehouseId).QuantityInStock);
        }

        return Task.CompletedTask;
    }
}
