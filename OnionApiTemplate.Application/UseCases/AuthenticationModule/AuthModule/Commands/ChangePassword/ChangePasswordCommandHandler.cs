using Khazen.Application.Common.Interfaces.Authentication;
using Khazen.Application.DOTs.Auth;
using Khazen.Application.UseCases.AuthenticationModule.Common;
using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.ChangePassword
{
    internal class ChangePasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IValidator<ChangePasswordCommand> loginValidator,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        ILogger<ChangePasswordCommandHandler> logger)
        : IRequestHandler<ChangePasswordCommand, AuthResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IValidator<ChangePasswordCommand> _loginValidator = loginValidator;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator = refreshTokenGenerator;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ChangePasswordCommandHandler> _logger = logger;

        public async Task<AuthResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting password change for User ID: {UserId}", request.UserId);

            var validationResult = await _loginValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                _logger.LogWarning("Validation failed for password change for user {UserId}: {Errors}", request.UserId, string.Join(", ", errors));
                throw new BadRequestException(errors);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var user = await _userManager.Users
                    .Include(x => x.RefreshTokens)
                    .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
                if (user is null)
                {
                    _logger.LogWarning("User not found for password change: {UserId}", request.UserId);
                    throw new NotFoundException<ApplicationUser>(request.UserId);
                }

                if (!user.EmailConfirmed)
                {
                    _logger.LogWarning("Change password  requested for unconfirmed email {Email}. Failing silently.", user.Email);
                    throw new BadRequestException("Password reset requested for unconfirmed email. Failing silently.");
                }

                if (string.IsNullOrEmpty(request.RowVersionBase64))
                {
                    _logger.LogWarning("Missing If-Match header (RowVersion) for user {UserId}", request.UserId);
                    throw new BadRequestException("The If-Match header (RowVersion) is required for concurrency control.");
                }

                byte[] rowVersionBytes;

                rowVersionBytes = Convert.FromBase64String(request.RowVersionBase64);

                user.SetRowVersion(rowVersionBytes);

                var result = await _userManager.ChangePasswordAsync(user, request.Dto.CurrentPassword, request.Dto.NewPassword);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Password change failed for user {UserId}: {Errors}",
                        request.UserId, string.Join(", ", result.Errors.Select(e => e.Description)));

                    if (result.Errors.Any(e => e.Code == "PasswordMismatch"))
                        throw new BadRequestException("Current password is incorrect");

                    throw new BadRequestException(result.Errors.Select(e => e.Description).ToList());
                }

                var activeRefreshTokens = user.RefreshTokens!.Where(x => x.IsActive && x.ExpiresOnUtc > DateTime.UtcNow).ToList();
                _logger.LogDebug("Revoking {Count} active refresh tokens for user {UserId}.", activeRefreshTokens.Count, user.Id);

                foreach (var rt in activeRefreshTokens)
                    rt.RevokedAt = DateTime.UtcNow;

                var newRefreshToken = new RefreshToken
                {
                    Token = _refreshTokenGenerator.Generate(),
                    ExpiresOnUtc = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow,
                    UserId = user.Id
                };
                user.RefreshTokens.Add(newRefreshToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Password successfully changed and {Count} tokens revoked for user {UserId}.", activeRefreshTokens.Count, user.Id);

                var userDto = _mapper.Map<ApplicationUserDto>(user);
                return new AuthResponse(userDto, await _jwtTokenGenerator.CreateJWTTokenAsync(user), newRefreshToken.Token, newRefreshToken.ExpiresOnUtc);
            }
            catch (FormatException)
            {
                throw new BadRequestException("The If-Match header value is not a valid Base64 string.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency conflict during password change for user {UserId}. Rolling back.", request.UserId);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw new ConcurrencyException("The user record was modified by another user. Please refresh and try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during password change for user {UserId}. Rolling back transaction.", request.UserId);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}