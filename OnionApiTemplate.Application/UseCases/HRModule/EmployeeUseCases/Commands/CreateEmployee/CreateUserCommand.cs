namespace Khazen.Application.UseCases.HRModule.EmployeeUseCases.Commands.CreateEmployee
{
    public record CreateEmployeeCommand(CreateEmployeeDto Dto, string CreatedBy) : IRequest<EmployeeDto>;

}
