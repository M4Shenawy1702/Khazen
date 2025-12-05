using Khazen.Application.DOTs.SalesModule.Customer;

namespace Khazen.Application.UseCases.SalesModule.CustomerUsecases.Queries.GetById
{
    public record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerDetailsDto>;
}
