using Khazen.Application.Common;
using Khazen.Application.DOTs.AccountingModule.AccountDtos;
using Khazen.Application.Specification.AccountingModule.AccountSpecifications;
using Khazen.Domain.Entities.AccountingModule;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AccountingModule.AccountUseCases.Queries.GetAll
{
    internal class GetAllAccountsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetAllAccountsQueryHandler> logger)
        : IRequestHandler<GetAllAccountsQuery, PaginatedResult<AccountDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetAllAccountsQueryHandler> _logger = logger;

        public async Task<PaginatedResult<AccountDto>> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Start Handling GetAllAccountsQueryHandler ");
                var accountRepository = _unitOfWork.GetRepository<Account, Guid>();
                var accounts = await accountRepository.GetAllAsync(new GetAllAccountsSpec(request.QueryParameters), cancellationToken, false);
                var data = _mapper.Map<IEnumerable<AccountDto>>(accounts);
                var count = await accountRepository.GetCountAsync(new GetAllAccountsCountSpec(), cancellationToken);
                _logger.LogDebug("Finished Handling GetAllAccountsQueryHandler returned : {Count} items", data.Count());
                return new PaginatedResult<AccountDto>(request.QueryParameters.PageIndex, request.QueryParameters.PageSize, count, data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling GetAllAccountsQueryHandler: {Message}", ex.Message);
                throw;
            }

        }
    }
}
