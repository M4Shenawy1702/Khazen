using Khazen.Application.BaseSpecifications.HRModule.DepartmentSpecifications;
using Khazen.Application.Common;
using Khazen.Application.DOTs.HRModule.Department;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.DepartmentUseCases.Queries.GetAll
{
    internal class GetAllDepartmentQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllDepartmentQueryHandler> logger
    ) : IRequestHandler<GetAllDepartmentQuery, PaginatedResult<DepartmentDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetAllDepartmentQueryHandler> _logger = logger;

        public async Task<PaginatedResult<DepartmentDto>> Handle(GetAllDepartmentQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching paginated departments. Page: {PageIndex}", request.QueryParameters.PageIndex);

            var repo = _unitOfWork.GetRepository<Department, Guid>();

            var departments = await repo.GetAllAsync(
                new GetAllDepartmentSpecification(request.QueryParameters),
                cancellationToken,
                asNoTracking: true
            );

            var count = await repo.GetCountAsync(
                new GetAllDepartmentCountSpecification(request.QueryParameters),
                cancellationToken,
                asNoTracking: true
            );

            var departmentsDtos = _mapper.Map<IEnumerable<DepartmentDto>>(departments);

            _logger.LogInformation("Successfully retrieved {Count} departments", departments.Count());

            return new PaginatedResult<DepartmentDto>(
                request.QueryParameters.PageIndex,
                request.QueryParameters.PageSize,
                count,
                departmentsDtos
            );
        }
    }
}