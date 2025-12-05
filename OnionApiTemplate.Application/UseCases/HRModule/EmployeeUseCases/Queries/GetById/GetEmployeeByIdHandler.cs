using Khazen.Application.BaseSpecifications.HRModule.EmployeeSpecifications;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.EmployeeUsecases.Queries.GetById
{
    internal class GetEmployeeByIdHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetEmployeeByIdHandler> logger
    ) : IRequestHandler<GetEmployeeByIdQuery, EmployeeDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetEmployeeByIdHandler> _logger = logger;

        public async Task<EmployeeDetailsDto> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting GetEmployeeByIdHandler for EmployeeId: {EmployeeId}", request.id);

            try
            {
                var repo = _unitOfWork.GetRepository<Employee, Guid>();

                var employee = await repo.GetAsync(new GetEmployeeByIdSpecification(request.id), cancellationToken, true);

                if (employee is null)
                {
                    _logger.LogWarning("Employee not found. EmployeeId: {EmployeeId}", request.id);
                    throw new NotFoundException<Employee>(request.id);
                }

                _logger.LogInformation("Employee retrieved successfully. EmployeeId: {EmployeeId}", employee.Id);

                var dto = _mapper.Map<EmployeeDetailsDto>(employee);

                _logger.LogDebug("Employee mapped to DTO successfully. EmployeeId: {EmployeeId}", employee.Id);

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while retrieving EmployeeId: {EmployeeId}", request.id);
                throw;
            }
        }
    }
}
