using Khazen.Application.Common.Interfaces;
using Khazen.Application.DOTs.SalesModule.SalesInvoicesDots;
using Khazen.Application.Specification.SalesModule.SalesInvoicesSpecifications;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Commands.Post
{
    internal class PostSalesInvoiceCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<PostSalesInvoiceCommand> validator,
        IJournalEntryService journalEntryService,
        ILogger<PostSalesInvoiceCommandHandler> logger,
        UserManager<ApplicationUser> userManager
    ) : IRequestHandler<PostSalesInvoiceCommand, SalesInvoiceDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<PostSalesInvoiceCommand> _validator = validator;
        private readonly IJournalEntryService _journalEntryService = journalEntryService;
        private readonly ILogger<PostSalesInvoiceCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<SalesInvoiceDetailsDto> Handle(PostSalesInvoiceCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting PostSalesInvoice. InvoiceId: {InvoiceId}, PostedBy: {User}", request.Id, request.PostedBy);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("PostSalesInvoice validation failed: {@Errors}", validationResult.Errors);
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var user = await _userManager.FindByNameAsync(request.PostedBy);
            if (user is null)
            {
                _logger.LogWarning("User not found while posting sales invoice. UserName: {UserName}", request.PostedBy);
                throw new NotFoundException<ApplicationUser>(request.PostedBy);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var invoiceRepo = _unitOfWork.GetRepository<SalesInvoice, Guid>();

                var salesInvoice = await invoiceRepo.GetAsync(new GetSalesInvoiceWithIncludesSpecifications(request.Id), cancellationToken);
                if (salesInvoice is null)
                {
                    _logger.LogWarning("Sales invoice not found. InvoiceId: {InvoiceId}", request.Id);
                    throw new NotFoundException<SalesInvoice>(request.Id);
                }

                if (salesInvoice.IsPosted)
                {
                    _logger.LogWarning("Invoice {InvoiceId} is already posted", salesInvoice.Id);
                    throw new BadRequestException($"Invoice {salesInvoice.Id} is already posted.");
                }

                if (salesInvoice.IsVoided)
                {
                    _logger.LogWarning("Cannot post a voided invoice {InvoiceId}", salesInvoice.Id);
                    throw new BadRequestException($"Cannot post a voided invoice {salesInvoice.Id}.");
                }

                _logger.LogInformation("Generating journal entry for invoice {InvoiceId}", salesInvoice.Id);
                await _journalEntryService.CreateSalesInvoiceJournalAsync(salesInvoice, user.UserName!, cancellationToken);

                _logger.LogDebug("Marking invoice {InvoiceId} as posted by {User}", salesInvoice.Id, request.PostedBy);
                salesInvoice.MarkAsPosted(request.PostedBy);

                _logger.LogDebug("Calculating totals and updating invoice status for {InvoiceId}", salesInvoice.Id);
                salesInvoice.CalculateTotals();
                salesInvoice.UpdateInvoiceStatus();

                invoiceRepo.Update(salesInvoice);

                _logger.LogInformation("Saving changes and committing transaction for invoice {InvoiceId}", salesInvoice.Id);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Invoice {InvoiceId} posted successfully.", salesInvoice.Id);

                return _mapper.Map<SalesInvoiceDetailsDto>(salesInvoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error posting sales invoice {InvoiceId}. Rolling back transaction.", request.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
