using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.BonusDtos;

namespace Khazen.Application.UseCases.HRModule.BonusUseCases.Queries.GetAll
{
    public record GetAllBounsQuery(BonusQueryParameters QueryParameters) : IRequest<IEnumerable<BonusDto>>;
}
