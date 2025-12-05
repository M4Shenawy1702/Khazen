using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.AdvanceDtos;

namespace Khazen.Application.UseCases.HRModule.AdvanceUseCases.Queries.GetAll
{
    public record class GetAllAdvanceQuery(AdvanceQueryParameters QueryParameters) : IRequest<IEnumerable<AdvanceDto>>;
}
