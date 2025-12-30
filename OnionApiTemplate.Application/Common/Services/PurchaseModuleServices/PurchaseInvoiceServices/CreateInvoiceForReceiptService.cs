using Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseInvoice;
using Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.Common.Services.PurchaseModuleServices.PurchaseInvoiceServices
{
    public class InvoiceFactoryService : IInvoiceFactoryService
    {
        public async Task<PurchaseInvoice> CreateInvoiceAsync(PurchaseReceipt receipt, CreatePurchaseInvoiceDto Dto, string userId, CancellationToken cancellationToken = default)
        {
            if (receipt == null)
                throw new ArgumentNullException(nameof(receipt));

            if (Dto == null)
                throw new ArgumentNullException(nameof(Dto));

            if (receipt.Items == null || receipt.Items.Count == 0)
                throw new InvalidOperationException("Cannot create invoice for a receipt without items.");

            if (receipt.Invoice != null)
                throw new InvalidOperationException("Receipt already has an invoice.");

            if (receipt.Items.Count != Dto.Items.Count)
                throw new InvalidOperationException("Invoice items count must match receipt items count.");

            //var invoice = new PurchaseInvoice
            //{
            //    PurchaseReceiptId = receipt.Id,
            //    SupplierId = receipt.PurchaseOrder!.SupplierId,
            //    InvoiceNumber = command.Dto.InvoiceNumber,
            //    InvoiceDate = DateTime.UtcNow,
            //    Notes = command.Dto.Notes,
            //    InvoiceStatus = Domain.Common.Enums.InvoiceStatus.Pending,
            //    PaymentStatus = Domain.Common.Enums.PaymentStatus.Unpaid,
            //    CreatedBy = command.CreatedBy,
            //    Items = command.Dto.Items.Select(i => new PurchaseInvoiceItem
            //    {
            //        ProductId = i.ProductId,
            //        Quantity = receipt.Items.First(r => r.ProductId == i.ProductId).ReceivedQuantity,
            //        UnitPrice = i.UnitPrice
            //    }).ToList()
            //};
            var invoice = new PurchaseInvoice(receipt.Id, receipt.PurchaseOrder!.SupplierId, Dto.InvoiceNumber, userId, Dto.InvoiceDate, Dto.Notes);
            foreach (var item in Dto.Items)
            {
                invoice.AddItem(new PurchaseInvoiceItem
                {
                    ProductId = item.ProductId,
                    Quantity = receipt.Items.First(r => r.ProductId == item.ProductId).ReceivedQuantity,
                    UnitPrice = item.UnitPrice,
                    AdditionalCharges = item.AdditionalCharges,
                    Notes = item.Notes
                });
            }
            return invoice;
        }
    }
}
