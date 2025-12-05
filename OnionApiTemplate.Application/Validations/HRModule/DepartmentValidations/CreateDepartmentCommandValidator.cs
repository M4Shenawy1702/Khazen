using Khazen.Application.UseCases.HRModule.DepartmentUseCases.Commands.Create;

namespace Khazen.Application.Validations.HRModule.NewFolder
{
    public class CreateDepartmentCommandValidation : AbstractValidator<CreateDepartmentCommand>
    {
        public CreateDepartmentCommandValidation()
        {
            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Department name is required.")
                .MaximumLength(100).WithMessage("Department name cannot exceed 100 characters.");
            RuleFor(x => x.Dto.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
