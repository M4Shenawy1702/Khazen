using Khazen.Application.BaseSpecifications.HRModule.EmployeeSpecifications;
using Khazen.Application.BaseSpecifications.HRModule.PerformanceReviewSpesifications;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.PerformanceReviewUseCases.Commands.Create
{
    internal class CreatePerformanceReviewCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreatePerformanceReviewCommand> validator,
        ILogger<CreatePerformanceReviewCommandHandler> logger,
        UserManager<ApplicationUser> userManager
    ) : IRequestHandler<CreatePerformanceReviewCommand, PerformanceReviewDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CreatePerformanceReviewCommand> _validator = validator;
        private readonly ILogger<CreatePerformanceReviewCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<PerformanceReviewDto> Handle(CreatePerformanceReviewCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting CreatePerformanceReviewCommandHandler for EmployeeId: {EmployeeId}, ReviewerId: {ReviewerId}",
                request.Dto.EmployeeId, request.Dto.ReviewerId);

            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for PerformanceReview: {Errors}",
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var user = await _userManager.FindByNameAsync(request.CurrentUserId);
            if (user is null)
            {
                _logger.LogError("User {UserId} not found", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }

            if (request.Dto.ReviewerId == request.Dto.EmployeeId)
            {
                _logger.LogWarning("Attempt to create self-review for EmployeeId: {EmployeeId}", request.Dto.EmployeeId);
                throw new BadRequestException(["Cannot Review Yourself"]);
            }
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var employeeRepository = _unitOfWork.GetRepository<Employee, Guid>();
                var employee = await employeeRepository.GetAsync(new GetEmployeeByIdSpecification(request.Dto.EmployeeId), cancellationToken);

                if (employee is null)
                {
                    _logger.LogWarning("Employee not found with Id: {EmployeeId}", request.Dto.EmployeeId);
                    throw new NotFoundException<Employee>(request.Dto.EmployeeId);
                }
                var reviewer = await employeeRepository.GetAsync(new GetEmployeeByIdSpecification(request.Dto.ReviewerId), cancellationToken);
                if (reviewer is null)
                {
                    _logger.LogWarning("Reviewer not found with Id: {ReviewerId}", request.Dto.ReviewerId);
                    throw new NotFoundException<Employee>(request.Dto.ReviewerId);
                }

                var performanceReviewRepository = _unitOfWork.GetRepository<PerformanceReview, Guid>();
                var reviewExisting = await performanceReviewRepository.GetAsync(
                    new GetPerformanceReviewByEmpIdAndDate(request.Dto.EmployeeId),
                    cancellationToken
                );

                if (reviewExisting is not null)
                {
                    _logger.LogWarning("Performance review already exists for EmployeeId: {EmployeeId}", request.Dto.EmployeeId);
                    throw new BadRequestException(["Performance Review Already Exists"]);
                }

                _logger.LogInformation("Creating new performance review for EmployeeId: {EmployeeId} by ReviewerId: {ReviewerId}",
                    request.Dto.EmployeeId, request.Dto.ReviewerId);

                var performanceReview = _mapper.Map<PerformanceReview>(request.Dto);
                performanceReview.CreatedAt = DateTime.UtcNow;
                performanceReview.CreatedBy = user.UserName!;

                await performanceReviewRepository.AddAsync(performanceReview, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                var performanceReviewDto = _mapper.Map<PerformanceReviewDto>(performanceReview);
                performanceReviewDto.EmployeeName = $"{employee.FirstName} {employee.LastName}";


                _logger.LogInformation("Performance review successfully created with Id: {ReviewId}", performanceReview.Id);

                return performanceReviewDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating Performance Review for EmployeeId: {EmployeeId}", request.Dto.EmployeeId);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
