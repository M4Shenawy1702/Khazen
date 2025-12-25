using Khazen.Application.BaseSpecifications.HRModule.EmployeeSpecifications;
using Khazen.Application.DOTs.HRModule.AttendaceDtos;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Leave
{
    internal class MarkAsLeaveCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<MarkAsLeaveCommandHandler> logger,
        UserManager<ApplicationUser> userManager
    )
    : IRequestHandler<MarkAsLeaveCommand, AttendanceDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<MarkAsLeaveCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<AttendanceDto> Handle(MarkAsLeaveCommand request, CancellationToken cancellationToken)
        {
            var employeeId = request.Dto.EmployeeId;
            string actorUsername = request.CurrentUserId;

            var targetDate = request.Dto.Date
                             ?? DateOnly.FromDateTime(DateTime.UtcNow);

            _logger.LogInformation("Starting MarkAsLeaveCommand for EmployeeId: {EmployeeId} on Date: {Date} by Actor: {Username}",
                employeeId, targetDate, actorUsername);

            var creatorUser = await _userManager.FindByNameAsync(actorUsername);
            if (creatorUser == null)
            {
                _logger.LogWarning("Creator user {Username} not found. Cannot audit transaction.", actorUsername);
                throw new NotFoundException<ApplicationUser>(actorUsername);
            }
            string actorUserId = creatorUser.Id;

            var attendanceRepo = _unitOfWork.GetRepository<AttendanceRecord, Guid>();
            var employeeRepo = _unitOfWork.GetRepository<Employee, Guid>();

            var employee = await employeeRepo.GetAsync(new GetEmployeeByIdSpecification(employeeId), cancellationToken);
            if (employee is null)
            {
                _logger.LogWarning("Employee with ID {EmployeeId} not found.", employeeId);
                throw new NotFoundException<Employee>(employeeId);
            }

            var attendance = await attendanceRepo
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == targetDate, cancellationToken);

            bool isNewRecord = (attendance is null);

            if (isNewRecord)
            {

                _logger.LogInformation("Creating new attendance record for LEAVE for EmployeeId: {EmployeeId}", employeeId);

                attendance = _mapper.Map<AttendanceRecord>(request.Dto);
                attendance.Date = targetDate;

                attendance.CreatedBy = actorUserId;
                attendance.CreatedAt = DateTime.UtcNow;

                await attendanceRepo.AddAsync(attendance, cancellationToken);
            }
            else
            {
                attendance!.ModifiedBy = actorUserId;
                attendance.ModifiedAt = DateTime.UtcNow;
                _logger.LogInformation("Updating existing attendance record {AttendanceId} for LEAVE override.", attendance.Id);
            }

            attendance.MarkLeave(request.Dto.Notes);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Attendance record for EmployeeId {EmployeeId} marked as LEAVE successfully by User {UserId}.",
                employeeId, actorUserId);

            return _mapper.Map<AttendanceDto>(attendance);
        }
    }
}