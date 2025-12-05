using Khazen.Application.DOTs.AccountingModule.AccountDtos;
using Khazen.Application.Specification.AccountingModule.AccountSpecifications;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AccountingModule.AccountUseCases.Queries.GetById
{
    internal class GetAccountByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<GetAccountByIdQuery> validator, ILogger<GetAccountByIdQueryHandler> logger)
        : IRequestHandler<GetAccountByIdQuery, AccountDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<GetAccountByIdQuery> _validator = validator;
        private readonly ILogger<GetAccountByIdQueryHandler> _logger = logger;

        public async Task<AccountDetailsDto> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Handling GetAccountByIdQuery for Id: {Id}", request.Id);
                var validatorResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validatorResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for GetAccountByIdQuery (Id: {Id}): {Errors}",
                        request.Id, string.Join(", ", validatorResult.Errors.Select(e => e.ErrorMessage)));
                    throw new BadRequestException(validatorResult.Errors.Select(x => x.ErrorMessage).ToList());
                }

                var accountRepository = _unitOfWork.GetRepository<Account, Guid>();

                var account = await accountRepository.GetAsync(new GetAccountByIdWithIncludeSpecification(request.Id), cancellationToken, true);

                if (account is null)
                {
                    _logger.LogWarning("Account with Id: {Id} not found", request.Id);
                    throw new NotFoundException<Account>(request.Id);
                }

                _logger.LogInformation("GetAccountByIdQuery for Id: {Id} handled successfully", request.Id);
                return _mapper.Map<AccountDetailsDto>(account);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling GetAccountByIdQuery for Id: {Id}", request.Id);
                throw;
            }

        }
    }
}
