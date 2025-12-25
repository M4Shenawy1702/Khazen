using Khazen.Application.BaseSpecifications.HRModule.AttendanceSpecifications;
using Khazen.Application.BaseSpecifications.HRModule.EmployeeSpecifications;
using Khazen.Application.DOTs.HRModule.AttendaceDtos;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.CheckIn
{
    public class CheckInAttendanceCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CheckInAttendanceCommand> validator,
        ILogger<CheckInAttendanceCommandHandler> logger,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<CheckInAttendanceCommand, AttendanceDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CheckInAttendanceCommand> _validator = validator;
        private readonly ILogger<CheckInAttendanceCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<AttendanceDto> Handle(CheckInAttendanceCommand request, CancellationToken cancellationToken)
        {
            var employeeId = request.Dto.EmployeeId;
            var checkInTime = TimeOnly.FromDateTime(DateTime.UtcNow);
            var targetDate = DateOnly.FromDateTime(DateTime.UtcNow);
            string actorUsername = request.CurrentUserId;

            _logger.LogInformation("Starting CheckIn for EmployeeId: {EmployeeId} on {Date} at {Time}",
                employeeId, targetDate, checkInTime);

            var creatorUser = await _userManager.FindByNameAsync(actorUsername);
            if (creatorUser == null)
            {
                _logger.LogWarning("Creator user {Username} not found. Cannot audit transaction.", actorUsername);
                throw new NotFoundException<ApplicationUser>(actorUsername);
            }

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Validation failed for EmployeeId: {EmployeeId}. Errors: {Errors}", employeeId, string.Join(", ", errors));
                throw new BadRequestException(errors);
            }

            var employeeRepo = _unitOfWork.GetRepository<Employee, Guid>();
            var employee = await employeeRepo.GetAsync(new GetEmployeeByIdSpecification(employeeId), cancellationToken);

            if (employee is null)
            {
                _logger.LogWarning("Employee {EmployeeId} not found. Cannot check in.", employeeId);
                throw new NotFoundException<Employee>(employeeId);
            }

            var attendanceRepo = _unitOfWork.GetRepository<AttendanceRecord, Guid>();

            var attendance = await attendanceRepo.GetAsync(
                new GetAttendanceRecordByEmployeeIdAndDateSpecification(employeeId, targetDate.ToDateTime(TimeOnly.MinValue)), cancellationToken);

            bool recordFound = (attendance is not null);

            if (recordFound && attendance.CheckInTime.HasValue && !attendance.CheckOutTime.HasValue)
            {
                _logger.LogWarning("Employee {EmployeeId} is currently checked in at {Time}.", employeeId, attendance.CheckInTime);
                throw new BadRequestException(["Employee is already checked in today"]);
            }

            if (!recordFound)
            {
                attendance = _mapper.Map<AttendanceRecord>(request.Dto);
                attendance.Date = targetDate;

                attendance.CreatedBy = creatorUser.Id;
                attendance.CreatedAt = DateTime.UtcNow;

                await attendanceRepo.AddAsync(attendance, cancellationToken);
                _logger.LogDebug("Created new attendance record for EmployeeId: {EmployeeId}.", employeeId);
            }

            attendance!.CheckIn(checkInTime);

            attendance.ModifiedBy = creatorUser.Id;
            attendance.ModifiedAt = DateTime.UtcNow;

            _logger.LogInformation("Employee {EmployeeId} checked in at {Time}. Saving changes.", employeeId, checkInTime);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Attendance record saved successfully for EmployeeId: {EmployeeId}. Time: {Time}", employeeId, checkInTime);

            return _mapper.Map<AttendanceDto>(attendance);
        }
    }
}