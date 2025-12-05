using Khazen.Domain.Exceptions;

namespace Khazen.Application.UseCases.AuthService.RolesModule.Commands.UpdateRole
{
    internal class UpdateRoleCommandHandler(RoleManager<IdentityRole> roleManager, IValidator<UpdateRoleCommand> validator)
        : IRequestHandler<UpdateRoleCommand, bool>
    {
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IValidator<UpdateRoleCommand> _validator = validator;

        public async Task<bool> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new BadRequestException(string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));

            var existingRole = await _roleManager.FindByIdAsync(request.RoleId)
                ?? throw new NotFoundException<IdentityRole>(request.RoleId);

            if (await _roleManager.FindByNameAsync(request.NewRoleName.Trim()) != null)
                throw new BadRequestException($"There is already a role with the name {request.NewRoleName}");

            existingRole.Name = request.NewRoleName;
            var result = await _roleManager.UpdateAsync(existingRole);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(x => x.Description)));
            return true;
        }
    }
}
