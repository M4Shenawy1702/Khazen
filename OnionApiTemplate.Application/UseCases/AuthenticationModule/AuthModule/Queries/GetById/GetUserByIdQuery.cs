using Khazen.Application.DOTs.Auth;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Queries.GetById
{
    internal record GetUserByIdQuery(string Id) : IRequest<ApplicationUserDto>
    {
    }
}
