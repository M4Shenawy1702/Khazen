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
            var bonusId = request.Id;
            var actorUserId = request.CurrentUserId;

            _logger.LogInformation("Starting ToggleBonusCommandHandler for BonusId: {BonusId} by Actor: {ActorId}", bonusId, actorUserId);

            var user = await _userManager.FindByIdAsync(actorUserId);
            if (user is null)
            {
                _logger.LogWarning("Actor user not found. ActorId: {ActorId}", actorUserId);
                throw new NotFoundException<ApplicationUser>(actorUserId);
            }

            var bonusRepo = _unitOfWork.GetRepository<Bonus, Guid>();
            var bonus = await bonusRepo.GetAsync(new GetBounsByIdSpecification(bonusId), cancellationToken);

            if (bonus == null)
            {
                _logger.LogWarning("Bonus not found while toggling IsDeleted for Id: {BonusId}", bonusId);
                throw new NotFoundException<Bonus>(bonusId);
            }

            if (bonus.IsPaid)
            {
                _logger.LogWarning("Attempted to toggle paid bonus record {BonusId}.", bonusId);
                throw new ConflictException("Cannot delete or toggle a bonus record that has already been marked as paid.");
            }

            bonus.Toggle(actorUserId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully toggled bonus (IsDeleted = {IsDeleted}) for BonusId: {BonusId}",
                bonus.IsDeleted, bonusId);

            return true;
        }
    }
}