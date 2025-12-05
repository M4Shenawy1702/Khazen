using Khazen.Domain.Exceptions;

namespace Khazen.Application.UseCases.AuthService.RolesModule.Commands.RevokeRole
{
    internal class RevokeRoleCommandHandler(
        RoleManager<IdentityRole> roleManager,
        IValidator<RevokeRoleCommand> validator,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<RevokeRoleCommand, bool>
    {
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IValidator<RevokeRoleCommand> _validator = validator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        public async Task<bool> Handle(RevokeRoleCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new BadRequestException(string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));

            var user = await _userManager.FindByIdAsync(request.UserId)
                ?? throw new NotFoundException<ApplicationUser>(request.UserId);

            var role = await _roleManager.FindByIdAsync(request.RoleId)
                ?? throw new NotFoundException<IdentityRole>(request.RoleId);

            if (!await _userManager.IsInRoleAsync(user, role.Name!))
                throw new BadRequestException($"User '{user.UserName}' is not assigned to the role '{role.Name}'.");

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name!);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join("; ", result.Errors.Select(e => e.Description)));

            return true;
        }
    }
}
