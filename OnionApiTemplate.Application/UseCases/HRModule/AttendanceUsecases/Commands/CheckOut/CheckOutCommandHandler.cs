using Khazen.Application.DOTs.HRModule.AttendaceDtos;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.CheckOut
{
    internal class CheckOutCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CheckOutCommandHandler> logger,
        UserManager<ApplicationUser> userManager
    ) : IRequestHandler<CheckOutCommand, AttendanceDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<CheckOutCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<AttendanceDto> Handle(CheckOutCommand request, CancellationToken cancellationToken)
        {
            var checkOutTime = request.Dto.CheckOutTime ?? TimeOnly.FromDateTime(DateTime.UtcNow);
            var targetDate = DateOnly.FromDateTime(DateTime.UtcNow);

            _logger.LogInformation("Starting CheckOut for EmployeeId: {EmployeeId} on {Date} at {Time} by modifiedBy: {ModifiedBy}",
                request.Dto.EmployeeId, targetDate, checkOutTime, request.CurrentUserId);

            var user = await _userManager.FindByNameAsync(request.CurrentUserId);
            if (user == null)
            {
                _logger.LogWarning("User {CurrentUserId} not found.", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }

            var attendanceRepo = _unitOfWork.GetRepository<AttendanceRecord, Guid>();

            var attendance = await attendanceRepo
                .FirstOrDefaultAsync(a => a.EmployeeId == request.Dto.EmployeeId && a.Date == targetDate, cancellationToken);

            if (attendance is null)
            {
                _logger.LogWarning("CheckOut failed: No active check-in record found for Employee {EmployeeId} today.", request.Dto.EmployeeId);
                throw new BadRequestException($"Cannot check out. No check-in record found for Employee ID: {request.Dto.EmployeeId} on {targetDate}.");
            }

            if (!attendance.CheckInTime.HasValue)
            {
                _logger.LogWarning("Cannot check out for Employee {EmployeeId}: CheckInTime is missing.", request.Dto.EmployeeId);
                throw new BadRequestException(["Employee has not checked in yet today."]);
            }

            if (attendance.CheckOutTime.HasValue)
            {
                _logger.LogWarning("Employee {EmployeeId} has already checked out today at {Time}.", request.Dto.EmployeeId, attendance.CheckOutTime);
                throw new BadRequestException(["Employee has already checked out today."]);
            }

            attendance.CheckOut(checkOutTime);

            attendance.ModifiedBy = request.CurrentUserId;
            attendance.ModifiedAt = DateTime.UtcNow;

            _logger.LogInformation("Employee {EmployeeId} checked out successfully at {CheckOutTime}.", request.Dto.EmployeeId, attendance.CheckOutTime);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Attendance record {AttendanceId} saved successfully.", attendance.Id);

            return _mapper.Map<AttendanceDto>(attendance);
        }
    }
}