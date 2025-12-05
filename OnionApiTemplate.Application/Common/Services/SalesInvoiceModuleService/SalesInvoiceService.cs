using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.Interfaces.ISalesModule.ISalesInvoiceServices;
using Khazen.Application.DOTs.SalesModule.SalesInvoicesDots;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.Common.Services.SalesInvoiceModuleService
{
    internal class SalesInvoiceService(
        IUnitOfWork unitOfWork,
        ILogger<SalesInvoiceService> logger,
        INumberSequenceService numberSequenceService)
        : ISalesInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<SalesInvoiceService> _logger = logger;
        private readonly INumberSequenceService _numberSequenceService = numberSequenceService;

        public async Task<SalesInvoice> CreateSalesInvoice(SalesOrder salesOrder, Customer customer, CreateSalesInvoiceDto dto, string CreatedBy, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Creating SalesInvoice for SalesOrder {OrderId}, Customer {CustomerId}.",
                salesOrder.Id, customer.Id);

            if (salesOrder.Status != OrderStatus.Delivered)
            {
                _logger.LogWarning(
                    "Invoice creation failed: SalesOrder {OrderId} is not delivered. Current status: {Status}.",
                    salesOrder.Id, salesOrder.Status);

                throw new BadRequestException("An invoice can only be created for delivered orders.");
            }

            if (salesOrder.CustomerId != customer.Id)
            {
                _logger.LogWarning(
                    "Invoice creation failed: SalesOrder {OrderId} does not belong to Customer {CustomerId}.",
                    salesOrder.Id, customer.Id);

                throw new BadRequestException("The sales order does not belong to the specified customer.");
            }

            if (salesOrder.Items.Count == 0)
            {
                _logger.LogWarning(
                    "Invoice creation failed: SalesOrder {OrderId} contains no items.",
                    salesOrder.Id);

                throw new BadRequestException("Sales order cannot have an invoice without items.");
            }

            var invoiceNumber = await _numberSequenceService.GetNextNumber("SalesInvoice", DateTime.Now.Year, cancellationToken);

            _logger.LogInformation(
                "Generated InvoiceNumber {InvoiceNumber} for Order {OrderId}.",
                invoiceNumber, salesOrder.Id);

            var salesInvoice = new SalesInvoice
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                SalesOrderId = salesOrder.Id,
                InvoiceDate = dto.InvoiceDate,
                InvoiceNumber = invoiceNumber,
                Notes = dto.Notes ?? string.Empty,
                SubTotal = salesOrder.SubTotalAmount,
                TaxAmount = salesOrder.TaxAmount,
                GrandTotal = salesOrder.GrandTotal,
                CreatedBy = CreatedBy,
                CreatedAt = DateTime.UtcNow,
                Items = salesOrder.Items.Select(orderItem => new SalesInvoiceItem
                {
                    ProductId = orderItem.ProductId,
                    Quantity = orderItem.Quantity,
                    UnitPrice = orderItem.UnitPrice,
                    DiscountType = orderItem.DiscountType,
                    DiscountValue = orderItem.DiscountValue
                }).ToList()
            };

            salesInvoice.UpdateInvoiceStatus();
            _logger.LogDebug("Invoice base created with SubTotal {SubTotal}, Tax {TaxAmount}, GrandTotal {GrandTotal}.",
                salesInvoice.SubTotal, salesInvoice.TaxAmount, salesInvoice.GrandTotal);

            if (dto.DiscountType.HasValue && dto.DiscountValue.HasValue)
            {
                _logger.LogInformation(
                    "Applying discount on SalesInvoice {InvoiceId}. Type: {Type}, Value: {Value}.",
                    salesInvoice.Id, dto.DiscountType.Value, dto.DiscountValue.Value);

                salesInvoice.DiscountAmount = dto.DiscountType.Value switch
                {
                    DiscountType.Percentage => salesInvoice.SubTotal * dto.DiscountValue.Value / 100m,
                    DiscountType.FixedAmount => dto.DiscountValue.Value,
                    _ => 0m
                };

                _logger.LogDebug(
                    "Calculated DiscountAmount for Invoice {InvoiceId} = {DiscountAmount}.",
                    salesInvoice.Id, salesInvoice.DiscountAmount);
            }
            else
            {
                _logger.LogInformation("No discount applied for Invoice {InvoiceId}.", salesInvoice.Id);
                salesInvoice.DiscountAmount = 0m;
            }


            salesInvoice.CalculateTotals();

            _logger.LogDebug(
                "After recalculation: Invoice {InvoiceId} => SubTotal={SubTotal}, Discount={Discount}, Tax={Tax}, GrandTotal={GrandTotal}.",
                salesInvoice.Id, salesInvoice.SubTotal, salesInvoice.DiscountAmount, salesInvoice.TaxAmount, salesInvoice.GrandTotal);

            if (salesInvoice.GrandTotal < 0)
            {
                _logger.LogWarning(
                    "GrandTotal negative for Invoice {InvoiceId}. Clamping to 0. Original: {OriginalGrandTotal}.",
                    salesInvoice.Id, salesInvoice.GrandTotal);

                salesInvoice.GrandTotal = 0;
            }

            _logger.LogInformation(
                "SalesInvoice {InvoiceId} successfully created for Order {OrderId}.",
                salesInvoice.Id, salesOrder.Id);

            return salesInvoice;
        }
    }
}
