using Khazen.Application.Common.Interfaces;
using Khazen.Application.DOTs.AccountingModule.AccountDtos;
using Khazen.Application.Specification.AccountingModule.AccountSpecifications;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AccountingModule.AccountUseCases.Commands.Update
{
    internal class UpdateAccountByIdCommandHandler(IUnitOfWork unitOfWork,
                                                   IMapper mapper,
                                                   IValidator<UpdateAccountByIdCommand> validator,
                                                   UserManager<ApplicationUser> userManager,
                                                   ILogger<UpdateAccountByIdCommandHandler> logger,
                                                   ICacheService cacheService)
        : IRequestHandler<UpdateAccountByIdCommand, AccountDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<UpdateAccountByIdCommand> _validator = validator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<UpdateAccountByIdCommandHandler> _logger = logger;
        private readonly ICacheService _cacheService = cacheService;

        public async Task<AccountDetailsDto> Handle(UpdateAccountByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Starting UpdateAccountByIdCommandHandler to update account with {AccountId}", request.Id);

                var validatorResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validatorResult.IsValid)
                {
                    _logger.LogWarning("UpdateAccountByIdCommandHandler validation failed for account with {AccountId}", request.Id);
                    throw new BadRequestException(validatorResult.Errors.Select(x => x.ErrorMessage).ToList());
                }
                var user = await _userManager.FindByNameAsync(request.ModifiedBy);
                if (user is null)
                {
                    _logger.LogInformation("User not found. Username: {ModifiedBy}", request.ModifiedBy);
                    throw new NotFoundException<ApplicationUser>(request.ModifiedBy);
                }

                var accountRepository = _unitOfWork.GetRepository<Account, Guid>();
                var account = await accountRepository.GetAsync(new GetAccountByIdSpecification(request.Id), cancellationToken);
                if (account == null)
                {
                    _logger.LogError("Account with {AccountId} not found", request.Id);
                    throw new NotFoundException<Account>(request.Id);
                }
                if (request.Dto.ParentId != null && request.Dto.ParentId != account.ParentId)
                {
                    var parentAccount = await accountRepository.GetAsync(new GetAccountByIdSpecification((Guid)request.Dto.ParentId), cancellationToken);
                    if (parentAccount == null)
                    {
                        _logger.LogError("Parent Account with {ParentAccountId} not found", request.Dto.ParentId);
                        throw new NotFoundException<Account>((Guid)request.Dto.ParentId);
                    }
                }
                await EnsureNoDuplicatesAsync(request, accountRepository, cancellationToken);

                account.Modify(request.Dto.Name, request.Dto.Code, request.Dto.Description, request.Dto.AccountType, request.ModifiedBy, request.Dto.ParentId);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Successfully updated account with {AccountId}", request.Id);

                _logger.LogDebug("Cache keys invalidated after updating account: {AccountId}", account.Id);

                await _cacheService.RemoveByPatternAsync("Khazen_/api/Accounts*");

                return _mapper.Map<AccountDetailsDto>(account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while Updating Account with Name: {AccountName}", request.Dto.Name);
                throw;
            }
        }

        private async Task EnsureNoDuplicatesAsync(UpdateAccountByIdCommand request, IGenericRepository<Account, Guid> accountRepository, CancellationToken cancellationToken)
        {
            var nameTask = accountRepository.AnyAsync(x => x.Name == request.Dto.Name && x.Id != request.Id, cancellationToken);
            var codeTask = accountRepository.AnyAsync(x => x.Code == request.Dto.Code && x.Id != request.Id, cancellationToken);

            await Task.WhenAll(nameTask, codeTask);

            var duplicateErrors = new List<string>();
            if (nameTask.Result) duplicateErrors.Add("Name is already in use.");
            if (codeTask.Result) duplicateErrors.Add("Code is already in use.");

            if (duplicateErrors.Count > 0)
            {
                _logger.LogError("Duplicate errors found: {DuplicateErrors}", duplicateErrors);
                throw new BadRequestException(duplicateErrors);
            }

        }
    }
}
