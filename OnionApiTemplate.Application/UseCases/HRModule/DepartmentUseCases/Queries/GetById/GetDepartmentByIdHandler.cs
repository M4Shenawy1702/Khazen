using Khazen.Application.BaseSpecifications.HRModule.DepartmentSpecifications;
using Khazen.Application.DOTs.HRModule.Department;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.DepartmentUseCases.Queries.GetById
{
    internal class GetDepartmentByIdHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetDepartmentByIdHandler> logger
    ) : IRequestHandler<GetDepartmentByIdQuery, DepartmentDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetDepartmentByIdHandler> _logger = logger;

        public async Task<DepartmentDetailsDto> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting GetDepartmentByIdHandler for DepartmentId: {DepartmentId}", request.Id);

            try
            {
                var repository = _unitOfWork.GetRepository<Department, Guid>();

                var department = await repository.GetAsync(
                    new GetDepartmentByIdSpecification(request.Id),
                    cancellationToken,
                    true
                );

                if (department is null)
                {
                    _logger.LogWarning("Department not found. DepartmentId: {DepartmentId}", request.Id);
                    throw new NotFoundException<Department>(request.Id);
                }

                _logger.LogInformation("Department found successfully. DepartmentId: {DepartmentId}, Name: {Name}",
                    department.Id, department.Name);

                return _mapper.Map<DepartmentDetailsDto>(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching department by Id: {DepartmentId}", request.Id);
                throw new ApplicationException($"An unexpected error occurred while retrieving department with Id {request.Id}.", ex);
            }
        }
    }
}
