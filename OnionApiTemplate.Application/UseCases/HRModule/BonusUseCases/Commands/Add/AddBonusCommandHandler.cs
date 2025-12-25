using Khazen.Application.BaseSpecifications.HRModule.EmployeeSpecifications;
using Khazen.Application.DOTs.HRModule.BonusDtos;
using Khazen.Application.UseCases.HRModule.BonusUseCases.Commands.Add;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.BonusUsecases.Commands.Grant
{
    public class GrantBonusCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<AddBonusCommand> validator,
        ILogger<GrantBonusCommandHandler> logger,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<AddBonusCommand, BonusDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<AddBonusCommand> _validator = validator;
        private readonly ILogger<GrantBonusCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<BonusDto> Handle(AddBonusCommand request, CancellationToken cancellationToken)
        {
            var employeeId = request.Dto.EmployeeId;
            var actorUserId = request.CurrentUserId;

            _logger.LogInformation("Starting GrantBonusCommand for EmployeeId: {EmployeeId} by Actor: {ActorId}", employeeId, actorUserId);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Validation failed for GrantBonusCommand. Errors: {Errors}", string.Join(", ", errors));
                throw new BadRequestException(errors);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var employeeRepo = _unitOfWork.GetRepository<Employee, Guid>();
                var employeeExists = await employeeRepo.GetAsync(new GetEmployeeByIdSpecification(employeeId), cancellationToken);
                if (employeeExists is null)
                {
                    _logger.LogError("Employee not found while granting bonus. EmployeeId: {EmployeeId}", employeeId);
                    throw new NotFoundException<Employee>(employeeId);
                }

                var user = await _userManager.FindByIdAsync(actorUserId);
                if (user is null)
                {
                    _logger.LogWarning("Actor user not found. ActorId: {ActorId}", actorUserId);
                    throw new NotFoundException<ApplicationUser>(actorUserId);
                }

                var bonusRepo = _unitOfWork.GetRepository<Bonus, Guid>();
                var bonus = _mapper.Map<Bonus>(request.Dto);

                bonus.CreatedBy = actorUserId;
                bonus.CreatedAt = DateTime.UtcNow;
                bonus.IsPaid = false;

                await bonusRepo.AddAsync(bonus, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Bonus successfully granted (ID: {BonusId}) for EmployeeId: {EmployeeId}", bonus.Id, employeeId);

                return _mapper.Map<BonusDto>(bonus);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Transaction rolled back. Error occurred while granting bonus for EmployeeId: {EmployeeId}", employeeId);
                throw;
            }
        }
    }
}