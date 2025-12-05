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
            _logger.LogDebug("Starting AddDeductionCommandHandler for EmployeeId: {EmployeeId}", request.Dto.EmployeeId);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for AddDeductionCommand: {Errors}",
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var employeeRepo = _unitOfWork.GetRepository<Employee, Guid>();
                var employee = await employeeRepo.GetAsync(new GetEmployeeByIdSpecification(request.Dto.EmployeeId), cancellationToken);

                if (employee is null)
                {
                    _logger.LogWarning("Employee {EmployeeId} not found.", request.Dto.EmployeeId);
                    throw new NotFoundException<Employee>(request.Dto.EmployeeId);
                }

                _logger.LogDebug("Employee {EmployeeId} found. Creating deduction...", request.Dto.EmployeeId);

                var user = await _userManager.FindByNameAsync(request.CreatedBy);
                if (user is null)
                {
                    _logger.LogInformation("User not found. UserId: {ModifiedBy}", request.CreatedBy);
                    throw new NotFoundException<ApplicationUser>(request.CreatedBy);
                }

                var deductionRepo = _unitOfWork.GetRepository<Deduction, int>();
                var deduction = employee.CreateDeduction(
                    request.Dto.Amount,
                    request.Dto.Reason ?? "Deduction",
                    request.Dto.Date,
                    request.CreatedBy);

                await deductionRepo.AddAsync(deduction, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Deduction created successfully for Employee {EmployeeId} by {CreatedBy}. DeductionId: {DeductionId}",
                    request.Dto.EmployeeId, request.CreatedBy, deduction.Id);

                return _mapper.Map<DeductionDto>(deduction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating deduction for EmployeeId {EmployeeId}", request.Dto.EmployeeId);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw new ApplicationException("An unexpected error occurred while adding the deduction.", ex);
            }
        }
    }
}
