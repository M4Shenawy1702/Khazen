using Khazen.Application.BaseSpecifications.HRModule.DepartmentSpecifications;
using Khazen.Application.DOTs.HRModule.Department;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.DepartmentUseCases.Queries.GetById
{
    internal class GetDepartmentByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetDepartmentByIdQueryHandler> logger
    ) : IRequestHandler<GetDepartmentByIdQuery, DepartmentDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetDepartmentByIdQueryHandler> _logger = logger;

        public async Task<DepartmentDetailsDto> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Fetching department details for ID: {DepartmentId}", request.Id);

            var repository = _unitOfWork.GetRepository<Department, Guid>();

            var department = await repository.GetAsync(
               new GetDepartmentByIdSpecification(request.Id),
               cancellationToken,
               asNoTracking: true
           );

            if (department is null)
            {
                _logger.LogWarning("Department {DepartmentId} not found.", request.Id);
                throw new NotFoundException<Department>(request.Id);
            }

            _logger.LogInformation("Successfully retrieved department: {Name} ({Id})",
                department.Name, department.Id);

            return _mapper.Map<DepartmentDetailsDto>(department);
        }
    }
}