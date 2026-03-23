using Khazen.Application.DOTs.SalesModule.Customer;

namespace Khazen.Application.Common.Interfaces.Authentication
{
    internal interface IUserRegistrationService
    {
        Task<string> RegisterCustomerUserAsync(CreateCustomerDto Dto, string CurrentUserId, CancellationToken cancellationToken = default);
    }
}
