using Khazen.Application.UseCases.AuthService.RolesModule.Commands.AssignRole;

namespace Khazen.Application.UseCases.AuthService.Commands.AssignRole
{
    public class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
    {
        public AssignRoleCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("RoleId is required.");
        }
    }
}
