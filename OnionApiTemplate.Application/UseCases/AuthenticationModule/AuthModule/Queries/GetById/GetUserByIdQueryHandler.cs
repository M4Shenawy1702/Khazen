using Khazen.Application.DOTs.Auth;
using Khazen.Application.UseCases.AuthenticationModule.AuthModule.Queries.GetById;
using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Queries
{
    internal class GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper, ILogger<GetUserByIdQueryHandler> logger)
        : IRequestHandler<GetUserByIdQuery, ApplicationUserDto>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetUserByIdQueryHandler> _logger = logger;

        public async Task<ApplicationUserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve user details for UserId: {UserId}", request.Id);

            var user = await _userManager.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User not found for requested UserId: {UserId}", request.Id);
                throw new NotFoundException<ApplicationUser>($"User with ID {request.Id} not found.");
            }

            _logger.LogDebug("User found: {Email}. Mapping DTO and roles.", user.Email);

            var userDto = _mapper.Map<ApplicationUserDto>(user);

            userDto.UserRoles = (await _userManager.GetRolesAsync(user)).ToList();

            _logger.LogInformation("Successfully retrieved user {UserId}  ", user.Id);

            return userDto;
        }
    }
}