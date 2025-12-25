using Khazen.Application.Common.Interfaces.Authentication;
using Khazen.Application.DOTs.Auth;
using Khazen.Application.Specification.AuthenticationModule.RefreshTokenSpecs;
using Khazen.Application.UseCases.AuthenticationModule.Common;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.GetRefreshToken
{
    internal class GetRefreshTokenCommandHandler(
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IValidator<GetRefreshTokenCommand> validator,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator,
        ILogger<GetRefreshTokenCommandHandler> logger)
        : IRequestHandler<GetRefreshTokenCommand, AuthResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IValidator<GetRefreshTokenCommand> _validator = validator;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator = refreshTokenGenerator;
        private readonly ILogger<GetRefreshTokenCommandHandler> _logger = logger;

        public async Task<AuthResponse> Handle(GetRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Refresh token request received. Token hash: {TokenHash}", request.RefreshToken.GetHashCode());

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for refresh token request. Errors: {Errors}",
                    string.Join(", ", validationResult.Errors.Select(x => x.ErrorMessage)));
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug("Database transaction started for token rotation.");

            try
            {
                var refreshTokenRepo = _unitOfWork.GetRepository<RefreshToken, Guid>();

                var refreshToken = await refreshTokenRepo.GetAsync(new GetRefreshTokenSpec(request.RefreshToken), cancellationToken);

                if (refreshToken == null || !refreshToken.IsActive || refreshToken.User == null || !refreshToken.User.IsActive)
                {
                    _logger.LogWarning("Invalid or expired refresh token used for refresh. Token hash: {TokenHash}", request.RefreshToken.GetHashCode());
                    throw new BadRequestException("Invalid or expired refresh token.");
                }

                _logger.LogInformation("Refresh token validated for user {UserId}", refreshToken.UserId);

                string token = await _jwtTokenGenerator.CreateJWTTokenAsync(refreshToken.User);
                _logger.LogDebug("New JWT access token generated.");

                refreshToken.RevokedAt = DateTime.UtcNow;
                _logger.LogInformation("Old refresh token revoked for user {UserId}", refreshToken.UserId);

                var newRefreshToken = new RefreshToken
                {
                    Token = _refreshTokenGenerator.Generate(),
                    CreatedAt = DateTime.UtcNow,
                    ExpiresOnUtc = DateTime.UtcNow.AddDays(7),
                    UserId = refreshToken.UserId
                };

                await refreshTokenRepo.AddAsync(newRefreshToken, cancellationToken);
                _logger.LogDebug("New refresh token created and added to repository.");

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogDebug("Changes saved to database. Old token revoked, new token created.");

                var userDto = _mapper.Map<ApplicationUserDto>(refreshToken.User);
                userDto.UserRoles = await _userManager.GetRolesAsync(refreshToken.User);
                _logger.LogTrace("User DTO mapped and role retrieved.");

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Transaction committed. Token rotation complete for user {UserId}", newRefreshToken.UserId);

                return new AuthResponse(userDto, token, newRefreshToken.Token, newRefreshToken.ExpiresOnUtc);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "An error occurred during token rotation.");
                throw;
            }
        }
    }
}