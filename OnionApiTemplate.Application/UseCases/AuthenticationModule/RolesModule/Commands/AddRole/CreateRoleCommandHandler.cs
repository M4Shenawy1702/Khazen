using Khazen.Domain.Exceptions;

namespace Khazen.Application.UseCases.AuthService.RolesModule.Commands.AddRole
{
    internal class CreateRoleCommandHandler(RoleManager<IdentityRole> roleManager, IValidator<CreateRoleCommand> validator)
        : IRequestHandler<CreateRoleCommand, bool>
    {
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IValidator<CreateRoleCommand> _validator = validator;

        public async Task<bool> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new BadRequestException(string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));

            var existingRole = await _roleManager.FindByNameAsync(request.RoleName);
            if (existingRole != null)
                throw new BadRequestException($"{request.RoleName} already exists");

            var result = await _roleManager.CreateAsync(new IdentityRole(request.RoleName));
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(x => x.Description)));

            return true;
        }
    }
}
