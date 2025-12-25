namespace Khazen.Application.UseCases.HRModule.DeductionUseCases.Commands.Delete
{
    public record ToggleDeductionCommand(Guid Id, string ToggledBy) : IRequest<bool>;
}
