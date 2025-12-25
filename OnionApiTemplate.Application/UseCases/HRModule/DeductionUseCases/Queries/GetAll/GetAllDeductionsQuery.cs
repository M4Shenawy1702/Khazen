using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.Deduction;

namespace Khazen.Application.UseCases.HRModule.DeductionUseCases.Queries.GetAll
{
    public record GetAllDeductionsQuery(DeductionQueryParameters QueryParameters) : IRequest<PaginatedResult<DeductionDto>>;
}
