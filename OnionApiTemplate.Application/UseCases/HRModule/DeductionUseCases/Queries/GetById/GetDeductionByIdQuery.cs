using Khazen.Application.DOTs.HRModule.DeductionDtos;

namespace Khazen.Application.UseCases.HRModule.DeductionUseCases.Queries.GetById
{
    public record GetDeductionByIdQuery(Guid Id) : IRequest<DeductionDetailsDto>;
}
