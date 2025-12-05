using Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseInvoice;
using Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Create;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.Common.Services.PurchaseModuleServices.PurchaseInvoiceServices
{
    public class PurchaseInvoiceFactory : IInvoiceFactoryService
    {
        private readonly ILogger<PurchaseInvoiceFactory> _logger;

        public PurchaseInvoiceFactory(ILogger<PurchaseInvoiceFactory> logger)
        {
            _logger = logger;
        }

        public async Task<PurchaseInvoice> CreateInvoiceAsync(
            PurchaseReceipt receipt,
          CreateInvoiceForReceiptCommand command,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Creating PurchaseInvoice from Receipt {ReceiptId}", receipt.Id);

            if (receipt == null) throw new ArgumentNullException(nameof(receipt));
            if (command.Dto == null) throw new ArgumentNullException(nameof(command.Dto));

            if (receipt.Items == null || receipt.Items.Count == 0)
                throw new BadRequestException("Cannot create invoice for a receipt without items.");

            if (command.Dto.Items.GroupBy(x => x.ProductId).Any(g => g.Count() > 1))
                throw new BadRequestException("Duplicate product items are not allowed.");

            if (receipt.Invoice != null)
                throw new BadRequestException("Receipt already has an invoice.");

            if (receipt.Items.Count != command.Dto.Items.Count)
                throw new BadRequestException("Invoice items count must match receipt items count.");

            var invoice = new PurchaseInvoice(receipt.Id, receipt.PurchaseOrder!.SupplierId, command.Dto.InvoiceNumber, command.CreatedBy, command.Dto.InvoiceDate, command.Dto.Notes);

            var dtoMap = command.Dto.Items.ToDictionary(i => i.ProductId, i => i);

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
