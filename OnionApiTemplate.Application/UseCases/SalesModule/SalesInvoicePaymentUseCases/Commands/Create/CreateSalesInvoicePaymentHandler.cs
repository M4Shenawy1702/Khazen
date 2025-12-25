using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.Interfaces.ISalesModule;
using Khazen.Application.Common.Interfaces.ISalesModule.ISalesInvoicePaymentServices;
using Khazen.Application.DOTs.SalesModule.SalesOrderPaymentDtos;
using Khazen.Application.Specification.SalesModule.SalesInvoicesSpecifications;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Entities.ConfigurationModule;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.SalesInvoicePaymentUseCases.Commands.Create
{
    internal class CreateSalesInvoicePaymentHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISafeTransactionService safeTransactionService,
    IJournalEntryService journalEntryService,
    ISalesPaymentDomainService paymentDomainService,
    IValidator<CreateSalesInvoicePaymentCommand> validator,
    ILogger<CreateSalesInvoicePaymentHandler> logger, UserManager<ApplicationUser> userManager
    ) : IRequestHandler<CreateSalesInvoicePaymentCommand, SalesInvoicePaymentDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ISafeTransactionService _safeTransactionService = safeTransactionService;
        private readonly IJournalEntryService _journalEntryService = journalEntryService;
        private readonly ISalesPaymentDomainService _paymentDomainService = paymentDomainService;
        private readonly IValidator<CreateSalesInvoicePaymentCommand> _validator = validator;
        private readonly ILogger<CreateSalesInvoicePaymentHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<SalesInvoicePaymentDto> Handle(CreateSalesInvoicePaymentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting sales invoice payment creation for InvoiceId: {InvoiceId}", request.Dto.SalesInvoiceId);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for sales invoice payment request: {Errors}", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var user = await _userManager.FindByNameAsync(request.CreatedBy);
            if (user is null)
            {
                _logger.LogInformation("User not found. Username: {CreatedBy}", request.CreatedBy);
                throw new NotFoundException<ApplicationUser>(request.CreatedBy);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var invoiceRepo = _unitOfWork.GetRepository<SalesInvoice, Guid>();
                var invoice = await invoiceRepo.GetAsync(
                    new GetSalesInvoiceWithIncludesSpecifications(request.Dto.SalesInvoiceId),
                    cancellationToken
                ) ?? throw new NotFoundException<SalesInvoice>(request.Dto.SalesInvoiceId);

                _logger.LogInformation("Retrieved invoice {InvoiceNumber} for payment", invoice.InvoiceNumber);

                var systemSettings = await _unitOfWork
                    .GetRepository<SystemSetting, int>()
                    .GetAllAsync(new GetAllSystemSettingsSpec(), cancellationToken);

                _paymentDomainService.ValidatePaymentAmount(request.Dto.Amount, invoice);

                var payment = _paymentDomainService.CreatePayment(invoice, request);

                _logger.LogInformation("Created payment object with Id: {PaymentId}", payment.Id);

                var journalEntry = await _journalEntryService.CreateSalesInvoicePaymentJournalAsync(
                    invoice, user.UserName!, request, systemSettings, cancellationToken);

                _logger.LogInformation("Created journal entry with Id: {JournalEntryId}", journalEntry.Id);

                await _safeTransactionService.ApplySalesPaymentTransactionAsync(
                    payment, journalEntry, request, invoice.InvoiceNumber, systemSettings, cancellationToken
                );

                _logger.LogInformation("Applied safe transaction for payment Id: {PaymentId}", payment.Id);

                await _unitOfWork.GetRepository<JournalEntry, Guid>().AddAsync(journalEntry, cancellationToken);
                await _unitOfWork.GetRepository<SalesInvoicePayment, Guid>().AddAsync(payment, cancellationToken);

                invoice.UpdateInvoiceStatus();
                invoiceRepo.Update(invoice);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Sales invoice payment created successfully for InvoiceId: {InvoiceId}", invoice.Id);

                return _mapper.Map<SalesInvoicePaymentDto>(payment);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Concurrency exception occurred while creating payment for InvoiceId: {InvoiceId}", request.Dto.SalesInvoiceId);
                throw new BadRequestException("The Safe was updated by another transaction. Please try again.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Exception occurred while creating payment for InvoiceId: {InvoiceId}", request.Dto.SalesInvoiceId);
                throw;
            }
        }
    }
}
