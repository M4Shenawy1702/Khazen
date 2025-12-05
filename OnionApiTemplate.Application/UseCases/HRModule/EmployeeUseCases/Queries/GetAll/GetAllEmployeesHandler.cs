using Khazen.Application.BaseSpecifications.HRModule.EmployeeSpecifications;
using Khazen.Application.Common;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.EmployeeUsecases.Queries.GetAll
{
    internal class GetAllEmployeesHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllEmployeesHandler> logger
    ) : IRequestHandler<GetAllEmployeesQuery, PaginatedResult<EmployeeDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetAllEmployeesHandler> _logger = logger;

        public async Task<PaginatedResult<EmployeeDto>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting GetAllEmployeesHandler with filters: {@QueryParameters}", request.queryParameters);

            try
            {
                var repo = _unitOfWork.GetRepository<Employee, Guid>();
                var employees = await repo.GetAllAsync(new GetAllEmployeesSpecification(request.queryParameters), cancellationToken, true);

                var employeeDtos = _mapper.Map<List<EmployeeDto>>(employees);

                var count = await repo.GetCountAsync(new GetAllEmployeesCountSpecification(request.queryParameters), cancellationToken);

                _logger.LogInformation("Returning {Count} employees (PageIndex: {PageIndex}, PageSize: {PageSize})",
                    employeeDtos.Count, request.queryParameters.PageIndex, request.queryParameters.PageSize);

                return new PaginatedResult<EmployeeDto>(
                    request.queryParameters.PageIndex,
                    request.queryParameters.PageSize,
                    count,
                    employeeDtos
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving employees.");
                throw;
            }
        }
    }
}
