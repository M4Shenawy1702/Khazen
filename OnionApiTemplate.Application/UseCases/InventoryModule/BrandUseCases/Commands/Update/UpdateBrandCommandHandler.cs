using Khazen.Application.BaseSpecifications.InventoryModule.BrandSpecifications;
using Khazen.Application.DOTs.InventoryModule.BrandDtos;
using Khazen.Application.Specification.InventoryModule.BrandSpecifications;
using Khazen.Application.UseCases.InventoryModule.BrandUseCases.Commands.Update;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

internal class UpdateBrandCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ILogger<UpdateBrandCommandHandler> logger,
    IValidator<UpdateBrandCommand> validator)
    : IRequestHandler<UpdateBrandCommand, BrandDetailsDto>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<UpdateBrandCommandHandler> _logger = logger;
    private readonly IValidator<UpdateBrandCommand> _validator = validator;

    public async Task<BrandDetailsDto> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Starting UpdateBrandCommandHandler for Brand ID: {BrandId}", request.Id);

        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Validation failed: {@Errors}", validationResult.Errors);
            throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        }

        try
        {
            var repo = _unitOfWork.GetRepository<Brand, Guid>();

            var brand = await repo.GetAsync(new GetBrandByIdSpecification(request.Id), cancellationToken);
            if (brand is null)
            {
                _logger.LogWarning("Brand with Id '{Id}' was not found.", request.Id);
                throw new NotFoundException<Brand>(request.Id);
            }

            if (await repo.GetAsync(new GetBrandByNameSpecification(request.Dto.Name), cancellationToken, true) is not null)
            {
                _logger.LogWarning("Brand with Name '{Name}' already exists.", request.Dto.Name);
                throw new AlreadyExistsException<Brand>($"with Name '{request.Dto.Name}'");
            }

            if (!string.IsNullOrEmpty(request.Dto.ContactEmail) &&
                await repo.GetAsync(new GetBrandByContactEmailSpecification(request.Dto.ContactEmail), cancellationToken, true) is not null)
            {
                _logger.LogWarning("Brand with ContactEmail '{ContactEmail}' already exists.", request.Dto.ContactEmail);
                throw new AlreadyExistsException<Brand>($"with ContactEmail '{request.Dto.ContactEmail}'");
            }

            if (!string.IsNullOrEmpty(request.Dto.WebsiteUrl) &&
                await repo.GetAsync(new GetBrandByWebsiteUrlSpecification(request.Dto.WebsiteUrl), cancellationToken, true) is not null)
                throw new AlreadyExistsException<Brand>($"with WebsiteUrl '{request.Dto.WebsiteUrl}'");
            _mapper.Map(request.Dto, brand);
            brand.ModifiedBy = request.ModifiedBy;
            brand.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Brand '{BrandName}' (ID: {BrandId}) updated successfully.",
                brand.Name, brand.Id);

            return _mapper.Map<BrandDetailsDto>(brand);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error occurred while updating Brand ID: {BrandId}",
                request.Id);

            throw new BadRequestException("An unexpected error occurred while updating the brand.");
        }
    }
}
