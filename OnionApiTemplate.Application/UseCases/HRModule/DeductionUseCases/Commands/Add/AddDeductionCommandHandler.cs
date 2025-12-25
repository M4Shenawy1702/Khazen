using Khazen.Application.BaseSpecifications.HRModule.EmployeeSpecifications;
using Khazen.Application.DOTs.HRModule.Deduction;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.DeductionUseCases.Commands.Add
{
    internal class AddDeductionCommandHandler(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IValidator<AddDeductionCommand> validator,
        ILogger<AddDeductionCommandHandler> logger,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<AddDeductionCommand, DeductionDto>
    {
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IValidator<AddDeductionCommand> _validator = validator;
        private readonly ILogger<AddDeductionCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<DeductionDto> Handle(AddDeductionCommand request, CancellationToken cancellationToken)
        {
            var employeeId = request.Dto.EmployeeId;
            var actorUserId = request.CurrentUserId;

            _logger.LogDebug("Starting AddDeductionCommandHandler for EmployeeId: {EmployeeId}", employeeId);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Validation failed for AddDeductionCommand: {Errors}", string.Join(", ", errors));
                throw new BadRequestException(errors);
            }

            var employeeRepo = _unitOfWork.GetRepository<Employee, Guid>();
            var employeeExists = await employeeRepo.GetAsync(new GetEmployeeByIdSpecification(employeeId), cancellationToken);

            if (employeeExists is null)
            {
                _logger.LogWarning("Employee {EmployeeId} not found.", employeeId);
                throw new NotFoundException<Employee>(employeeId);
            }

            var user = await _userManager.FindByIdAsync(actorUserId);
            if (user is null)
            {
                _logger.LogWarning("Actor user not found. ActorId: {ActorId}", actorUserId);
                throw new NotFoundException<ApplicationUser>(actorUserId);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var deductionRepo = _unitOfWork.GetRepository<Deduction, Guid>();
                var deduction = _mapper.Map<Deduction>(request.Dto);

                deduction.CreatedBy = actorUserId;
                deduction.CreatedAt = DateTime.UtcNow;

                await deductionRepo.AddAsync(deduction, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Deduction created successfully for Employee {EmployeeId}. DeductionId: {DeductionId}",
                    employeeId, deduction.Id);

                return _mapper.Map<DeductionDto>(deduction);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Transaction rolled back. Error occurred while creating deduction for EmployeeId {EmployeeId}", employeeId);
                throw;
            }
        }
    }
}