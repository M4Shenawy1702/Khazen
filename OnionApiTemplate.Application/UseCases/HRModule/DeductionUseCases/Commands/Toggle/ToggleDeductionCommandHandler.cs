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
            try
            {
                _logger.LogDebug("Starting DeleteDeductionCommandHandler for DeductionId: {DeductionId}", request.Id);

                var deductionRepo = _unitOfWork.GetRepository<Deduction, int>();
                var deduction = await deductionRepo.GetByIdAsync(request.Id, cancellationToken);
                if (deduction is null)
                {
                    _logger.LogInformation("Deduction not found. DeductionId: {DeductionId}", request.Id);
                    throw new NotFoundException<Deduction>(request.Id);
                }

                _logger.LogInformation("Toggling IsDeleted for DeductionId: {DeductionId} by {ModifiedBy}", request.Id, request.ModifiedBy);

                var user = await _userManager.FindByNameAsync(request.ModifiedBy);
                if (user is null)
                {
                    _logger.LogInformation("User not found. UserId: {ModifiedBy}", request.ModifiedBy);
                    throw new NotFoundException<ApplicationUser>(request.ModifiedBy);
                }

                deduction.Toggle(request.ModifiedBy);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Deduction soft-deleted successfully. DeductionId: {DeductionId}", request.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Deduction with Id: {DeductionId}", request.Id);
                throw;
            }
        }
    }
}
