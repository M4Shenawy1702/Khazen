using Khazen.Application.DOTs.HRModule.SalaryDots;

namespace Khazen.Application.UseCases.HRModule.SalaryUseCases.Commands.Create
{
    public record CreateSalaryCommand(CreateSalaryDto Dto, string CreatedBy) : IRequest<SalaryDto>;
}
