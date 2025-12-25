using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;

namespace Khazen.Application.UseCases.HRModule.SalaryUseCases.Queries.GetAll
{
    public record GetAllSalariesQuery(SalariesQueryParameters QueryParameters) : IRequest<PaginatedResult<SalaryDto>>;
}
