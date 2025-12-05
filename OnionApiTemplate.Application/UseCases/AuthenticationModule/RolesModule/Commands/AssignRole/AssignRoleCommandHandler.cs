using Khazen.Domain.Exceptions;

namespace Khazen.Application.UseCases.AuthService.RolesModule.Commands.AssignRole
{
    internal class AssignRoleCommandHandler(RoleManager<IdentityRole> roleManager, IValidator<AssignRoleCommand> validator, UserManager<ApplicationUser> userManager)
        : IRequestHandler<AssignRoleCommand, bool>
    {
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IValidator<AssignRoleCommand> _validator = validator;
        private readonly UserManager<ApplicationUser> _userManager = userManager;


        public async Task<bool> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new BadRequestException(string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));

            var user = await _userManager.FindByIdAsync(request.UserId) ??
                throw new NotFoundException<ApplicationUser>(request.UserId);

            var role = await _roleManager.FindByIdAsync(request.RoleId) ??
                throw new NotFoundException<IdentityRole>(request.RoleId);

            var result = await _userManager.AddToRoleAsync(user, role.Name!);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(x => x.Description)));
            return true;
        }
    }
}
