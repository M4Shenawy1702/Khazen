using Khazen.Application.Common.Interfaces.Authentication;
using Khazen.Application.DOTs.Auth;
using Khazen.Application.UseCases.AuthenticationModule.Common;
using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Commands.ChangePassword
{
    internal class ChangePasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IValidator<ChangePasswordCommand> loginValidator,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator,
        IMapper mapper,
        IUnitOfWork unitOfWork)
        : IRequestHandler<ChangePasswordCommand, AuthResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IValidator<ChangePasswordCommand> _loginValidator = loginValidator;
        private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator = refreshTokenGenerator;
        private readonly IMapper _mapper = mapper;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public async Task<AuthResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _loginValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var user = await _userManager.Users
                    .Include(x => x.RefreshTokens)
                    .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken: cancellationToken) ??
                    throw new NotFoundException<ApplicationUser>(request.UserId);

                if (await _userManager.CheckPasswordAsync(user, request.Dto.CurrentPassword) == false)
                    throw new BadRequestException("Old password is incorrect");

                var result = await _userManager.ChangePasswordAsync(user, request.Dto.CurrentPassword, request.Dto.NewPassword);
                if (!result.Succeeded)
                    throw new BadRequestException(result.Errors.Select(e => e.Description).ToList());

                var activeRefreshTokens = user.RefreshTokens!.Where(x => x.IsActive).ToList();
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
                await _userManager.UpdateAsync(user);

                var userDto = _mapper.Map<ApplicationUserDto>(user);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new AuthResponse(userDto, await _jwtTokenGenerator.CreateJWTTokenAsync(user), newRefreshToken.Token, newRefreshToken.ExpiresOnUtc);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }

        }
    }
}
