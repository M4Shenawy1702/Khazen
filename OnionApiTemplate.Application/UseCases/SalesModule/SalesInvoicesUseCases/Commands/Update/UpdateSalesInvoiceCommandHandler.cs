using Khazen.Application.DOTs.SalesModule.SalesInvoicesDots;
using Khazen.Application.Specification.SalesModule.CustomerSpecifications;
using Khazen.Application.Specification.SalesModule.SalesInvoicesSpecifications;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Commands.Update
{
    internal class UpdateSalesInvoiceCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<UpdateSalesInvoiceCommand> validator,
        ILogger<UpdateSalesInvoiceCommandHandler> logger,
        UserManager<ApplicationUser> userManager
    ) : IRequestHandler<UpdateSalesInvoiceCommand, SalesInvoiceDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdateSalesInvoiceCommand> _validator = validator;
        private readonly ILogger<UpdateSalesInvoiceCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<SalesInvoiceDto> Handle(UpdateSalesInvoiceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting UpdateSalesInvoice. InvoiceId: {InvoiceId}, ModifiedBy: {User}", request.Id, request.ModifiedBy);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed: {@Errors}", validationResult.Errors);
                throw new BadRequestException(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }

            var user = await _userManager.FindByNameAsync(request.ModifiedBy);
            if (user is null)
            {
                _logger.LogWarning("User not found while updating invoice. UserName: {UserName}", request.ModifiedBy);
                throw new NotFoundException<ApplicationUser>(request.ModifiedBy);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var invoiceRepo = _unitOfWork.GetRepository<SalesInvoice, Guid>();
                var salesInvoice = await invoiceRepo.GetAsync(new GetSalesInvoiceWithIncludesSpecifications(request.Id), cancellationToken);
                if (salesInvoice is null)
                {
                    _logger.LogWarning("Invoice {InvoiceId} not found", request.Id);
                    throw new NotFoundException<SalesInvoice>(request.Id);
                }

                if (salesInvoice.IsPosted || salesInvoice.InvoiceStatus == InvoiceStatus.Paid)
                {
                    _logger.LogWarning("Cannot update invoice {InvoiceId} because it is posted or paid", salesInvoice.Id);
                    throw new BadRequestException($"Invoice {salesInvoice.Id} cannot be updated because it is already posted or paid.");
                }

                var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
                _logger.LogDebug("Fetching customer {CustomerId}", request.Dto.CustomerId);

                var customer = await customerRepo.GetAsync(new GetCustomerByIdSpecification(request.Dto.CustomerId), cancellationToken);
                if (customer is null)
                {
                    _logger.LogWarning("Customer {CustomerId} not found", request.Dto.CustomerId);
                    throw new NotFoundException<Customer>(request.Dto.CustomerId);
                }

                _logger.LogDebug("Updating invoice {InvoiceId}", salesInvoice.Id);
                salesInvoice.Modify(request.Dto.InvoiceDate, request.Dto.Notes, request.Dto.CustomerId, request.ModifiedBy);

                salesInvoice.UpdateInvoiceStatus();

                invoiceRepo.Update(salesInvoice);

                _logger.LogInformation("Saving changes for invoice {InvoiceId}", salesInvoice.Id);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Invoice {InvoiceId} updated successfully by {User}.", salesInvoice.Id, request.ModifiedBy);
                return _mapper.Map<SalesInvoiceDto>(salesInvoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating invoice {InvoiceId}. Rolling back.", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
