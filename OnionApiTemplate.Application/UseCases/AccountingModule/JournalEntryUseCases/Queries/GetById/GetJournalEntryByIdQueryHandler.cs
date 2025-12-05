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
                _logger.LogDebug("Start GetJournalEntryByIdQueryHandler Id: {Id} ...", request.Id);
                var repo = _unitOfWork.GetRepository<JournalEntry, Guid>();
                var entry = await repo.GetAsync(new GetJurnalEntryByIdWithIncludesSpecification(request.Id), cancellationToken, true);
                if (entry == null)
                {
                    _logger.LogError("Journal entry with Id {Id} was not found", request.Id);
                    throw new NotFoundException<JournalEntry>(request.Id);
                }
                _logger.LogInformation("Journal entry with Id {Id} retrieved successfully.", entry.Id);

                return _mapper.Map<JournalEntryDetailsDto>(entry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching journal entry with Id {Id}", request.Id);
                throw;
            }
        }
    }
}
