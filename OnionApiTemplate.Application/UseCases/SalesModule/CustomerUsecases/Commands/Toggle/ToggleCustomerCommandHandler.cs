using Khazen.Application.UseCases.SalesModule.CustomerUsecases.Commands.Delete;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.CustomerUsecases.Commands.Toggle
{
    public class ToggleCustomerCommandHandler(
        IUnitOfWork unitOfWork,
        IValidator<ToggleCustomerCommand> validator,
        ILogger<ToggleCustomerCommandHandler> logger,
        UserManager<ApplicationUser> userManager)
                : IRequestHandler<ToggleCustomerCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IValidator<ToggleCustomerCommand> _validator = validator;
        private readonly ILogger<ToggleCustomerCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<bool> Handle(ToggleCustomerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to toggle Customer state. CustomerId: {Id}, RequestedBy: {AdminId}",
               request.Id, request.CurrentUserId);

            var user = await _userManager.FindByNameAsync(request.CurrentUserId);
            if (user == null)
            {
                _logger.LogError("Identity User {UserId} not found.", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }

            var repo = _unitOfWork.GetRepository<Customer, Guid>();
            var customer = await repo.GetByIdAsync(request.Id, cancellationToken);

            if (customer == null)
            {
                _logger.LogError("Customer {Id} not found.", request.Id);
                throw new NotFoundException<Customer>(request.Id);
            }

            bool initialState = customer.IsDeleted;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug("Database transaction started for CustomerId: {Id}", request.Id);

            try
            {
                if (customer.UserId != null)
                {
                    _logger.LogInformation("Customer is linked to Identity User {UserId}. Syncing lockout status.", customer.UserId);

                    var targetUser = await _userManager.FindByIdAsync(customer.UserId);
                    if (targetUser == null)
                    {
                        _logger.LogError("Identity User {UserId} not found for Customer {Id}.", customer.UserId, request.Id);
                        throw new NotFoundException<ApplicationUser>(customer.UserId);
                    }


                    bool willBeDisabled = customer.IsDeleted;

                    targetUser.IsActive = !willBeDisabled;
                    var lockout = willBeDisabled ? DateTimeOffset.MaxValue : (DateTimeOffset?)null;

                    await _userManager.SetLockoutEndDateAsync(targetUser, lockout);
                    await _userManager.UpdateSecurityStampAsync(targetUser);

                    var identityResult = await _userManager.UpdateAsync(targetUser);

                    if (!identityResult.Succeeded)
                    {
                        var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                        _logger.LogError("Identity update failed for User {UserId}. Errors: {Errors}", customer.UserId, errors);
                        throw new Exception($"Identity Sync Error: {errors}");
                    }

                    _logger.LogDebug("Identity User {UserId} lockout status updated to: {LockoutStatus}",
                        customer.UserId, willBeDisabled ? "Locked" : "Unlocked");
                }

                customer.Toggle(user.Id);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation(
                    "Customer {Id} state toggled successfully. State Change: {From} -> {To}. Action performed by: {AdminName}",
                    request.Id, initialState, !initialState, user.UserName);

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                _logger.LogCritical(ex, "CRITICAL FAILURE: Toggle operation failed for Customer {Id}. Transaction rolled back.", request.Id);

                throw;
            }
        }
    }
}
