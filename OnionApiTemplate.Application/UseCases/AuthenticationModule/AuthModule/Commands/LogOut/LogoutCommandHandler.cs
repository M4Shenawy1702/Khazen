using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.LogOut
{
    internal class LogoutCommandHandler(
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        IValidator<LogoutCommand> validator)
        : IRequestHandler<LogoutCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IValidator<LogoutCommand> _validator = validator;

        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());

            var user = await _userManager.Users
                .Include(x => x.RefreshTokens)
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken)
                ?? throw new NotFoundException<ApplicationUser>(request.UserId);


            var activeTokens = user.RefreshTokens?.Where(rt => rt.IsActive).ToList();
            if (activeTokens == null || activeTokens.Count == 0)
                return true;

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                foreach (var token in activeTokens)
                {
                    token.RevokedAt = DateTime.UtcNow;
                }

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
