namespace Khazen.Application.UseCases.HRModule.EmployeeUseCases.Commands.Create
{
    public record CreateEmployeeCommand(CreateEmployeeDto Dto, string CurrentUserId) : IRequest<EmployeeDto>;

}
