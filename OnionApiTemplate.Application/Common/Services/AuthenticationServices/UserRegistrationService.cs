using Khazen.Application.Common.Interfaces.Authentication;
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

        public async Task<string> RegisterCustomerUserAsync(
            string userName,
            string email,
            string phoneNumber,
            string fullName,
            string address,
            string password,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting registration for username: {UserName}, email: {Email}", userName, email);

            var emailTask = _userManager.FindByEmailAsync(email);
            var usernameTask = _userManager.FindByNameAsync(userName);
            var phoneTask = _userManager.Users.AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);

            await Task.WhenAll(emailTask, usernameTask, phoneTask);

            var duplicateErrors = new List<string>();

            if (emailTask.Result is not null)
            {
                duplicateErrors.Add("Email is already in use.");
                _logger.LogWarning("Duplicate email detected: {Email}", email);
            }

            if (usernameTask.Result is not null)
            {
                duplicateErrors.Add("Username is already in use.");
                _logger.LogWarning("Duplicate username detected: {UserName}", userName);
            }

            if (phoneTask.Result)
            {
                duplicateErrors.Add("Phone number is already in use.");
                _logger.LogWarning("Duplicate phone number detected: {Phone}", phoneNumber);
            }

            if (duplicateErrors.Count > 0)
                throw new BadRequestException(duplicateErrors);

            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                PhoneNumber = phoneNumber,
                FullName = fullName,
                Address = address,
                UserType = UserType.Customer
            };

            _logger.LogInformation("Creating identity user: {UserName}", user.UserName);

            var createResult = await _userManager.CreateAsync(user, password);
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
