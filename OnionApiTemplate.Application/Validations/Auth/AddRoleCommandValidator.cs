using Khazen.Application.UseCases.AuthService.RolesModule.Commands.AddRole;

namespace Khazen.Application.Validations.Auth
{
    public class AddRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public AddRoleCommandValidator()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("Role name is required.")
                .MinimumLength(3).WithMessage("Role name must be at least 3 characters long.")
                .MaximumLength(50).WithMessage("Role name cannot exceed 50 characters.");
        }
    }
}
