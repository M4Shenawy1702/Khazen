namespace Khazen.Application.UseCases.HRModule.EmployeeUsecases.Queries.GetById
{
    public record GetEmployeeByIdQuery(Guid Id) : IRequest<EmployeeDetailsDto>;
}
