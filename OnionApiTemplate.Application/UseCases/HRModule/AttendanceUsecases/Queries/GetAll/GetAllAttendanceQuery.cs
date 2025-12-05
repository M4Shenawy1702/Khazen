using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.HRModule.AttendaceDtos;

namespace Khazen.Application.UseCases.HRModule.AttendanceUsecases.Queries.GetAll
{
    public record GetAllAttendanceQuery(AttendanceQueryParameters QueryParameters) : IRequest<PaginatedResult<AttendanceDto>>;
}
