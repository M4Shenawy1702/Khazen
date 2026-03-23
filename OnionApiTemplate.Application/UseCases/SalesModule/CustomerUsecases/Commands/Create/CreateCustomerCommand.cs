using Khazen.Application.DOTs.SalesModule.Customer;

namespace Khazen.Application.UseCases.SalesModule.CustomerUsecases.Commands.Create
{
    public record CreateCustomerCommand(CreateCustomerDto Dto, string CurrentUserId) : IRequest<CustomerDto>;
}
