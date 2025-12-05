using Khazen.Application.BaseSpecifications.InventoryModule.BrandSpecifications;
using Khazen.Application.DOTs.InventoryModule.BrandDtos;
using Khazen.Application.Specification.InventoryModule.BrandSpecifications;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.BrandUseCases.Commands.Create
{
    internal class CreateBrandCommandHandler(
        IUnitOfWork _unitOfWork,
        IMapper _mapper,
        IValidator<CreateBrandCommand> _validator,
        ILogger<CreateBrandCommandHandler> _logger)
        : IRequestHandler<CreateBrandCommand, BrandDto>
    {
        public async Task<BrandDto> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting CreateBrandCommandHandler for Brand Name: {BrandName}", request.Dto.Name);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogError("Validation failed: {@Errors}", validationResult.Errors);
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            try
            {
                var repo = _unitOfWork.GetRepository<Brand, Guid>();

                if (await repo.GetAsync(new GetBrandByNameSpecification(request.Dto.Name), cancellationToken, true) is not null)
                {
                    _logger.LogInformation("Brand with Name '{Name}' already exists", request.Dto.Name);
                    throw new AlreadyExistsException<Brand>($"with Name '{request.Dto.Name}'");
                }

                if (!string.IsNullOrEmpty(request.Dto.ContactEmail) &&
                    await repo.GetAsync(new GetBrandByContactEmailSpecification(request.Dto.ContactEmail), cancellationToken, true) is not null)
                {
                    _logger.LogInformation("Brand with ContactEmail '{ContactEmail}' already exists", request.Dto.ContactEmail);
                    throw new AlreadyExistsException<Brand>($"with ContactEmail '{request.Dto.ContactEmail}'");
                }

                if (!string.IsNullOrEmpty(request.Dto.WebsiteUrl) &&
                    await repo.GetAsync(new GetBrandByWebsiteUrlSpecification(request.Dto.WebsiteUrl), cancellationToken, true) is not null)
                {
                    _logger.LogInformation("Brand with WebsiteUrl '{WebsiteUrl}' already exists", request.Dto.WebsiteUrl);
                    throw new AlreadyExistsException<Brand>($"with WebsiteUrl '{request.Dto.WebsiteUrl}'");
                }

                var brand = _mapper.Map<Brand>(request.Dto);
                brand.CreatedBy = request.CreatedBy;
                brand.CreatedAt = DateTime.UtcNow;

                await repo.AddAsync(brand, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Brand {BrandName} was created successfully", request.Dto.Name);

                return _mapper.Map<BrandDto>(brand);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating brand");
                throw new ApplicationException("An unexpected error occurred while creating the brand.");
            }
        }

    }
}
