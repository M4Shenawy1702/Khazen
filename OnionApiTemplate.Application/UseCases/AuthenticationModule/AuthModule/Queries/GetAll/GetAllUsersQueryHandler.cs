using Khazen.Application.Common;
using Khazen.Application.DOTs.Auth;
using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.AuthenticationModule.AuthModule.Queries.GetAll
{
    internal class GetAllUsersQueryHandler(
        UserManager<ApplicationUser> userManager,
        IMapper mapper,
        ILogger<GetAllUsersQueryHandler> logger,
        IValidator<GetAllUsersQuery> validator)
        : IRequestHandler<GetAllUsersQuery, PaginatedResult<ApplicationUserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetAllUsersQueryHandler> _logger = logger;
        private readonly IValidator<GetAllUsersQuery> _validator = validator;

        public async Task<PaginatedResult<ApplicationUserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing GetAllUsersQuery: Page={Page}, Size={Size}, UserType={UserType}",
                request.Parameters.PageIndex, request.Parameters.PageSize, request.Parameters.UserType);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("GetAllUsersQuery validation failed: {Errors}", string.Join(", ", errors));
                throw new BadRequestException(errors);
            }

            var baseQuery = _userManager.Users.AsNoTracking();
            var absoluteTotalCount = await baseQuery.CountAsync(cancellationToken);

            var filteredQuery = baseQuery;

            if (request.Parameters.UserType.HasValue)
            {
                filteredQuery = filteredQuery.Where(u => u.UserType == request.Parameters.UserType.Value);
            }

            if (request.Parameters.IsDeleted.HasValue)
            {
                bool targetIsActive = !request.Parameters.IsDeleted.Value;
                filteredQuery = filteredQuery.Where(u => u.IsActive == targetIsActive);
            }

            var filteredTotalCount = await filteredQuery.CountAsync(cancellationToken);

            if (filteredTotalCount == 0 && absoluteTotalCount > 0)
            {
                _logger.LogInformation("Query returned no users matching filter criteria.");
                return PaginatedResult<ApplicationUserDto>.Empty(request.Parameters.PageIndex, request.Parameters.PageSize);
            }

            var pagedQuery = filteredQuery
                .Skip((request.Parameters.PageIndex - 1) * request.Parameters.PageSize)
                .Take(request.Parameters.PageSize);

            var users = await pagedQuery.ToListAsync(cancellationToken);

            var usersDto = _mapper.Map<IEnumerable<ApplicationUserDto>>(users);

            _logger.LogInformation("Successfully retrieved {UserCount} users (Filtered Total: {TotalCount})", users.Count, filteredTotalCount);

            return new PaginatedResult<ApplicationUserDto>(
                request.Parameters.PageIndex,
                request.Parameters.PageSize,
                filteredTotalCount,
                usersDto);
        }
    }
}