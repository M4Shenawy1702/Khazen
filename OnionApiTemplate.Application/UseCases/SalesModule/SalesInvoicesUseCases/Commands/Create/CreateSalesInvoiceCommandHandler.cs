using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.Interfaces.ISalesModule.ISalesInvoiceServices;
using Khazen.Application.DOTs.SalesModule.SalesInvoicesDots;
using Khazen.Application.Specification.SalesModule.CustomerSpecifications;
using Khazen.Application.Specification.SalesModule.SalesOrdersSpecifications;
using Khazen.Application.UseCases.SalesModule.SalesInvoicesUseCases.Commands.Create;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

internal class CreateSalesInvoiceCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IValidator<CreateSalesInvoiceCommand> validator,
    ISalesInvoiceService salesInvoiceService,
    IJournalEntryService journalEntryService,
    ILogger<CreateSalesInvoiceCommandHandler> logger,
    UserManager<ApplicationUser> userManager)
    : IRequestHandler<CreateSalesInvoiceCommand, SalesInvoiceDto>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IValidator<CreateSalesInvoiceCommand> _validator = validator;
    private readonly ISalesInvoiceService _salesInvoiceService = salesInvoiceService;
    private readonly IJournalEntryService _journalEntryService = journalEntryService;
    private readonly ILogger<CreateSalesInvoiceCommandHandler> _logger = logger;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<SalesInvoiceDto> Handle(CreateSalesInvoiceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Starting CreateSalesInvoice. SalesOrderId: {OrderId}, CustomerId: {CustomerId}",
            request.Dto.SalesOrderId,
            request.Dto.CustomerId
        );

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("CreateSalesInvoice validation failed: {@Errors}", validationResult.Errors);
            throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        }

        var user = await _userManager.FindByNameAsync(request.CreatedBy);
        if (user is null)
        {
            _logger.LogInformation("User not found. UserName: {CreatedBy}", request.CreatedBy);
            throw new NotFoundException<ApplicationUser>(request.CreatedBy);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();
            _logger.LogDebug("Fetching customer {CustomerId}", request.Dto.CustomerId);

            var customer = await customerRepo.GetAsync(
                new GetCustomerByIdSpecification(request.Dto.CustomerId),
                cancellationToken);
            if (customer is null)
            {
                _logger.LogWarning("Customer {CustomerId} not found", request.Dto.CustomerId);
                throw new NotFoundException<Customer>(request.Dto.CustomerId);
            }

            var salesOrdersRepo = _unitOfWork.GetRepository<SalesOrder, Guid>();
            _logger.LogDebug("Fetching sales order {OrderId} with includes", request.Dto.SalesOrderId);

            var salesOrder = await salesOrdersRepo.GetAsync(
                new GetSalesOrderWithIncludesSpecifications(request.Dto.SalesOrderId),
                cancellationToken
            );
            if (salesOrder is null)
            {
                _logger.LogWarning("Sales order {OrderId} not found", request.Dto.SalesOrderId);
                throw new NotFoundException<SalesOrder>(request.Dto.SalesOrderId);
            }

            _logger.LogInformation(
                "Creating sales invoice for Order {OrderId}, Customer {CustomerId}",
                salesOrder.Id,
                customer.Id
            );

            var salesInvoice = await _salesInvoiceService.CreateSalesInvoice(salesOrder, customer, request.Dto, request.CreatedBy, cancellationToken);

            _logger.LogDebug(
                "Sales invoice built in service. InvoiceNumber: {Number}, SubTotal: {Sub}, GrandTotal: {Total}",
                salesInvoice.InvoiceNumber,
                salesInvoice.SubTotal,
                salesInvoice.GrandTotal
            );

            var invoiceRepo = _unitOfWork.GetRepository<SalesInvoice, Guid>();
            _logger.LogDebug("Saving new sales invoice {InvoiceId} to the database", salesInvoice.Id);

            await invoiceRepo.AddAsync(salesInvoice, cancellationToken);

            salesOrder.CalculateTotals();
            _logger.LogDebug("Updating sales order totals for order {OrderId}", salesOrder.Id);

            salesOrdersRepo.Update(salesOrder);

            _logger.LogInformation("Saving all changes to database...");
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Committing transaction...");
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation(
                "Sales invoice {InvoiceId} created successfully for Order {OrderId}",
                salesInvoice.Id,
                salesOrder.Id
            );

            return _mapper.Map<SalesInvoiceDto>(salesInvoice);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error creating sales invoice for Order {OrderId}. Rolling back transaction.",
                request.Dto.SalesOrderId
            );

            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
