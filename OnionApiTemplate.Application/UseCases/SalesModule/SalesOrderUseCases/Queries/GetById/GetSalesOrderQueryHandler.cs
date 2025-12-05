using Khazen.Application.DOTs.SalesModule.SalesOrderDtos;
using Khazen.Application.Specification.SalesModule.SalesOrdersSpecifications;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Queries.GetById
{
    internal class GetSalesOrderQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<GetSalesOrderQuery> validator,
        ILogger<GetSalesOrderQueryHandler> logger
        ) : IRequestHandler<GetSalesOrderQuery, SalesOrderDetailsDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<GetSalesOrderQuery> _validator = validator;
        private readonly ILogger<GetSalesOrderQueryHandler> _logger = logger;

        public async Task<SalesOrderDetailsDto> Handle(GetSalesOrderQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetSalesOrderQuery for OrderId: {OrderId}", request.Id);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for GetSalesOrderQuery. Errors: {Errors}",
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList());

                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var salesOrdersRepository = _unitOfWork.GetRepository<SalesOrder, Guid>();

            var salesOrder = await salesOrdersRepository.GetAsync(
               new GetSalesOrderWithIncludesSpecifications(request.Id),
               cancellationToken,
               true);

            if (salesOrder is null)
            {
                _logger.LogWarning("Sales order not found. OrderId: {OrderId}", request.Id);
                throw new NotFoundException<SalesOrder>(request.Id);
            }

            _logger.LogInformation("Sales order retrieved successfully. OrderId: {OrderId}", request.Id);

            var salesOrderDto = _mapper.Map<SalesOrderDetailsDto>(salesOrder);
            return salesOrderDto;
        }
    }
}
