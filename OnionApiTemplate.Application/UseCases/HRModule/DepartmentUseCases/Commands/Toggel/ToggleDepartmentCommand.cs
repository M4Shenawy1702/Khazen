namespace Khazen.Application.UseCases.HRModule.DepartmentUseCases.Commands.Delete
{
    public record ToggleDepartmentCommand(Guid Id, string ModifiedBy) : IRequest<bool>;
}
