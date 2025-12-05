using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.DepartmentUseCases.Commands.Delete
{
    internal class ToggleDepartmentHandler(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        ILogger<ToggleDepartmentHandler> logger
    ) : IRequestHandler<ToggleDepartmentCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<ToggleDepartmentHandler> _logger = logger;

        public async Task<bool> Handle(ToggleDepartmentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting ToggleDepartmentHandler for DepartmentId: {DepartmentId}, ModifiedBy: {ModifiedBy}",
                request.Id, request.ModifiedBy);

            try
            {
                var repository = _unitOfWork.GetRepository<Department, Guid>();

                var existingDepartment = await repository.GetByIdAsync(request.Id, cancellationToken);
                if (existingDepartment is null)
                {
                    _logger.LogWarning("Department not found. DepartmentId: {DepartmentId}", request.Id);
                    throw new NotFoundException<Department>(request.Id);
                }

                var user = await _userManager.FindByNameAsync(request.ModifiedBy);
                if (user is null)
                {
                    _logger.LogWarning("User not found. UserId: {ModifiedBy}", request.ModifiedBy);
                    throw new NotFoundException<ApplicationUser>(request.ModifiedBy);
                }

                existingDepartment.Toggle(request.ModifiedBy);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Department toggled successfully. DepartmentId: {DepartmentId}, ModifiedBy: {ModifiedBy}",
                    request.Id, request.ModifiedBy);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling department. DepartmentId: {DepartmentId}, ModifiedBy: {ModifiedBy}",
                    request.Id, request.ModifiedBy);
                throw new ApplicationException("An unexpected error occurred while toggling the department.", ex);
            }
        }
    }
}
