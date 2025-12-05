using Khazen.Application.BaseSpecifications;

namespace Khazen.Application.Specification.AuthenticationModule.RefreshTokenSpecs
{
    internal class GetRefreshTokenSpec : BaseSpecifications<RefreshToken>
    {
        public GetRefreshTokenSpec(string refreshToken)
            : base(rt => rt.Token == refreshToken)
        {
            AddInclude(rt => rt.User);
        }
    }
}
