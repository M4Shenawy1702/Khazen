using Khazen.Application.Common;
using Khazen.Application.Common.QueryParameters;
using Khazen.Application.DOTs.SalesModule.Customer;

namespace Khazen.Application.UseCases.SalesModule.CustomerUsecases.Queries.GetAll
{
    public record GetAllCustomersQuery(CustomerQueryParameters QueryParameters) : IRequest<PaginatedResult<CustomerDto>>;
}
