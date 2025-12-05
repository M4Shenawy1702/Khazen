using Khazen.Domain.Exceptions;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.VerifyEmail
{
    internal class VerifyEmailCommandHandler(UserManager<ApplicationUser> userManager)
        : IRequestHandler<VerifyEmailCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<bool> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId)
                ?? throw new NotFoundException<ApplicationUser>(request.UserId);

            var result = await _userManager.ConfirmEmailAsync(user, Uri.UnescapeDataString(request.Token));

            if (!result.Succeeded)
                throw new BadRequestException("Invalid or expired email verification token.");

            return true;
        }
    }
}
