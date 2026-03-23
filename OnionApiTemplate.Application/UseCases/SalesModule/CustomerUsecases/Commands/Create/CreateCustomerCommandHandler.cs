using Khazen.Application.Common.Interfaces.Authentication;
using Khazen.Application.DOTs.SalesModule.Customer;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.CustomerUsecases.Commands.Create
{
    internal class CreateCustomerCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateCustomerCommand> validator,
        IUserRegistrationService userRegistrationService,
        ILogger<CreateCustomerCommandHandler> logger,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<CreateCustomerCommand, CustomerDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CreateCustomerCommand> _validator = validator;
        private readonly IUserRegistrationService _userRegistrationService = userRegistrationService;
        private readonly ILogger<CreateCustomerCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting customer creation process for {CustomerName}", request.Dto.Name);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for customer {CustomerName}: {@Errors}",
                    request.Dto.Name, validationResult.Errors.Select(e => e.ErrorMessage));
                throw new BadRequestException(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }
            var user = await _userManager.FindByNameAsync(request.CurrentUserId);
            if (user is null)
            {
                _logger.LogInformation("User not found. CurrentUserId: {CurrentUserId}", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var customerRepo = _unitOfWork.GetRepository<Customer, Guid>();

                var existingCustomer = await customerRepo.AnyAsync(c => c.Name == request.Dto.Name, cancellationToken);
                if (existingCustomer)
                {
                    _logger.LogWarning("Duplicate customer name detected: {CustomerName}", request.Dto.Name);
                    throw new BadRequestException("Customer with the same name already exists.");
                }

                _logger.LogInformation("Registering user for customer {CustomerName}", request.Dto.Name);
                var userId = await _userRegistrationService.RegisterCustomerUserAsync(request.Dto, user.Id, cancellationToken);

                var customer = new Customer(
                    request.Dto.Name,
                    request.Dto.Address,
                    userId,
                    request.Dto.CustomerType,
                    user.Id);

                await customerRepo.AddAsync(customer, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Customer {CustomerName} created successfully with ID {CustomerId}",
                    customer.Name, customer.Id);

                return _mapper.Map<CustomerDto>(customer);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Failed to create customer {CustomerName}", request.Dto.Name);
                throw;
            }
        }
    }
}
