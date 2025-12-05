using Khazen.Application.UseCases.AuthService.RolesModule.Commands.RevokeRole;

namespace Khazen.Application.UseCases.AuthService.Commands.RevokeRole
{
    public class RevokeRoleCommandValidator : AbstractValidator<RevokeRoleCommand>
    {
        public RevokeRoleCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("RoleId is required.");
        }
    }
}
