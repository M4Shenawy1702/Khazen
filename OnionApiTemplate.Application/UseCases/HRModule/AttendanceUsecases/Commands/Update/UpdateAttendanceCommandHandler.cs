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
            var attendanceId = request.Id;
            var actorUserId = request.CurrentUserId;
            var dto = request.Dto;

            _logger.LogInformation("Starting UpdateAttendanceCommandHandler for AttendanceId: {AttendanceId} by Actor: {ActorId}",
                attendanceId, actorUserId);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Validation failed for AttendanceId: {AttendanceId}. Errors: {Errors}", attendanceId, string.Join(", ", errors));
                throw new BadRequestException(errors);
            }

            var attendanceRepo = _unitOfWork.GetRepository<AttendanceRecord, Guid>();
            var attendance = await attendanceRepo.GetAsync(
                new GetAttendanceRecordByIdSpecification(attendanceId),
                cancellationToken
            ) ?? throw new NotFoundException<AttendanceRecord>(attendanceId);

            _logger.LogDebug("Found attendance record for EmployeeId: {EmployeeId}", attendance.EmployeeId);


            if (dto.CheckInTime.HasValue)
            {
                attendance.CheckIn(dto.CheckInTime.Value);
            }
            if (dto.CheckOutTime.HasValue)
            {
                attendance.CheckOut(dto.CheckOutTime.Value);
            }

            if (dto.Status.HasValue)
            {
                attendance.UpdateStatus(dto.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(dto.Notes))
            {
                attendance.UpdateNotes(dto.Notes);
            }

            attendance.ModifiedBy = actorUserId;
            attendance.ModifiedAt = DateTime.UtcNow;

            _logger.LogInformation("Updating attendance for EmployeeId: {EmployeeId}. NewStatus: {NewStatus}",
                attendance.EmployeeId, attendance.Status);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated attendance record successfully for AttendanceId: {AttendanceId}", attendanceId);

            return _mapper.Map<AttendanceDto>(attendance);
        }
    }
}