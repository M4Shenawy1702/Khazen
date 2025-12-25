using Khazen.Application.UseCases.HRModule.EmployeeUseCases.Commands.Create;

namespace Khazen.Application.Validations.HRModule.EmployeeValidations
{
    public class CreateEmployeeRequestValidation : AbstractValidator<CreateEmployeeCommand>
    {
        public CreateEmployeeRequestValidation()
        {
            RuleFor(x => x.Dto.FirstName)
                .NotEmpty().WithMessage("Full name is required");

            RuleFor(x => x.Dto.LastName)
               .NotEmpty().WithMessage("Full name is required");

            RuleFor(x => x.Dto.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                  .Matches(@"^(?:\+?20|0)?1[0125][0-9]{8}$").WithMessage("Phone number is not valid");

            RuleFor(x => x.Dto.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email format is not valid");

            RuleFor(x => x.Dto.Address)
                .NotEmpty().WithMessage("Address is required");

            RuleFor(x => x.Dto.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required");

            RuleFor(x => x.Dto.DepartmentId)
                .NotEmpty().WithMessage("Department is required");

            RuleFor(x => x.Dto.Gender)
                .IsInEnum().WithMessage("Gender is not valid");

            RuleFor(x => x.Dto.UserName)
                .NotEmpty().WithMessage("Username is required");

            RuleFor(x => x.Dto.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");

            RuleFor(x => x.Dto.BaseSalary)
                .GreaterThan(0).WithMessage("Salary must be greater than zero");

            RuleFor(x => x.Dto.JobTitle)
                .IsInEnum().WithMessage("Job title is not valid");

            RuleFor(x => x.Dto.NationalId)
                .NotEmpty().WithMessage("National ID is required")
                .Matches(@"^\d{14}$").WithMessage("National ID must be 14 digits");

            RuleFor(x => x.Dto.RoleId)
                .NotEmpty().WithMessage("Role is required");
        }
    }
}
