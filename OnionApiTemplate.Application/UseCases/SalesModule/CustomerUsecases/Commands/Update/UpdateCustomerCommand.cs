using Khazen.Application.DOTs.SalesModule.Customer;

namespace Khazen.Application.UseCases.SalesModule.CustomerUsecases.Commands.Update
{
    public record UpdateCustomerCommand(Guid Id, UpdateCustomerDto Dto, string ModifiedBy) : IRequest<CustomerDto>;
}
