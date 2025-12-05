using Khazen.Application.BaseSpecifications.HRModule.AttendanceSpecifications;
using Khazen.Application.Common;
using Khazen.Application.DOTs.HRModule.AttendaceDtos;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.AttendanceUsecases.Queries.GetAll
{
    internal class GetAllAttendanceQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAllAttendanceQueryHandler> logger,
        IValidator<GetAllAttendanceQuery> validator
    ) : IRequestHandler<GetAllAttendanceQuery, PaginatedResult<AttendanceDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetAllAttendanceQueryHandler> _logger = logger;
        private readonly IValidator<GetAllAttendanceQuery> _validator = validator;

        public async Task<PaginatedResult<AttendanceDto>> Handle(GetAllAttendanceQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all attendance records. PageIndex: {PageIndex}, PageSize: {PageSize}",
                request.QueryParameters.PageIndex, request.QueryParameters.PageSize);

            try
            {

                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed . Errors: {Errors}",
                                              string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }
                var attendanceRepo = _unitOfWork.GetRepository<AttendanceRecord, Guid>();

                var attendances = await attendanceRepo.GetAllAsync(
                    new GetAllAttendanceSpecification(request.QueryParameters),
                    cancellationToken,
                    asNoTracking: true
                );

                var count = await attendanceRepo.GetCountAsync(
                    new GetAllAttendanceCountSpecification(request.QueryParameters),
                    cancellationToken
                );

                _logger.LogDebug("Retrieved {Count} attendance records from database.", count);

                var data = _mapper.Map<IEnumerable<AttendanceDto>>(attendances);

                _logger.LogInformation("Successfully fetched {Count} attendance records.", data.Count());

                return new PaginatedResult<AttendanceDto>(
                    request.QueryParameters.PageIndex,
                    request.QueryParameters.PageSize,
                    count,
                    data
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching attendance records.");
                throw;
            }
        }
    }
}
