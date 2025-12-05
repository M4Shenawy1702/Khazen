namespace Khazen.Application.Common.Interfaces.Authentication
{
    internal interface IUserRegistrationService
    {
        Task<string> RegisterCustomerUserAsync(string userName, string email, string phoneNumber, string fullName, string address, string password, CancellationToken cancellationToken = default);
    }
}
