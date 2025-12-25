using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.DeductionUseCases.Commands.Delete
{
    internal class ToggleDeductionCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<ToggleDeductionCommandHandler> logger,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<ToggleDeductionCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ToggleDeductionCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<bool> Handle(ToggleDeductionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting ToggleDeductionCommandHandler for DeductionId: {DeductionId}", request.Id);

            var deductionRepo = _unitOfWork.GetRepository<Deduction, Guid>();
            var deduction = await deductionRepo.GetByIdAsync(request.Id, cancellationToken);

            if (deduction is null)
            {
                _logger.LogWarning("Deduction not found. DeductionId: {DeductionId}", request.Id);
                throw new NotFoundException<Deduction>(request.Id);
            }

            var user = await _userManager.FindByNameAsync(request.ToggledBy);
            if (user is null)
            {
                _logger.LogWarning("User not found for identity: {ToggledBy}", request.ToggledBy);
                throw new NotFoundException<ApplicationUser>(request.ToggledBy);
            }

            if (deduction.IsProcessed)
            {
                throw new BadRequestException("Cannot toggle a deduction that has already been processed in payroll.");
            }

            _logger.LogInformation("Toggling IsDeleted for DeductionId: {DeductionId} by {ModifiedBy}", request.Id, request.ToggledBy);
            deduction.Toggle(request.ToggledBy);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deduction status toggled successfully. DeductionId: {DeductionId}", request.Id);

            return true;
        }
    }
}