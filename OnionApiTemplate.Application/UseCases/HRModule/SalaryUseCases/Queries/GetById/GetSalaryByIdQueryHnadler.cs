using Khazen.Application.BaseSpecifications.HRModule.SalarySepcifications;
using Khazen.Application.DOTs.HRModule.SalaryDots;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.SalaryUseCases.Queries.GetById
{
    internal class GetSalaryByIdQueryHnadler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<GetSalaryByIdQueryHnadler> logger
    ) : IRequestHandler<GetSalaryByIdQuery, SalaryDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetSalaryByIdQueryHnadler> _logger = logger;

        public async Task<SalaryDetailsDto> Handle(GetSalaryByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching Salary record with Id: {SalaryId}", request.Id);

            var repo = _unitOfWork.GetRepository<Salary, Guid>();

            var salary = await repo.GetAsync(
                new GetSalaryByIdSpecification(request.Id),
                cancellationToken,
                 true
            );

            if (salary == null)
            {
                _logger.LogWarning("Salary record with Id {SalaryId} not found", request.Id);
                throw new NotFoundException<Salary>(request.Id);
            }

            _logger.LogInformation("Salary {SalaryId} retrieved successfully. Mapping to DTO.", request.Id);

            return _mapper.Map<SalaryDetailsDto>(salary);
        }
    }
}
