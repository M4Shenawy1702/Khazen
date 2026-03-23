using Khazen.Application.DOTs.SalesModule.Customer;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.CustomerUsecases.Commands.Update
{
    public class UpdateCustomerCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<UpdateCustomerCommand> validator,
        ILogger<UpdateCustomerCommandHandler> logger,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<UpdateCustomerCommand, CustomerDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdateCustomerCommand> _validator = validator;
        private readonly ILogger<UpdateCustomerCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<CustomerDto> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for UpdateCustomerCommand: {@Errors}", validationResult.Errors);
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var user = await _userManager.FindByNameAsync(request.CurrentUserId);
            if (user == null)
            {
                _logger.LogError("Identity User {UserId} not found.", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }

            try
            {
                var repo = _unitOfWork.GetRepository<Customer, Guid>();
                var customer = await repo.GetByIdAsync(request.Id, cancellationToken);
                if (customer is null)
                {
                    _logger.LogWarning("Customer {CustomerId} not found", request.Id);
                    throw new NotFoundException<Customer>(request.Id);
                }

                _logger.LogInformation("Updating customer {CustomerId} - current name: {CustomerName}", customer.Id, customer.Name);

                var duplicate = await repo.AnyAsync(c => c.Name == request.Dto.Name && c.Id != request.Id, cancellationToken);
                if (duplicate)
                {
                    _logger.LogWarning("Duplicate customer name detected: {CustomerName}", request.Dto.Name);
                    throw new BadRequestException($"Another customer with name '{request.Dto.Name}' already exists.");
                }

                customer.Modify(
                    request.Dto.Name,
                    request.Dto.Address,
                    request.Dto.CustomerType,
                    user.Id);

                repo.Update(customer);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Customer {CustomerId} updated successfully. New name: {CustomerName}", customer.Id, customer.Name);

                return _mapper.Map<CustomerDto>(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update customer {CustomerId}", request.Id);
                throw;
            }
        }
    }
}
