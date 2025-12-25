using Khazen.Application.DOTs.HRModule.BonusDtos;

namespace Khazen.Application.UseCases.HRModule.BonusUseCases.Queries.GetById
{
    public record GetBonusByIdQuery(Guid Id) : IRequest<BonusDetailsDto>;
}