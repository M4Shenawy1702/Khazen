using Khazen.Application.DOTs.HRModule.AttendaceDtos;

namespace Khazen.Application.UseCases.HRModule.AttendanceUsecases.Queries.GetById
{
    public record GetAttendanceByIdQuery(Guid Id) : IRequest<AttendanceDto>;
}
