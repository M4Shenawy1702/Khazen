using Khazen.Application.Common.Interfaces.Authentication;
using Khazen.Application.DOTs.Auth;
using Khazen.Application.UseCases.AuthenticationModule.Common;
using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.Login
{
    public class LoginCommandHandler(UserManager<ApplicationUser> userManager,
        IValidator<LoginCommand> loginValidator,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator,
        IMapper mapper,
        IUnitOfWork unitOfWork)
        : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IValidator<LoginCommand> _loginValidator = loginValidator;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator = refreshTokenGenerator;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<AuthResponse> Handle(LoginCommand query, CancellationToken cancellationToken)
        {
            var validationResult = await _loginValidator.ValidateAsync(query, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                throw new BadRequestException(errors);
            }

            var user = await _userManager.Users
             .Include(u => u.RefreshTokens)
             .FirstOrDefaultAsync(u => u.Email == query.Email, cancellationToken: cancellationToken);

            if (user == null || !await _userManager.CheckPasswordAsync(user, query.Password))
                throw new InvalidCredentialsException();

            if (!user.EmailConfirmed)
                throw new BadRequestException("Please verify your email before logging in.");

            var userDto = _mapper.Map<ApplicationUserDto>(user);
            userDto.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? string.Empty;

            if (user.RefreshTokens!.Any(rt => rt.IsActive))
            {
                var ActiveRefreshToken = user.RefreshTokens!.FirstOrDefault(x => x.ExpiresOnUtc > DateTime.UtcNow);
                if (ActiveRefreshToken != null)
                    return new AuthResponse(userDto, await _jwtTokenGenerator.CreateJWTTokenAsync(user), ActiveRefreshToken.Token, ActiveRefreshToken.ExpiresOnUtc);
            }

            var refreshToken = new RefreshToken
            {
                Token = _refreshTokenGenerator.Generate(),
                ExpiresOnUtc = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id
            };
            user.LastLoginAt = DateTime.UtcNow;
            user.RefreshTokens!.Add(refreshToken);

            var refreshTokenRepository = _unitOfWork.GetRepository<RefreshToken, Guid>();
            await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponse(userDto, await _jwtTokenGenerator.CreateJWTTokenAsync(user), refreshToken.Token, refreshToken.ExpiresOnUtc);
        }
    }
}
