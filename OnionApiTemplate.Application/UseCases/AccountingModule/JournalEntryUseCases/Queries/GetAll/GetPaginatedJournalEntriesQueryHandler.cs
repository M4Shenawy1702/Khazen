using Khazen.Application.Common;
using Khazen.Application.DOTs.AccountingModule.JournalEntryDots;
using Khazen.Application.Specification.AccountingModule.JurnalEntrySpecificatons;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Queries.GetAll
{
    internal class GetPaginatedJournalEntriesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetPaginatedJournalEntriesQueryHandler> logger, IValidator<GetPaginatedJournalEntriesQuery> validator)
        : IRequestHandler<GetPaginatedJournalEntriesQuery, PaginatedResult<JournalEntryDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetPaginatedJournalEntriesQueryHandler> _logger = logger;
        private readonly IValidator<GetPaginatedJournalEntriesQuery> _validator = validator;

        public async Task<PaginatedResult<JournalEntryDto>> Handle(GetPaginatedJournalEntriesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Starting GetPaginatedJournalEntriesQueryHandler...");

                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogError("Validation failed: {Errors}", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }
                var repo = _unitOfWork.GetRepository<JournalEntry, Guid>();

                var entries = await repo.GetAllAsync(new GetAllJournalEntriesSpecification(request.QueryParameters), cancellationToken, true);
                var entriesList = entries.ToList();
                var data = _mapper.Map<IReadOnlyList<JournalEntryDto>>(entriesList);

                var count = await repo.GetCountAsync(new GetAllJournalEntriesCountSpecification(request.QueryParameters), cancellationToken, true);

                _logger.LogInformation(
                     "Fetched {Count} journal entries | PageIndex: {PageIndex}, PageSize: {PageSize}",
                     entriesList.Count,
                     request.QueryParameters.PageIndex,
                     request.QueryParameters.PageSize
                 );

                return new PaginatedResult<JournalEntryDto>(
                    request.QueryParameters.PageIndex,
                    request.QueryParameters.PageSize,
                    count,
                    data
                );

            }
            catch (Exception ex)
            {

                _logger.LogError(ex,
                 "Error fetching journal entries | PageIndex: {PageIndex}, PageSize: {PageSize}, TransactionType: {TransactionType}",
                 request.QueryParameters.PageIndex,
                 request.QueryParameters.PageSize,
                 request.QueryParameters.TransactionType);

                throw;

            }
        }
    }
}
