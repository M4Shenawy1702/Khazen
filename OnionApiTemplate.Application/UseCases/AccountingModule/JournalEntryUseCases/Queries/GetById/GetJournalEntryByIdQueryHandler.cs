using Khazen.Application.DOTs.AccountingModule.JournalEntryDots;
using Khazen.Application.Specification.AccountingModule.JurnalEntrySpecificatons;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Queries.GetById
{
    public class GetJournalEntryByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GetJournalEntryByIdQueryHandler> logger) : IRequestHandler<GetJournalEntryByIdQuery, JournalEntryDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetJournalEntryByIdQueryHandler> _logger = logger;

        public async Task<JournalEntryDetailsDto> Handle(GetJournalEntryByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug("Start GetJournalEntryByIdQueryHandler Id: {Id}...", request.Id);
                var repo = _unitOfWork.GetRepository<JournalEntry, Guid>();

                _logger.LogDebug("Executing GetAsync for Journal Entry Id: {Id} with includes.", request.Id);
                var entry = await repo.GetAsync(new GetJurnalEntryByIdWithIncludesSpecification(request.Id), cancellationToken, true);

                if (entry == null)
                {
                    _logger.LogError("Journal entry with Id {Id} was not found", request.Id);
                    throw new NotFoundException<JournalEntry>(request.Id);
                }

                _logger.LogInformation("Journal entry {Number} (Id: {Id}) retrieved successfully. Total lines: {LineCount}.",
                    entry.JournalEntryNumber, entry.Id, entry.Lines.Count);

                _logger.LogDebug("Starting mapping of Journal Entry Id {Id} to DTO.", entry.Id);
                var resultDto = _mapper.Map<JournalEntryDetailsDto>(entry);

                _logger.LogInformation("Successfully mapped Journal Entry Id {Id} to DTO.", entry.Id);

                return resultDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching journal entry with Id {Id}", request.Id);
                throw;
            }
        }
    }
}