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
            _logger.LogInformation("Attempting to toggle department state for ID: {DepartmentId}", request.Id);

            var repository = _unitOfWork.GetRepository<Department, Guid>();

            var existingDepartment = await repository.GetByIdAsync(request.Id, cancellationToken);
            if (existingDepartment is null)
            {
                _logger.LogWarning("Department {DepartmentId} not found", request.Id);
                throw new NotFoundException<Department>(request.Id);
            }

            var user = await _userManager.FindByNameAsync(request.CurrentUserId);
            if (user is null)
            {
                _logger.LogError("User {UserId} not found", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }

            existingDepartment.Toggle(request.CurrentUserId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Department {DepartmentId} toggled successfully (IsDeleted: {Status})",
                request.Id, existingDepartment.IsDeleted);

            return true;
        }
    }
}