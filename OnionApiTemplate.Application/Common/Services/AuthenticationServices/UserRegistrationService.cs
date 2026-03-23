using Khazen.Application.Common.Interfaces.Authentication;
using Khazen.Application.DOTs.SalesModule.Customer;
using Khazen.Domain.Common.Consts;
using Khazen.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.Common.Services.AuthenticationServices
{
    public class UserRegistrationService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ILogger<UserRegistrationService> logger)
        : IUserRegistrationService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly ILogger<UserRegistrationService> _logger = logger;

        public async Task<string> RegisterCustomerUserAsync(CreateCustomerDto Dto, string CurrentUserId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting registration for username: {UserName}, email: {Email}", Dto.UserName, Dto.Email);

            var emailTask = _userManager.FindByEmailAsync(Dto.Email);
            var usernameTask = _userManager.FindByNameAsync(Dto.UserName);
            var phoneTask = _userManager.Users.AnyAsync(u => u.PhoneNumber == Dto.PhoneNumber, cancellationToken);

            await Task.WhenAll(emailTask, usernameTask, phoneTask);

            var duplicateErrors = new List<string>();

            if (emailTask.Result is not null)
            {
                duplicateErrors.Add("Email is already in use.");
                _logger.LogWarning("Duplicate email detected: {Email}", Dto.Email);
            }

            if (usernameTask.Result is not null)
            {
                duplicateErrors.Add("Username is already in use.");
                _logger.LogWarning("Duplicate username detected: {UserName}", Dto.UserName);
            }

            if (phoneTask.Result)
            {
                duplicateErrors.Add("Phone number is already in use.");
                _logger.LogWarning("Duplicate phone number detected: {Phone}", Dto.PhoneNumber);
            }

            if (duplicateErrors.Count > 0)
                throw new BadRequestException(duplicateErrors);

            var user = new ApplicationUser
            {
                UserName = Dto.UserName,
                Email = Dto.Email,
                PhoneNumber = Dto.PhoneNumber,
                FullName = Dto.Name,
                Address = Dto.Address,
                UserType = UserType.Customer,
                CreatedBy = CurrentUserId
            };

            _logger.LogInformation("Creating identity user: {UserName}", user.UserName);

            var createResult = await _userManager.CreateAsync(user, Dto.Password);
            if (!createResult.Succeeded)
            {
                _logger.LogError("User creation failed: {@Errors}", createResult.Errors);
                throw new BadRequestException(createResult.Errors.Select(e => e.Description).ToList());
            }

            var role = await _roleManager.FindByNameAsync(AppRoles.Customer);
            if (role is null)
            {
                _logger.LogError("Customer role '{Role}' not found.", AppRoles.Customer);
                throw new NotFoundException($"Customer role '{AppRoles.Customer}' not found.");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role.Name!);
            if (!roleResult.Succeeded)
            {
                _logger.LogError("Assigning role '{Role}' failed: {@Errors}", role.Name, roleResult.Errors);
                throw new BadRequestException(roleResult.Errors.Select(e => e.Description).ToList());
            }

            _logger.LogInformation("Customer user registered successfully. UserId: {Id}", user.Id);

            return user.Id;
        }
    }
}
