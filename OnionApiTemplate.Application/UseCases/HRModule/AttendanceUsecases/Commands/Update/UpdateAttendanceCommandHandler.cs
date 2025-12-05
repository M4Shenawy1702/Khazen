using Khazen.Application.BaseSpecifications.HRModule.AttendanceSpecifications;
using Khazen.Application.DOTs.HRModule.AttendaceDtos;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.AttendanceUsecases.Commands.Update
{
    internal class UpdateAttendanceCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<UpdateAttendanceCommand> validator,
        ILogger<UpdateAttendanceCommandHandler> logger
    ) : IRequestHandler<UpdateAttendanceCommand, AttendanceDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdateAttendanceCommand> _validator = validator;
        private readonly ILogger<UpdateAttendanceCommandHandler> _logger = logger;

        public async Task<AttendanceDto> Handle(UpdateAttendanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Starting UpdateAttendanceCommandHandler for AttendanceId: {AttendanceId}", request.Id);

                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for AttendanceId: {AttendanceId}. Errors: {Errors}",
                        request.Id,
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }

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
                _logger.LogInformation("Found attendance record for EmployeeId: {EmployeeId}", attendance.EmployeeId);

                attendance.CheckIn(request.Dto.CheckInTime);
                attendance.CheckOut(request.Dto.CheckOutTime);
                attendance.UpdateStatus(request.Dto.Status);
                attendance.UpdateNotes(request.Dto.Notes);

                _logger.LogDebug(
                    "Updating attendance for EmployeeId: {EmployeeId}. OldStatus: {OldStatus}, NewStatus: {NewStatus}",
                    attendance.EmployeeId,
                    attendance.Status,
                    request.Dto.Status
                );

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Updated attendance record successfully for AttendanceId: {AttendanceId}", request.Id);

                return _mapper.Map<AttendanceDto>(attendance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating attendance record for Id: {AttendanceId}", request.Id);
                throw;
            }
        }
    }
}
