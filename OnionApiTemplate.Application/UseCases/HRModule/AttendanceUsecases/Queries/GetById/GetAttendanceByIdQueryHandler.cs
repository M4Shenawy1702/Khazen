using Khazen.Application.BaseSpecifications.HRModule.AttendanceSpecifications;
using Khazen.Application.DOTs.HRModule.AttendaceDtos;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.AttendanceUsecases.Queries.GetById
{
    internal class GetAttendanceByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<GetAttendanceByIdQueryHandler> logger
    ) : IRequestHandler<GetAttendanceByIdQuery, AttendanceDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetAttendanceByIdQueryHandler> _logger = logger;

        public async Task<AttendanceDto> Handle(GetAttendanceByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching attendance record with Id: {AttendanceId}", request.Id);

            try
            {
                var attendanceRepo = _unitOfWork.GetRepository<AttendanceRecord, Guid>();

                var attendance = await attendanceRepo.GetAsync(
                    new GetAttendanceRecordByIdSpecification(request.Id),
                    cancellationToken
                );

                if (attendance is null)
                {
                    _logger.LogWarning("Attendance record not found for Id: {AttendanceId}", request.Id);
                    throw new NotFoundException<AttendanceRecord>(request.Id);
                }

                _logger.LogInformation("Attendance record retrieved successfully for Id: {AttendanceId}", request.Id);

                return _mapper.Map<AttendanceDto>(attendance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching attendance record with Id: {AttendanceId}", request.Id);
                throw;
            }
        }
    }
}
