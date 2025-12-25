using Khazen.Application.Specification.AccountingModule.JurnalEntrySpecificatons;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.SalaryUseCases.Commands.Delete
{
    internal class DeleteSalaryCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<DeleteSalaryCommandHandler> logger) : IRequestHandler<DeleteSalaryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<DeleteSalaryCommandHandler> _logger = logger;

        public async Task<bool> Handle(DeleteSalaryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Initiating Salary Reversal: ID {SalaryId} by User {User}",
                request.SalaryId, request.CurrentUserId);

            var salaryRepo = _unitOfWork.GetRepository<Salary, Guid>();
            var journalRepo = _unitOfWork.GetRepository<JournalEntry, Guid>();

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var salary = await salaryRepo.GetByIdAsync(request.SalaryId, cancellationToken);
                if (salary == null)
                {
                    _logger.LogWarning("Salary {Id} not found.", request.SalaryId);
                    throw new NotFoundException<Salary>(request.SalaryId);
                }

                if (salary.IsDeleted)
                {
                    _logger.LogWarning("Salary {Id} is already deleted.", request.SalaryId);
                    throw new BadRequestException($"Salary {request.SalaryId} is already deleted.");
                }

                _logger.LogDebug("Invalidating salary record {Id}", request.SalaryId);
                salary.MarkAsDeleted(request.CurrentUserId);
                salaryRepo.Update(salary);


                var relatedJournal = await journalRepo.GetAsync(new GetJournalEntryByReferenceSpec(salary.Id), cancellationToken);

                if (relatedJournal != null)
                {
                    _logger.LogInformation("Reversing related Journal Entry: {JournalId}", relatedJournal.Id);

                    var reversalEntry = relatedJournal.CreateReversal(request.CurrentUserId, $"Reversal of Salary {salary.SalaryDate:MM/yyyy}");
                    await journalRepo.AddAsync(reversalEntry, cancellationToken);
                }
                else
                {
                    _logger.LogWarning("No related journal entry found for Salary {Id}. HR deleted without accounting reversal.", salary.Id);
                }

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Salary {Id} and its accounting impact successfully reversed.", request.SalaryId);

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Failed to delete Salary {Id}. Transaction rolled back.", request.SalaryId);
                throw;
            }
        }
    }
}
