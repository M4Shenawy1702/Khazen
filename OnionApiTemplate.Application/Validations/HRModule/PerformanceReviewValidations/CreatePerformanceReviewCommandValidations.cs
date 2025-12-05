using Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Commands.Create;

namespace Khazen.Application.Validations.HRModule.PerformanceReviewValidations
{
    public class CreatePerformanceReviewCommandValidations : AbstractValidator<CreatePerformanceReviewCommand>
    {
        public CreatePerformanceReviewCommandValidations()
        {
            RuleFor(x => x.Dto.EmployeeId)
                .NotEmpty().WithMessage("Employee ID is required.");
            RuleFor(x => x.Dto.ReviewerId)
                .NotEmpty().WithMessage("Reviewer ID is required.");
            RuleFor(x => x.Dto.Comments)
                .MaximumLength(500).WithMessage("Comments cannot exceed 500 characters.");
            RuleFor(x => x.Dto.Rate)
                .InclusiveBetween(1, 5).WithMessage("Rate must be between 1 and 5.");
            RuleFor(x => x.Dto.ActionPlan)
                .MaximumLength(500).WithMessage("Action plan cannot exceed 500 characters.");
        }
    }
}
