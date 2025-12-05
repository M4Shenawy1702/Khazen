using Khazen.Application.Common;
using Khazen.Application.DOTs.SalesModule.Customer;
using Khazen.Application.Specification.SalesModule.CustomerSpecifications;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.CustomerUsecases.Queries.GetAll
{
    public class GetAllCustomersQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<GetAllCustomersQuery> validator,
        ILogger<GetAllCustomersQueryHandler> logger)
        : IRequestHandler<GetAllCustomersQuery, PaginatedResult<CustomerDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<GetAllCustomersQuery> _validator = validator;
        private readonly ILogger<GetAllCustomersQueryHandler> _logger = logger;

        public async Task<PaginatedResult<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAllCustomersQuery: PageIndex={PageIndex}, PageSize={PageSize}",
                request.QueryParameters.PageIndex, request.QueryParameters.PageSize);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for GetAllCustomersQuery: {Errors}",
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var repo = _unitOfWork.GetRepository<Customer, Guid>();

            var customers = await repo.GetAllAsync(new GetAllCustomersSpec(request.QueryParameters), cancellationToken);
            var data = _mapper.Map<List<CustomerDto>>(customers);

            var count = await repo.GetCountAsync(new GetCustomersCountSpec(request.QueryParameters), cancellationToken);

            _logger.LogInformation("Retrieved {Count} customers (Page {PageIndex})", data.Count, request.QueryParameters.PageIndex);

            return new PaginatedResult<CustomerDto>(
                request.QueryParameters.PageIndex,
                request.QueryParameters.PageSize,
                count,
                data
            );
        }
    }
}
