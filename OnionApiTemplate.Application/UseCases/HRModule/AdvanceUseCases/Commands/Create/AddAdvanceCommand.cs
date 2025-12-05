using Khazen.Application.DOTs.HRModule.AdvanceDtos;

namespace Khazen.Application.UseCases.HRModule.AdvanceUseCases.Commands.Create
{
    public record AddAdvanceCommand(AddAdvanceDto Dto, string CreatedBy) : IRequest<AdvanceDto>;
}
