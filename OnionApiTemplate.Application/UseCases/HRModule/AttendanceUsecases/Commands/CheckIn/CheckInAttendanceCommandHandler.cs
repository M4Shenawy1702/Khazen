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
        ILogger<CheckInAttendanceCommandHandler> logger
    ) : IRequestHandler<CheckInAttendanceCommand, AttendanceDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CheckInAttendanceCommand> _validator = validator;
        private readonly ILogger<CheckInAttendanceCommandHandler> _logger = logger;

        public async Task<AttendanceDto> Handle(CheckInAttendanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Starting CheckInAttendanceCommandHandler for EmployeeId: {EmployeeId}", request.Dto.EmployeeId);

                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for EmployeeId: {EmployeeId}. Errors: {Errors}",
                        request.Dto.EmployeeId,
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }

                var employeeRepo = _unitOfWork.GetRepository<Employee, Guid>();
                var employee = await employeeRepo.GetAsync(new GetEmployeeByIdSpecification(request.Dto.EmployeeId), cancellationToken)
                    ?? throw new NotFoundException<Employee>(request.Dto.EmployeeId);

                _logger.LogInformation("Employee {EmployeeId} found successfully.", request.Dto.EmployeeId);

                var attendanceRepo = _unitOfWork.GetRepository<AttendanceRecord, Guid>();
                var today = DateTime.UtcNow.Date;

                var attendance = await attendanceRepo.GetAsync(
                    new GetAttendanceRecordByEmployeeIdAndDateSpecification(request.Dto.EmployeeId, today),
                    cancellationToken
                );

                if (attendance != null && attendance.CheckInTime.HasValue)
                {
                    _logger.LogWarning("Employee {EmployeeId} has already checked in today.", request.Dto.EmployeeId);
                    throw new BadRequestException(["Employee has already checked in today"]);
                }

                if (attendance is null)
                {
                    attendance = _mapper.Map<AttendanceRecord>(request.Dto);
                    await attendanceRepo.AddAsync(attendance, cancellationToken);
                    _logger.LogDebug("Created new attendance record for EmployeeId: {EmployeeId}.", request.Dto.EmployeeId);
                }

                attendance.CheckIn(TimeOnly.FromDateTime(DateTime.UtcNow));
                _logger.LogInformation("Employee {EmployeeId} checked in at {Time}.", request.Dto.EmployeeId, DateTime.UtcNow);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Attendance record saved successfully for EmployeeId: {EmployeeId}.", request.Dto.EmployeeId);

                return _mapper.Map<AttendanceDto>(attendance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during check-in for EmployeeId: {EmployeeId}", request.Dto.EmployeeId);
                throw;
            }
        }
    }
}
