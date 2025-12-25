namespace Khazen.Application.UseCases.HRModule.EmployeeUsecases.Commands.Update
{
    public record UpdateEmployeeCommand(Guid Id, UpdateEmployeeDto Dto, string CurrentUserId) : IRequest<EmployeeDetailsDto>;
}
