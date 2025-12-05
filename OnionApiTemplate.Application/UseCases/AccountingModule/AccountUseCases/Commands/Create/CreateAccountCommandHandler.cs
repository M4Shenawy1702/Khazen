using Khazen.Application.Common.Interfaces;
using Khazen.Application.DOTs.AccountingModule.AccountDtos;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AccountingModule.AccountUseCases.Commands.Create
{
    internal class CreateAccountCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CreateAccountCommand> validator, ICacheService cacheService,
        ILogger<CreateAccountCommandHandler> logger, UserManager<ApplicationUser> userManager)
        : IRequestHandler<CreateAccountCommand, AccountDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CreateAccountCommand> _validator = validator;
        private readonly ICacheService _cacheService = cacheService;
        private readonly ILogger<CreateAccountCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<AccountDetailsDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Start CreateAccountCommand for Account Name: {AccountName}", request.Dto.Name);

                var validatorResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validatorResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for {AccountName}: {Errors}",
                     request.Dto.Name,
                     string.Join(", ", validatorResult.Errors.Select(e => e.ErrorMessage)));
                    throw new BadRequestException(validatorResult.Errors.Select(x => x.ErrorMessage).ToList());
                }
                var user = await _userManager.FindByNameAsync(request.CreatedBy);
                if (user is null)
                {
                    _logger.LogInformation("User not found. Username: {CreatedBy}", request.CreatedBy);
                    throw new NotFoundException<ApplicationUser>(request.CreatedBy);
                }
                var accountRepository = _unitOfWork.GetRepository<Account, Guid>();

                if (request.Dto.ParentId.HasValue)
                {
                    var parentExists = await accountRepository.AnyAsync(a => a.Id == request.Dto.ParentId, cancellationToken);
                    if (!parentExists)
                    {
                        _logger.LogWarning("Parent Account not found for ParentId: {ParentId}", request.Dto.ParentId);
                        throw new NotFoundException<Account>(request.Dto.ParentId.Value);
                    }
                }

                await EnsureNoDuplicatesAsync(request, accountRepository, cancellationToken);

                var account = new Account(request.Dto.Name, request.Dto.Code, request.Dto.Description, request.Dto.AccountType, request.CreatedBy, request.Dto.ParentId);
                await accountRepository.AddAsync(account, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Account created successfully with Id: {AccountId}", account.Id);

                _logger.LogDebug("Cache keys invalidated after creating account: {AccountId}", account.Id);

                await _cacheService.RemoveByPatternAsync("Khazen_/api/Accounts*");

                return _mapper.Map<AccountDetailsDto>(account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating Account with Name: {AccountName}", request.Dto.Name);
                throw;
            }
        }

        private async Task EnsureNoDuplicatesAsync(CreateAccountCommand request, IGenericRepository<Account, Guid> accoutRepository, CancellationToken cancellationToken)
        {
            var nameExistsTask = await accoutRepository.AnyAsync(a => a.Name == request.Dto.Name, cancellationToken);
            var codeExistsTask = await accoutRepository.AnyAsync(a => a.Code == request.Dto.Code, cancellationToken);


            var duplicateErrors = new List<string>();

            if (nameExistsTask) duplicateErrors.Add("Name is already in use.");
            if (codeExistsTask) duplicateErrors.Add("Code is already in use.");

            if (duplicateErrors.Count > 0)
            {
                _logger.LogError("Duplicate errors found: {DuplicateErrors}", duplicateErrors);
                throw new BadRequestException(duplicateErrors);
            }
        }
    }
}
