using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.CustomerUsecases.Commands.Delete
{
    public class ToggleCustomerCommandHandler(
        IUnitOfWork unitOfWork,
        IValidator<ToggleCustomerCommand> validator,
        ILogger<ToggleCustomerCommandHandler> logger)
                : IRequestHandler<ToggleCustomerCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IValidator<ToggleCustomerCommand> _validator = validator;
        private readonly ILogger<ToggleCustomerCommandHandler> _logger = logger;

        public async Task<bool> Handle(ToggleCustomerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ToggleCustomerCommand started for CustomerId: {Id}", request.Id);

            var result = await _validator.ValidateAsync(request, cancellationToken);
            if (!result.IsValid)
            {
                _logger.LogWarning("Validation failed for CustomerId: {Id}. Errors: {Errors}",
                    request.Id,
                    string.Join(", ", result.Errors.Select(e => e.ErrorMessage)));

                throw new BadRequestException(result.Errors.Select(x => x.ErrorMessage));
            }

            var repo = _unitOfWork.GetRepository<Customer, Guid>();
            var customer = await repo.GetByIdAsync(request.Id, cancellationToken);

            if (customer is null)
            {
                _logger.LogWarning("Customer not found for CustomerId: {Id}", request.Id);
                throw new NotFoundException<Customer>(request.Id);
            }

            customer.Toggle(request.ModifiedBy);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("ToggleCustomerCommand completed for CustomerId: {Id}", request.Id);

            return true;
        }
    }
}
