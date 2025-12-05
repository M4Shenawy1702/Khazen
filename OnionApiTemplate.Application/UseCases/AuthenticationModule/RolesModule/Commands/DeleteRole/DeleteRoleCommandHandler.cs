using Khazen.Domain.Exceptions;

namespace Khazen.Application.UseCases.AuthService.RolesModule.Commands.DeleteRole
{
    internal class DeleteRoleCommandHandler(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
        : IRequestHandler<DeleteRoleCommand, bool>
    {
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId)
                ?? throw new NotFoundException<IdentityRole>(request.RoleId);

            var users = await _userManager.GetUsersInRoleAsync(role.Name!);
            if (users.Any())
                throw new BadRequestException("Cannot delete a role that is assigned to one or more users.");

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(x => x.Description)));

            return true;
        }
    }
}
