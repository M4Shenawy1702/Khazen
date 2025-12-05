using Khazen.Application.DOTs.SalesModule.Customer;
using Khazen.Application.Specification.SalesModule.CustomerSpecifications;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.CustomerUsecases.Queries.GetById
{
    public class GetCustomerByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<GetCustomerByIdQuery> validator,
        ILogger<GetCustomerByIdQueryHandler> logger)
        : IRequestHandler<GetCustomerByIdQuery, CustomerDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<GetCustomerByIdQuery> _validator = validator;
        private readonly ILogger<GetCustomerByIdQueryHandler> _logger = logger;

        public async Task<CustomerDetailsDto> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetCustomerByIdQuery for CustomerId: {CustomerId}", request.Id);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for GetCustomerByIdQuery: {Errors}",
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var repo = _unitOfWork.GetRepository<Customer, Guid>();
            var entity = await repo.GetAsync(new GetCustomerByIdWithIncludeSpec(request.Id), cancellationToken);

            if (entity is null)
            {
                _logger.LogWarning("Customer with ID {CustomerId} not found.", request.Id);
                throw new NotFoundException<Customer>(request.Id);
            }

            _logger.LogInformation("Customer with ID {CustomerId} retrieved successfully.", request.Id);

            return _mapper.Map<CustomerDetailsDto>(entity);
        }
    }
}
