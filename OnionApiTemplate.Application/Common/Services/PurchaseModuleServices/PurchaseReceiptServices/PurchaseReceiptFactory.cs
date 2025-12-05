using Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseReceipt;
using Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos;
using Khazen.Application.UseCases.PurchaseModule.PurchaseReceiptUseCases.Commands.Create;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;

namespace Khazen.Application.Common.Services.PurchaseModuleServices.PurchaseReceiptServices
{
    public class PurchaseReceiptFactory : IPurchaseReceiptFactory
    {
        public PurchaseReceipt CreateReceipt(
            PurchaseOrder order,
            Warehouse warehouse,
             string CreatedBy,
            CreatePurchaseReceiptCommand command)
        {
            if (command.Dto.Items == null || command.Dto.Items.Count == 0)
                throw new BadRequestException("Receipt must have at least one item.");

            var receipt = new PurchaseReceipt(order.Id, warehouse.Id, command.CreatedBy, command.Dto.Notes);

            ValidateProducts(order, command.Dto);
            ValidateQuantities(order, command.Dto);

            foreach (var item in command.Dto.Items)
            {
                receipt.AddItem(new PurchaseReceiptItem
                {
                    ProductId = item.ProductId,
                    ReceivedQuantity = item.ReceivedQuantity
                });
            }

            return receipt;
        }

        private static void ValidateProducts(PurchaseOrder order, CreatePurchaseReceiptDto dto)
        {
            var orderProducts = order.Items.Select(x => x.ProductId).ToHashSet();

            foreach (var item in dto.Items)
            {
                if (!orderProducts.Contains(item.ProductId))
                    throw new BadRequestException($"Product {item.ProductId} is not in the purchase order.");
            }
        }

        private static void ValidateQuantities(PurchaseOrder order, CreatePurchaseReceiptDto dto)
        {
            var ordered = order.Items.ToDictionary(x => x.ProductId, x => x.Quantity);
            var alreadyReceived = order.PurchaseReceipts
                .SelectMany(r => r.Items)
                .GroupBy(i => i.ProductId)
                .ToDictionary(g => g.Key, g => g.Sum(i => i.ReceivedQuantity));

            foreach (var item in dto.Items)
            {
                var orderedQty = ordered[item.ProductId];
                var received = alreadyReceived.GetValueOrDefault(item.ProductId, 0);

                if (received + item.ReceivedQuantity > orderedQty)
                {
                    throw new BadRequestException(
                        $"Product {item.ProductId} exceeds ordered quantity. Ordered: {orderedQty}, Received before: {received}, Current: {item.ReceivedQuantity}");
                }
            }
        }
    }

}
