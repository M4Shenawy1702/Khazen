using Khazen.Application.DOTs.HRModule.SalaryDots;

namespace Khazen.Application.UseCases.HRModule.SalaryUseCases.Queries.GetById
{
    public record GetSalaryByIdQuery(Guid Id) : IRequest<SalaryDetailsDto>;
}
