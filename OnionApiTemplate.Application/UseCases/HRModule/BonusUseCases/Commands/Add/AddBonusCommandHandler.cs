using Khazen.Application.DOTs.HRModule.BonusDtos;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.BonusUseCases.Commands.Add
{
    public class AddBonusCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<AddBonusCommand> validator,
        ILogger<AddBonusCommandHandler> logger,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<AddBonusCommand, BonusDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<AddBonusCommand> _validator = validator;
        private readonly ILogger<AddBonusCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<BonusDto> Handle(AddBonusCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting AddBonusCommand for EmployeeId: {EmployeeId}", request.Dto.EmployeeId);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for AddBonusCommand: {Errors}",
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }

                var employeeRepo = _unitOfWork.GetRepository<Employee, Guid>();
                var employee = await employeeRepo.GetByIdAsync(request.Dto.EmployeeId, cancellationToken);

                if (employee is null)
                {
                    _logger.LogError("Employee not found while adding bonus. EmployeeId: {EmployeeId}", request.Dto.EmployeeId);
                    throw new NotFoundException<Employee>(request.Dto.EmployeeId);
                }

                var user = await _userManager.FindByNameAsync(request.CreatedBy);
                if (user is null)
                {
                    _logger.LogInformation("User not found. UserId: {CreatedBy}", request.CreatedBy);
                    throw new NotFoundException<ApplicationUser>(request.CreatedBy);
                }

                var bonusRepo = _unitOfWork.GetRepository<Bonus, int>();
                var bonus = employee.CreateBonus(
                    request.Dto.BonusAmount,
                    request.Dto.Reason!,
                    request.Dto.Date,
                    request.CreatedBy
                );

                await bonusRepo.AddAsync(bonus, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Bonus successfully added for EmployeeId: {EmployeeId} with Amount: {Amount}",
                    request.Dto.EmployeeId, request.Dto.BonusAmount);

                return _mapper.Map<BonusDto>(bonus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while adding bonus for EmployeeId: {EmployeeId}", request.Dto.EmployeeId);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                throw new ApplicationException("An unexpected error occurred while processing the bonus addition.", ex);
            }
        }
    }
}
