using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.Auth;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Queries.GetAll
{
    public record GetAllUsersQuery(GetAllUsersQueryParameters Parameters) : IRequest<PaginatedResult<ApplicationUserDto>>;
}
