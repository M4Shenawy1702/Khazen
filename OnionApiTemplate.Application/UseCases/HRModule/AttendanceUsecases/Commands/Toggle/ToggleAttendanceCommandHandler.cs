using Khazen.Application.BaseSpecifications.HRModule.AttendanceSpecifications;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Delete
{
    internal class ToggleAttendanceCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<ToggleAttendanceCommandHandler> logger,
        UserManager<ApplicationUser> userManager
    ) : IRequestHandler<ToggleAttendanceCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ToggleAttendanceCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<bool> Handle(ToggleAttendanceCommand request, CancellationToken cancellationToken)
        {

            _logger.LogInformation("Starting Soft Deletion Toggle for AttendanceId: {AttendanceId} by toggle user: {ToggleBy}",
                request.Id, request.CurrentUserId);

            var user = await _userManager.FindByNameAsync(request.CurrentUserId);
            if (user == null)
            {
                _logger.LogWarning("Creator user {user} not found. Cannot audit transaction.", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }

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
            attendance.Toggle(user.Id);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Soft Deletion Toggled for AttendanceId: {AttendanceId}. Previous: {PreviousState}, Current: {CurrentState}",
                request.Id,
                previousState,
                attendance.IsDeleted
            );

            return true;
        }
    }
}