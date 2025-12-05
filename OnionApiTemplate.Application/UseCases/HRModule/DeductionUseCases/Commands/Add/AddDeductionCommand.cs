using Khazen.Application.DOTs.HRModule.Deduction;
using Khazen.Application.DOTs.HRModule.DeductionDtos;

namespace Khazen.Application.UseCases.HRModule.DeductionUseCases.Commands.Add
{
    public record AddDeductionCommand(AddDeductionDto Dto, string CreatedBy) : IRequest<DeductionDto>;
}
