using Khazen.Application.BaseSpecifications.HRModule.AttendanceSpecifications;
using Khazen.Application.DOTs.HRModule.AttendaceDtos;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.CheckOut
{
    internal class CheckOutCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CheckOutCommandHandler> logger
    ) : IRequestHandler<CheckOutCommand, AttendanceDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CheckOutCommandHandler> _logger = logger;

        public async Task<AttendanceDto> Handle(CheckOutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Starting CheckOutCommandHandler for AttendanceId: {AttendanceId}", request.AttendanceId);

                var attendanceRepo = _unitOfWork.GetRepository<AttendanceRecord, Guid>();

                var attendance = await attendanceRepo.GetAsync(
                    new GetAttendanceRecordByIdSpecification(request.AttendanceId),
                    cancellationToken
                );
                if (attendance is null)
                {
                    _logger.LogWarning("Attendance record {AttendanceId} not found.", request.AttendanceId);
                    throw new NotFoundException<AttendanceRecord>(request.AttendanceId);
                }

                _logger.LogInformation("Attendance record {AttendanceId} retrieved successfully.", request.AttendanceId);

                if (!attendance.CheckInTime.HasValue)
                {
                    _logger.LogWarning("Cannot check out AttendanceId: {AttendanceId} because employee never checked in.", request.AttendanceId);
                    throw new BadRequestException(["Employee has not checked in yet."]);
                }

                if (attendance.CheckOutTime.HasValue)
                {
                    _logger.LogWarning("Employee has already checked out for AttendanceId: {AttendanceId}", request.AttendanceId);
                    throw new BadRequestException(["Employee has already checked out."]);
                }

                attendance.CheckOut(TimeOnly.FromDateTime(DateTime.UtcNow));
                _logger.LogInformation("Employee checked out successfully at {CheckOutTime} for AttendanceId: {AttendanceId}.",
                    attendance.CheckOutTime, request.AttendanceId);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Attendance record {AttendanceId} saved successfully.", request.AttendanceId);

                return _mapper.Map<AttendanceDto>(attendance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during checkout for AttendanceId: {AttendanceId}", request.AttendanceId);
                throw;
            }
        }
    }
}