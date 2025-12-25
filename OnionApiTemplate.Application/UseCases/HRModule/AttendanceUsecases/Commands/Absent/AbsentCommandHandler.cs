using Khazen.Application.BaseSpecifications.HRModule.EmployeeSpecifications;
using Khazen.Application.DOTs.HRModule.AttendaceDtos;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Absent
{
    internal class AbsentCommandHandler(
       IUnitOfWork unitOfWork,
       IMapper mapper,
       IValidator<AbsentCommand> validator,
       ILogger<AbsentCommandHandler> logger,
       UserManager<ApplicationUser> userManager
   ) : IRequestHandler<AbsentCommand, AttendanceDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<AbsentCommand> _validator = validator;
        private readonly ILogger<AbsentCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<AttendanceDto> Handle(AbsentCommand request, CancellationToken cancellationToken)
        {
            var employeeId = request.Dto.EmployeeId;

            _logger.LogInformation("Starting AbsentCommandHandler for EmployeeId: {EmployeeId} on Date: {Date}", employeeId, request.Dto.Date);


            var user = await _userManager.FindByNameAsync(request.CurrentUserId);
            if (user == null)
            {
                _logger.LogWarning("Creator user {Username} not found. Cannot audit transaction.", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Validation failed for EmployeeId: {EmployeeId}. Errors: {Errors}", employeeId, string.Join(", ", errors));
                throw new BadRequestException(errors);
            }

            var employeeRepo = _unitOfWork.GetRepository<Employee, Guid>();
            var employee = await employeeRepo.GetAsync(
                new GetEmployeeByIdSpecification(employeeId), cancellationToken)
                ?? throw new NotFoundException<Employee>(employeeId);

            var attendanceRepo = _unitOfWork.GetRepository<AttendanceRecord, Guid>();

            var attendance = await attendanceRepo
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date == request.Dto.Date, cancellationToken);

            bool isNewRecord = (attendance is null);

            if (attendance != null && attendance.CheckInTime.HasValue)
            {
                _logger.LogWarning("Cannot mark as Absent: Employee {EmployeeId} has already checked in today at {Time}. Use MarkLeave/UpdateStatus for overrides.", employeeId, attendance.CheckInTime);
                throw new BadRequestException(["Record cannot be marked as Absent; employee has checked in. Use 'Mark Leave' or 'Update Status' if an administrative override is required."]);
            }

            if (isNewRecord)
            {
                attendance = _mapper.Map<AttendanceRecord>(request.Dto);
                attendance.Date = request.Dto.Date;

                attendance.CreatedBy = user.Id;
                attendance.CreatedAt = DateTime.UtcNow;

                await attendanceRepo.AddAsync(attendance, cancellationToken);
                _logger.LogDebug("Created new attendance record for EmployeeId: {EmployeeId}.", employeeId);
            }

            attendance!.MarkAsAbsent();

            attendance.ModifiedBy = user.Id;
            attendance.ModifiedAt = DateTime.UtcNow;


            _logger.LogInformation("Attendance marked as absent for EmployeeId: {EmployeeId}. Saving changes.", employeeId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Attendance marked as absent successfully for EmployeeId: {EmployeeId}", employeeId);

            return _mapper.Map<AttendanceDto>(attendance);
        }
    }
}