using Khazen.Application.UseCases.HRModule.DepartmentUseCases.Commands.Update;

namespace Khazen.Application.Validations.HRModule.NewFolder
{
    public class UpdateDepartmentCommandValidation : AbstractValidator<UpdateDepartmentCommand>
    {
        public UpdateDepartmentCommandValidation()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Department ID is required.");
            RuleFor(x => x.Dto.Name)
                .NotEmpty().WithMessage("Department name is required.")
                .MaximumLength(100).WithMessage("Department name cannot exceed 100 characters.");
            RuleFor(x => x.Dto.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
