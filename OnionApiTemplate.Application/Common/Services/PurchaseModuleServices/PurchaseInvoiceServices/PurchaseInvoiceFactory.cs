using Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseInvoice;
using Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.Common.Services.PurchaseModuleServices.PurchaseInvoiceServices
{
    public class PurchaseInvoiceFactory(ILogger<PurchaseInvoiceFactory> logger) : IInvoiceFactoryService
    {
        private readonly ILogger<PurchaseInvoiceFactory> _logger = logger;

        public async Task<PurchaseInvoice> CreateInvoiceAsync(
            PurchaseReceipt receipt,
          CreatePurchaseInvoiceDto Dto,
           string userId,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Creating PurchaseInvoice from Receipt {ReceiptId}", receipt.Id);

            if (receipt == null) throw new ArgumentNullException(nameof(receipt));
            if (Dto == null) throw new ArgumentNullException(nameof(Dto));

            if (receipt.Items == null || receipt.Items.Count == 0)
                throw new BadRequestException("Cannot create invoice for a receipt without items.");

            if (Dto.Items.GroupBy(x => x.ProductId).Any(g => g.Count() > 1))
                throw new BadRequestException("Duplicate product items are not allowed.");

            if (receipt.Invoice != null)
                throw new BadRequestException("Receipt already has an invoice.");

            if (receipt.Items.Count != Dto.Items.Count)
                throw new BadRequestException("Invoice items count must match receipt items count.");

            var invoice = new PurchaseInvoice(receipt.Id, receipt.PurchaseOrder!.SupplierId, Dto.InvoiceNumber, userId, Dto.InvoiceDate, Dto.Notes);

            var dtoMap = Dto.Items.ToDictionary(i => i.ProductId, i => i);

            foreach (var item in receipt.Items)
            {
                if (!dtoMap.TryGetValue(item.ProductId, out var dtoItem))
                    throw new BadRequestException($"Missing invoice item for product {item.ProductId}");

                invoice.Items.Add(new PurchaseInvoiceItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.ReceivedQuantity,
                    UnitPrice = dtoItem.UnitPrice
                });
            }

            invoice.RecalculateTotals();

            _logger.LogInformation("PurchaseInvoice created from Receipt {ReceiptId} with {ItemCount} items.",
                receipt.Id, invoice.Items.Count);

            return invoice;
        }
    }
}
