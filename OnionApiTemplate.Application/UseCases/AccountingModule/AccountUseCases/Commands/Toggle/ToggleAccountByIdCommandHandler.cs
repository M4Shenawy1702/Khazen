using Khazen.Application.Common.Interfaces;
using Khazen.Application.Specification.AccountingModule.AccountSpecifications;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AccountingModule.AccountUseCases.Commands.Delete
{
    internal class ToggleAccountByIdCommandHandler(IUnitOfWork unitOfWork,
                                                   IValidator<ToggleAccountByIdCommand> validator,
                                                   ILogger<ToggleAccountByIdCommandHandler> logger,
                                                   UserManager<ApplicationUser> userManager,
                                                   ICacheService cacheService)
        : IRequestHandler<ToggleAccountByIdCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IValidator<ToggleAccountByIdCommand> _validator = validator;
        private readonly ILogger<ToggleAccountByIdCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ICacheService _cacheService = cacheService;

        public async Task<bool> Handle(ToggleAccountByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Start DeleteAccountByIdCommandHandler for Account Id: {AccountId}", request.Id);

                var validatorResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validatorResult.IsValid)
                    throw new BadRequestException(string.Join(",", validatorResult.Errors.Select(x => x.ErrorMessage)));

                var user = await _userManager.FindByNameAsync(request.ToggledBy);
                if (user is null)
                {
                    _logger.LogInformation("User not found. Username: {ToggledBy}", request.ToggledBy);
                    throw new NotFoundException<ApplicationUser>(request.ToggledBy);
                }
                var accountRepository = _unitOfWork.GetRepository<Account, Guid>();

                var account = await accountRepository.GetAsync(new GetAccountByIdSpecification(request.Id), cancellationToken, true);
                if (account is null)
                {
                    _logger.LogWarning("Account with Id: {AccountId} not found", request.Id);
                    throw new NotFoundException<Account>(request.Id);
                }
                var hasChildren = await accountRepository.AnyAsync(a => a.ParentId == request.Id, cancellationToken);
                if (hasChildren)
                {
                    _logger.LogWarning("Cannot delete Account Id {AccountId} because it has child accounts", request.Id);
                    throw new BadRequestException($"Cannot delete account with Id {request.Id} because it has child accounts.");
                }

                account.Toggle(request.ToggledBy);
                accountRepository.Update(account);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Successfully deleted Account with Id: {AccountId}", request.Id);

                _logger.LogDebug("Cache keys invalidated after toggling account: {AccountId}", account.Id);

                await _cacheService.RemoveAsync($"Khazen_/api/Accounts/{account.Id}");
                await _cacheService.RemoveByPatternAsync("Khazen_/api/Accounts*");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while Deleting Account with Id: {AccountId}", request.Id);
                throw;
            }

        }
    }
}
