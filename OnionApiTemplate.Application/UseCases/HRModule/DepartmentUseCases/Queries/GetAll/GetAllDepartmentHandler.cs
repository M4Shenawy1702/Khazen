using Khazen.Application.BaseSpecifications.HRModule.DepartmentSpecifications;
using Khazen.Application.Common;
using Khazen.Application.DOTs.HRModule.Department;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.DepartmentUseCases.Queries.GetAll
{
    internal class GetAllDepartmentHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllDepartmentHandler> logger
    ) : IRequestHandler<GetAllDepartmentQuery, PaginatedResult<DepartmentDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetAllDepartmentHandler> _logger = logger;

        public async Task<PaginatedResult<DepartmentDto>> Handle(GetAllDepartmentQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting GetAllDepartmentHandler with PageIndex: {PageIndex}, PageSize: {PageSize}",
                request.QueryParameters.PageIndex, request.QueryParameters.PageSize);

            try
            {
                var repo = _unitOfWork.GetRepository<Department, Guid>();

                var departments = await repo.GetAllAsync(
                    new GetAllDepartmentSpecification(request.QueryParameters),
                    cancellationToken,
                    true
                );

                var count = await repo.GetCountAsync(
                    new GetAllDepartmentCountSpecification(request.QueryParameters),
                    cancellationToken
                );

                var departmentsDtos = _mapper.Map<List<DepartmentDto>>(departments);

                _logger.LogInformation("Fetched {Count} departments successfully", departments.Count());

                var result = new PaginatedResult<DepartmentDto>(
                    request.QueryParameters.PageIndex,
                    request.QueryParameters.PageSize,
                    count,
                    departmentsDtos
                );

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all departments");
                throw new ApplicationException("An unexpected error occurred while retrieving departments.", ex);
            }
        }
    }
}
