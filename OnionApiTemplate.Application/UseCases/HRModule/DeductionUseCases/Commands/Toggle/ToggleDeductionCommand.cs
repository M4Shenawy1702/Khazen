namespace Khazen.Application.UseCases.HRModule.DeductionUseCases.Commands.Delete
{
    public record ToggleDeductionCommand(int Id, string ModifiedBy) : IRequest<bool>;
}
