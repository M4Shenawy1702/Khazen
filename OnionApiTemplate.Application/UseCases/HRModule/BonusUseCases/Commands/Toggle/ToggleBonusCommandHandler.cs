using Khazen.Application.Specification.HRModule.BounsSpecifications;
using Khazen.Application.UseCases.HRModule.BonusUseCases.Commands.Delete;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.BonusUseCases.Commands.Toggle
{
    internal class ToggleBonusCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<ToggleBonusCommandHandler> logger,
        UserManager<ApplicationUser> userManager
    ) : IRequestHandler<ToggleBonusCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ToggleBonusCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<bool> Handle(ToggleBonusCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting ToggleBonusCommandHandler for BonusId: {BonusId}", request.Id);

            try
            {
                var bonusRepo = _unitOfWork.GetRepository<Bonus, int>();
                var bonus = await bonusRepo.GetAsync(new GetBounsByIdSpecification(request.Id), cancellationToken);
                if (bonus == null)
                {
                    _logger.LogWarning("Bonus not found while toggling IsDeleted for Id: {BonusId}", request.Id);
                    throw new NotFoundException<Bonus>(request.Id);
                }
                var user = await _userManager.FindByNameAsync(request.ModifiedBy);
                if (user is null)
                {
                    _logger.LogInformation("User not found. UserId: {ModifiedBy}", request.ModifiedBy);
                    throw new NotFoundException<ApplicationUser>(request.ModifiedBy);
                }
                bonus.Toggle(request.ModifiedBy);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully toggled bonus (IsDeleted = {IsDeleted}) for BonusId: {BonusId}",
                    bonus.IsDeleted, request.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while toggling bonus for Id: {BonusId}", request.Id);
                throw new ApplicationException("An unexpected error occurred while toggling the bonus.", ex);
            }
        }
    }
}
