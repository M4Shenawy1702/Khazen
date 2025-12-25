using Khazen.Application.Common.Interfaces.Authentication;
using Khazen.Application.DOTs.Auth;
using Khazen.Application.UseCases.AuthenticationModule.Common;
using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.Login
{
    internal class LoginCommandHandler(UserManager<ApplicationUser> userManager,
        IValidator<LoginCommand> loginValidator,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        ILogger<LoginCommandHandler> logger,
         IRecaptchaService recaptchaService)
        : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IValidator<LoginCommand> _loginValidator = loginValidator;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator = refreshTokenGenerator;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<LoginCommandHandler> _logger = logger;
        private readonly IRecaptchaService _recaptchaService = recaptchaService;

        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting login for email: {Email}", request.Dto.Email);

            var validationResult = await _loginValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Login validation failed for email {Email}: {Errors}", request.Dto.Email, string.Join(", ", errors));
                throw new BadRequestException(errors);
            }

            if (request.Dto.RecaptchaToken is null)
            {
                _logger.LogWarning("Recaptcha token was missing but required for email: {Email}", request.Dto.Email);
                throw new BadRequestException("Recaptcha token is required.");
            }

            _logger.LogInformation("Validating Recaptcha token for email: {Email}", request.Dto.Email);
            var recaptchaResult = await _recaptchaService.VerifyAsync(request.Dto.RecaptchaToken, request.Dto.Email);

            _logger.LogInformation("Recaptcha validation passed (Score: {Score}) for email: {Email}",
                recaptchaResult.Score, request.Dto.Email);

            var user = await _userManager.Users
               .Include(u => u.RefreshTokens)
               .FirstOrDefaultAsync(u => u.Email == request.Dto.Email, cancellationToken: cancellationToken);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Dto.Password))
            {
                _logger.LogWarning("Login failed for email {Email}: Invalid credentials.", request.Dto.Email);
                throw new InvalidCredentialsException();
            }

            if (!user.EmailConfirmed)
            {
                _logger.LogWarning("Login failed for email {Email}: Email not confirmed.", request.Dto.Email);
                throw new BadRequestException("Please verify your email before logging in.");
            }

            _logger.LogDebug("User {UserId} authenticated successfully. Mapping DTO and retrieving roles.", user.Id);

            var userDto = _mapper.Map<ApplicationUserDto>(user);
            userDto.UserRoles = await _userManager.GetRolesAsync(user);

            var activeRefreshToken = user.RefreshTokens!
                .FirstOrDefault(rt => rt.IsActive && rt.ExpiresOnUtc > DateTime.UtcNow);

            if (activeRefreshToken != null)
            {
                _logger.LogInformation("Returning existing active refresh token for user {UserId}.", user.Id);
                return new AuthResponse(
                    userDto,
                    await _jwtTokenGenerator.CreateJWTTokenAsync(user),
                    activeRefreshToken.Token,
                    activeRefreshToken.ExpiresOnUtc);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var refreshToken = new RefreshToken
                {
                    Token = _refreshTokenGenerator.Generate(),
                    ExpiresOnUtc = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow,
                    UserId = user.Id
                };

                user.LastLoginAt = DateTime.UtcNow;
                user.RefreshTokens!.Add(refreshToken);

                _logger.LogDebug("Generating new refresh token for user {UserId}. Saving changes...", user.Id);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("User {UserId} logged in successfully. New tokens generated and saved.", user.Id);

                return new AuthResponse(userDto, await _jwtTokenGenerator.CreateJWTTokenAsync(user), refreshToken.Token, refreshToken.ExpiresOnUtc);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token generation and save for user {UserId}. Rolling back transaction.", user.Id);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}