namespace Khazen.Application.UseCases.HRModule.EmployeeUsecases.Queries.GetById
{
    public record GetEmployeeByIdQuery(Guid id) : IRequest<EmployeeDetailsDto>;
}
