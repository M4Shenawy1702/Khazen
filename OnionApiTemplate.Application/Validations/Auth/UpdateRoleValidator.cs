using Khazen.Application.UseCases.AuthService.RolesModule.Commands.UpdateRole;

namespace Khazen.Application.Validations.Auth
{
    public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
    {
        public UpdateRoleCommandValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("Role ID is required.");

            RuleFor(x => x.NewRoleName)
                .NotEmpty().WithMessage("Role name is required.")
                .MinimumLength(3).WithMessage("Role name must be at least 3 characters long.")
                .MaximumLength(50).WithMessage("Role name cannot exceed 50 characters.");
        }
    }
}
