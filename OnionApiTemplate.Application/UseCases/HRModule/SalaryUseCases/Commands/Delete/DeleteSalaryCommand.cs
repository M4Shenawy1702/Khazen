namespace Khazen.Application.UseCases.HRModule.SalaryUseCases.Commands.Delete
{
    public record DeleteSalaryCommand(Guid SalaryId, string CurrentUserId) : IRequest<bool>;
}
