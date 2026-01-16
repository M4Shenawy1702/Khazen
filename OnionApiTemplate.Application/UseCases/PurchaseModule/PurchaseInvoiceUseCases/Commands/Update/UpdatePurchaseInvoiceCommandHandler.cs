using Khazen.Application.DOTs.PurchaseModule.PurchaseInvoiceDtos;
using Khazen.Application.Specification.PurchaseModule.PurchaseInvoiceSpecifications;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchaseInvoiceUseCases.Commands.Update
{
    public class UpdatePurchaseInvoiceCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<UpdatePurchaseInvoiceCommand> validator,
        ILogger<UpdatePurchaseInvoiceCommandHandler> logger,
        UserManager<ApplicationUser> userManager)
                : IRequestHandler<UpdatePurchaseInvoiceCommand, PurchaseInvoiceDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdatePurchaseInvoiceCommand> _validator = validator;
        private readonly ILogger<UpdatePurchaseInvoiceCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<PurchaseInvoiceDto> Handle(UpdatePurchaseInvoiceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("UpdatePurchaseInvoice: Initiating update for Invoice {InvoiceId}. Triggered by User {UserId}",
                request.Id, request.CurrentUserId);

            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                _logger.LogWarning("UpdatePurchaseInvoice: Validation failed for Invoice {InvoiceId}. Errors: {Errors}",
                    request.Id, string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage)));

                throw new BadRequestException(validation.Errors.Select(e => e.ErrorMessage).ToList());
            }

            _logger.LogDebug("UpdatePurchaseInvoice: Fetching User {UserId} and Invoice {InvoiceId} in parallel.",
                request.CurrentUserId, request.Id);

            var userTask = _userManager.FindByIdAsync(request.CurrentUserId);
            var invoiceRepo = _unitOfWork.GetRepository<PurchaseInvoice, Guid>();
            var invoiceTask = invoiceRepo.GetAsync(new GetPurchaseInvoiceWithAllIncludesByIdSpec(request.Id), cancellationToken);

            await Task.WhenAll(userTask, invoiceTask);

            var user = await userTask;
            if (user == null)
            {
                _logger.LogError("UpdatePurchaseInvoice: Identity Failure. User {UserId} not found in database.", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }

            var invoice = await invoiceTask;
            if (invoice == null)
            {
                _logger.LogWarning("UpdatePurchaseInvoice: Resource Failure. Invoice {InvoiceId} does not exist.", request.Id);
                throw new NotFoundException<PurchaseInvoice>(request.Id);
            }

            _logger.LogDebug("UpdatePurchaseInvoice: Validating Invoice state for {InvoiceNumber}. Current Status: {Status}, IsPosted: {IsPosted}",
                invoice.InvoiceNumber, invoice.InvoiceStatus, invoice.IsPosted);

            if (invoice.IsPosted || invoice.InvoiceStatus != InvoiceStatus.Draft)
            {
                _logger.LogWarning("UpdatePurchaseInvoice: Business Rule Violation. Attempted to modify immutable Invoice {InvoiceNumber}. Status: {Status}",
                    invoice.InvoiceNumber, invoice.InvoiceStatus);

                throw new BadRequestException("Only Draft invoices can be updated. Posted invoices must be reversed.");
            }

            _logger.LogInformation("UpdatePurchaseInvoice: Opening Database Transaction for Invoice {InvoiceNumber}.", invoice.InvoiceNumber);
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                _logger.LogDebug("UpdatePurchaseInvoice: Applying modifications to Invoice {InvoiceNumber}. ModifiedBy: {UserName}",
                    invoice.InvoiceNumber, user.UserName);

                invoice.Modify(request.Dto.InvoiceNumber, user.Id, request.Dto.Notes);

                _logger.LogTrace("UpdatePurchaseInvoice: Triggering total recalculation for Invoice {InvoiceNumber}.", invoice.InvoiceNumber);
                invoice.RecalculateTotals();

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("UpdatePurchaseInvoice: SUCCESS. Invoice {InvoiceNumber} ({InvoiceId}) updated and committed by {User}.",
                    invoice.InvoiceNumber, invoice.Id, user.UserName);

                return _mapper.Map<PurchaseInvoiceDto>(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "UpdatePurchaseInvoice: CRITICAL FAILURE for Invoice {InvoiceId}. Initiating Transaction Rollback.",
                    request.Id);

                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
