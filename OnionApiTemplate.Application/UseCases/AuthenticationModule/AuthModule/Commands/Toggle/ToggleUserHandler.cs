using Khazen.Domain.Exceptions;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.Toggle
{
    internal class ToggleUserHandler(UserManager<ApplicationUser> userManager)
        : IRequestHandler<ToggleUserCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        public async Task<bool> Handle(ToggleUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id)
                ?? throw new NotFoundException<ApplicationUser>(request.Id);

            user.IsActive = !user.IsActive;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(x => x.Description)));

            return true;
        }
    }
}
