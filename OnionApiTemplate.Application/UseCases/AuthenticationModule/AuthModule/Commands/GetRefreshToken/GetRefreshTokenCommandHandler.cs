using Khazen.Application.Common.Interfaces.Authentication;
using Khazen.Application.DOTs.Auth;
using Khazen.Application.Specification.AuthenticationModule.RefreshTokenSpecs;
using Khazen.Application.UseCases.AuthenticationModule.Common;
using Khazen.Domain.Exceptions;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.GetRefreshToken
{
    internal class GetRefreshTokenCommandHandler(UserManager<ApplicationUser> userManager, IMapper mapper, IUnitOfWork unitOfWork, IValidator<GetRefreshTokenCommand> validator, IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator)
        : IRequestHandler<GetRefreshTokenCommand, AuthResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IValidator<GetRefreshTokenCommand> _validator = validator;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator = refreshTokenGenerator;

        public async Task<AuthResponse> Handle(GetRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var refreshTokenRepo = _unitOfWork.GetRepository<RefreshToken, Guid>();
                var refreshToken = await refreshTokenRepo.GetAsync(new GetRefreshTokenSpec(request.RefreshToken), cancellationToken);

                if (refreshToken == null || !refreshToken.IsActive || refreshToken.User == null || !refreshToken.User.IsActive)
                    throw new BadRequestException("Invalid or expired refresh token.");

                string token = await _jwtTokenGenerator.CreateJWTTokenAsync(refreshToken.User);

                refreshToken.RevokedAt = DateTime.UtcNow;

                var newRefreshToken = new RefreshToken
                {
                    Token = _refreshTokenGenerator.Generate(),
                    CreatedAt = DateTime.UtcNow,
                    ExpiresOnUtc = DateTime.UtcNow.AddDays(7),
                    UserId = refreshToken.UserId
                };

                await _unitOfWork.GetRepository<RefreshToken, Guid>().AddAsync(newRefreshToken, cancellationToken);

                await _unitOfWork.SaveChangesAsync();

                var userDto = _mapper.Map<ApplicationUserDto>(refreshToken.User);
                userDto.Role = (await _userManager.GetRolesAsync(refreshToken.User)).FirstOrDefault() ?? string.Empty;


                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new AuthResponse(userDto, token, newRefreshToken.Token, newRefreshToken.ExpiresOnUtc);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }

        }
    }
}
