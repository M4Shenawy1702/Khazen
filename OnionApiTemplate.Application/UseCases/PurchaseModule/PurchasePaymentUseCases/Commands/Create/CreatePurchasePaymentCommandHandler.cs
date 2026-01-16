using Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchasePaymentServices;
using Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots;
using Khazen.Application.Specification.PurchaseModule.PurchaseInvoiceSpecifications;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.PurchaseModule.PurchasePaymentUseCases.Commands.Create
{
    public class CreatePurchasePaymentCommandHandler(
        IValidator<CreatePurchasePaymentCommand> validator,
        IPurchasePaymentDomainService paymentService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreatePurchasePaymentCommandHandler> logger,
         UserManager<ApplicationUser> userManager)
        : IRequestHandler<CreatePurchasePaymentCommand, PurchasePaymentDto>
    {
        private readonly IValidator<CreatePurchasePaymentCommand> _validator = validator;
        private readonly IPurchasePaymentDomainService _paymentService = paymentService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CreatePurchasePaymentCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<PurchasePaymentDto> Handle(CreatePurchasePaymentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Start CreatePurchasePayment | InvoiceId={InvoiceId}, Amount={Amount}, Method={Method}",
                request.Dto.PurchaseInvoiceId, request.Dto.Amount, request.Dto.Method);

            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = string.Join(" | ", validation.Errors.Select(e => e.ErrorMessage));
                _logger.LogWarning("Validation failed for CreatePurchasePayment | Errors={Errors}", errors);

                throw new BadRequestException(validation.Errors.Select(e => e.ErrorMessage).ToList());
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug("Transaction started for CreatePurchasePayment");

            try
            {
                _logger.LogDebug(
                    "Fetching PurchaseInvoice with includes | InvoiceId={InvoiceId}",
                    request.Dto.PurchaseInvoiceId);

                var invoice = await _unitOfWork
                    .GetRepository<PurchaseInvoice, Guid>()
                    .GetAsync(new GetPurchaseInvoiceWithAllIncludesByIdSpec(request.Dto.PurchaseInvoiceId), cancellationToken);

                if (invoice is null)
                {
                    _logger.LogError(
                        "PurchaseInvoice not found | InvoiceId={InvoiceId}",
                        request.Dto.PurchaseInvoiceId);

                    throw new NotFoundException<PurchaseInvoice>(request.Dto.PurchaseInvoiceId);
                }

                _logger.LogDebug(
                    "Invoice found | InvoiceId={InvoiceId}, CurrentPaid={Paid}, Total={Total}, Status={Status}",
                    invoice.Id, invoice.PaidAmount, invoice.TotalAmount, invoice.PaymentStatus);

                _logger.LogInformation(
                    "Calling PurchasePaymentDomainService.CreatePaymentAsync | InvoiceId={InvoiceId}",
                    invoice.Id);

                var user = await _userManager.FindByNameAsync(request.CurrentUserId);
                if (user is null)
                {
                    _logger.LogInformation("User not found. UserId: {ModifiedBy}", request.CurrentUserId);
                    throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
                }

                var payment = await _paymentService.CreatePaymentAsync(
                    invoice,
                    user.UserName!,
                    request.Dto,
                    cancellationToken);

                _logger.LogInformation(
                    "Payment created successfully | PaymentId={PaymentId}, InvoiceId={InvoiceId}, Amount={Amount}",
                    payment.Id, invoice.Id, request.Dto.Amount);

                _unitOfWork.GetRepository<PurchaseInvoice, Guid>().Update(invoice);

                _logger.LogDebug(
                    "Invoice updated after payment | InvoiceId={InvoiceId}, NewPaid={Paid}, Remaining={Remaining}, Status={Status}",
                    invoice.Id, invoice.PaidAmount, invoice.RemainingAmount, invoice.PaymentStatus);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation(
                    "Transaction committed | InvoiceId={InvoiceId}, PaymentId={PaymentId}",
                    invoice.Id, payment.Id);

                return _mapper.Map<PurchasePaymentDto>(payment);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                _logger.LogError(
                    ex,
                    "Error occurred during CreatePurchasePayment | InvoiceId={InvoiceId}, Amount={Amount}, Method={Method}",
                    request.Dto.PurchaseInvoiceId, request.Dto.Amount, request.Dto.Method);

                throw;
            }
        }
    }
}
