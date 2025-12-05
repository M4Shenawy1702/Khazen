using Khazen.Application.BaseSpecifications.HRModule.AttendanceSpecifications;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Delete
{
    internal class ToggleAttendanceCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<ToggleAttendanceCommandHandler> logger
    ) : IRequestHandler<ToggleAttendanceCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ToggleAttendanceCommandHandler> _logger = logger;

        public async Task<bool> Handle(ToggleAttendanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Starting ToggleAttendanceCommandHandler for AttendanceId: {AttendanceId}", request.Id);

                var attendanceRepo = _unitOfWork.GetRepository<AttendanceRecord, Guid>();
                var attendance = await attendanceRepo.GetAsync(
                    new GetAttendanceRecordByIdSpecification(request.Id),
                    cancellationToken
                );
                if (attendance == null)
                {
                    _logger.LogWarning("Attendance record not found for Id: {AttendanceId}", request.Id);
                    throw new NotFoundException<AttendanceRecord>(request.Id);
                }

                bool previousState = attendance.IsDeleted;
                attendance.IsDeleted = !attendance.IsDeleted;

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Toggled IsDeleted for AttendanceId: {AttendanceId}. Previous: {PreviousState}, Current: {CurrentState}",
                    request.Id,
                    previousState,
                    attendance.IsDeleted
                );

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling attendance record for Id: {AttendanceId}", request.Id);
                throw;
            }
        }
    }
}
