using Khazen.Application.DOTs.HRModule.BonusDtos;

namespace Khazen.Application.UseCases.HRModule.BonusUseCases.Commands.Add
{
    public record AddBonusCommand(AddBonusDto Dto, string CreatedBy) : IRequest<BonusDto>;
}
