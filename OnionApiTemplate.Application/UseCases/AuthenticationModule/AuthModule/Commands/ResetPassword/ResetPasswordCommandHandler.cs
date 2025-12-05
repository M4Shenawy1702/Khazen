using Khazen.Domain.Exceptions;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.ResetPassword
{
    internal class ResetPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IValidator<ResetPasswordCommand> validator)
        : IRequestHandler<ResetPasswordCommand, bool>
    {
        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var validation = await validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                throw new BadRequestException(validation.Errors.Select(e => e.ErrorMessage).ToList());

            var user = await userManager.FindByEmailAsync(request.ResetPasswordDto.Email)
                       ?? throw new NotFoundException<ApplicationUser>(request.ResetPasswordDto.Email);

            var result = await userManager.ResetPasswordAsync(user, request.ResetPasswordDto.Token, request.ResetPasswordDto.NewPassword);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.Select(e => e.Description).ToList());

            var activeRefreshTokens = user.RefreshTokens!.Where(x => x.IsActive).ToList();
            foreach (var rt in activeRefreshTokens)
                rt.RevokedAt = DateTime.UtcNow;

            await userManager.UpdateAsync(user);

            return true;
        }
    }

}
