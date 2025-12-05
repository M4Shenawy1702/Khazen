using Khazen.Application.Common;
using Khazen.Application.DOTs.SalesModule.SalesOrderDtos;
using Khazen.Application.Specification.SalesModule.SalesOrdersSpecifications;
using Khazen.Domain.Entities.SalesModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.SalesModule.SalesOrderUseCases.Queries.GetAll
{
    internal class GetAllSalesOrdersQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<GetAllSalesOrdersQuery> validator,
        ILogger<GetAllSalesOrdersQueryHandler> logger
        )
        : IRequestHandler<GetAllSalesOrdersQuery, PaginatedResult<SalesOrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<GetAllSalesOrdersQuery> _validator = validator;
        private readonly ILogger<GetAllSalesOrdersQueryHandler> _logger = logger;

        public async Task<PaginatedResult<SalesOrderDto>> Handle(GetAllSalesOrdersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAllSalesOrdersQuery. PageIndex: {PageIndex}, PageSize: {PageSize}",
                request.queryParameters.PageIndex, request.queryParameters.PageSize);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for GetAllSalesOrdersQuery: {Errors}",
                    validationResult.Errors.Select(e => e.ErrorMessage).ToList());

                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var salesOrdersRepository = _unitOfWork.GetRepository<SalesOrder, Guid>();

            var data = await salesOrdersRepository.GetAllAsync(
               new GetAllSalesOdersSpecifications(request.queryParameters),
               cancellationToken,
               true);

            var salesOrdersDto = _mapper.Map<List<SalesOrderDto>>(data);

            var salesOrdersCount = await salesOrdersRepository.GetCountAsync(
               new GetAllSalesOdersCountSpecifications(request.queryParameters),
               cancellationToken,
               true);

            _logger.LogInformation("Total sales orders count: {Count}", salesOrdersCount);

            return new PaginatedResult<SalesOrderDto>(
                request.queryParameters.PageIndex,
                request.queryParameters.PageSize,
                salesOrdersCount,
                salesOrdersDto);
        }
    }
}
