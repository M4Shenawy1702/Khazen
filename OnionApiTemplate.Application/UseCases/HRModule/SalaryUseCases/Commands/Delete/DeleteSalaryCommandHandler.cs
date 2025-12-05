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
            _logger.LogInformation("Starting deletion process for SalaryId: {SalaryId} by User: {ModifiedBy}",
                request.SalaryId, request.ModifiedBy);
            try
            {
                var salaryRepo = _unitOfWork.GetRepository<Salary, Guid>();
                var salary = await salaryRepo.GetByIdAsync(request.SalaryId, cancellationToken);

                if (salary == null)
                {
                    _logger.LogWarning("Salary record not found for SalaryId: {SalaryId}", request.SalaryId);
                    throw new NotFoundException<Salary>(request.SalaryId);
                }

                _logger.LogDebug("Toggling salary status for SalaryId: {SalaryId}", request.SalaryId);

                salary.Toggle(request.ModifiedBy);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Salary deleted (toggled) successfully for SalaryId: {SalaryId}", request.SalaryId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting SalaryId: {SalaryId}", request.SalaryId);
                throw;
            }
        }
    }
}
