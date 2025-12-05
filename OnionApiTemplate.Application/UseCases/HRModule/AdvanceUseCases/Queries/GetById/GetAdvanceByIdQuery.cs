using Khazen.Application.DOTs.HRModule.AdvanceDtos;

namespace Khazen.Application.UseCases.HRModule.AdvanceUseCases.Queries.GetById
{
    public record GetAdvanceByIdQuery(int Id) : IRequest<AdvanceDetailsDto>;
}