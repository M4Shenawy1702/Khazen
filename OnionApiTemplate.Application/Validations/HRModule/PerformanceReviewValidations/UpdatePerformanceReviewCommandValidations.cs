using Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Commands.Update;

namespace Khazen.Application.Validations.HRModule.PerformanceReviewValidations
{
    public class UpdatePerformanceReviewCommandValidations : AbstractValidator<UpdatePerformanceReviewCommand>
    {
        public UpdatePerformanceReviewCommandValidations()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Performance Review ID is required.");
            RuleFor(x => x.Dto.Rate)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
            RuleFor(x => x.Dto.Comments)
                .MaximumLength(500).WithMessage("Comments cannot exceed 500 characters.");
            RuleFor(x => x.Dto.ActionPlans)
                .MaximumLength(500).WithMessage("Action plan cannot exceed 500 characters.");
            RuleFor(x => x.Dto.ReviewDate)
                .NotEmpty().WithMessage("Review date is required.")
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now)).WithMessage("Review date cannot be in the future.");
        }
    }
}
