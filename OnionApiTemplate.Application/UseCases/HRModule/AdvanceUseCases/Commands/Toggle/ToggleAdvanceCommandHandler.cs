using Khazen.Application.BaseSpecifications.HRModule.AdvanceSpecifications;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.AdvanceUseCases.Commands.Toggle
{
    internal class ToggleAdvanceCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ToggleAdvanceCommandHandler> logger,
        UserManager<ApplicationUser> userManager
    ) : IRequestHandler<ToggleAdvanceCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ToggleAdvanceCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<bool> Handle(ToggleAdvanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Starting ToggleAdvanceCommand for AdvanceId: {AdvanceId}", request.Id);

                var advanceRepo = _unitOfWork.GetRepository<Advance, int>();
                var advance = await advanceRepo.GetAsync(new GetAdvanceByIdSpecification(request.Id), cancellationToken);

                if (advance is null)
                {
                    _logger.LogWarning("Advance with ID {AdvanceId} not found.", request.Id);
                    throw new NotFoundException<Advance>(request.Id);
                }
                var user = await _userManager.FindByNameAsync(request.ModifiedBy);
                if (user is null)
                {
                    _logger.LogInformation("User not found. UserId: {ModifiedBy}", request.ModifiedBy);
                    throw new NotFoundException<ApplicationUser>(request.ModifiedBy);
                }
                advance.Toggle(request.ModifiedBy);

                _logger.LogInformation("Toggling Advance {AdvanceId}. New IsDeleted state: {IsDeleted}", advance.Id, advance.IsDeleted);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Successfully toggled Advance {AdvanceId}.", advance.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while toggling Advance {AdvanceId}", request.Id);
                throw new ApplicationException($"An unexpected error occurred while toggling Advance {request.Id}.", ex);
            }
        }
    }
}
