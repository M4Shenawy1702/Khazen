using Khazen.Application.BaseSpecifications.HRModule.EmployeeSpecifications;
using Khazen.Application.DOTs.HRModule.AdvanceDtos;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.AdvanceUseCases.Commands.Create
{
    internal class AddAdvanceCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<AddAdvanceCommand> validator,
        ILogger<AddAdvanceCommandHandler> logger,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<AddAdvanceCommand, AdvanceDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<AddAdvanceCommand> _validator = validator;
        private readonly ILogger<AddAdvanceCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<AdvanceDto> Handle(AddAdvanceCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                _logger.LogDebug("Starting AddAdvanceCommandHandler for EmployeeId: {EmployeeId}", request.Dto.EmployeeId);

                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for AddAdvanceCommand: {Errors}",
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }

                var employeeRepo = _unitOfWork.GetRepository<Employee, Guid>();
                var employee = await employeeRepo.GetAsync(new GetEmployeeByIdSpecification(request.Dto.EmployeeId), cancellationToken);

                if (employee is null)
                {
                    _logger.LogWarning("Employee {EmployeeId} not found.", request.Dto.EmployeeId);
                    throw new NotFoundException<Employee>(request.Dto.EmployeeId);
                }
                _logger.LogDebug("Employee {EmployeeId} found. Calculating total advances and deductions...", request.Dto.EmployeeId);

                var user = await _userManager.FindByNameAsync(request.CreatedBy);
                if (user is null)
                {
                    _logger.LogInformation("User not found. UserId: {ModifiedBy}", request.CreatedBy);
                    throw new NotFoundException<ApplicationUser>(request.CreatedBy);
                }

                var totalAdvances = employee.Advances?.Sum(x => x.Amount) ?? 0;
                var totalDeductions = employee.Deductions?.Sum(x => x.Amount) ?? 0;
                var totalAfterNewAdvance = totalAdvances + request.Dto.Amount;
                var maxAllowed = employee.BaseSalary + totalDeductions;

                if (totalAfterNewAdvance > maxAllowed)
                {
                    _logger.LogWarning("Advance exceeds salary for Employee {EmployeeId}. Requested: {Requested}, Allowed: {Allowed}",
                        request.Dto.EmployeeId, totalAfterNewAdvance, maxAllowed);

                    throw new BadRequestException(
                        $"Total advance amount ({totalAfterNewAdvance}) exceeds employee's salary ({employee.BaseSalary}).");
                }

                var advanceRepo = _unitOfWork.GetRepository<Advance, int>();
                var advance = employee.CreateAdvance(request.Dto.Amount, request.Dto.Reason, request.Dto.Date, request.CreatedBy);

                await advanceRepo.AddAsync(advance, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Advance created successfully for Employee {EmployeeId} by {CreatedBy}. AdvanceId: {AdvanceId}",
                 request.Dto.EmployeeId, request.CreatedBy, advance.Id);

                return _mapper.Map<AdvanceDto>(advance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating advance for EmployeeId {EmployeeId}", request.Dto.EmployeeId);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw new ApplicationException("An unexpected error occurred while processing the advance addition.", ex);
            }
        }
    }
}
