using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Delete
{
    internal class DeleteProductCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteProductCommandHandler> logger)
        : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<DeleteProductCommandHandler> _logger = logger;

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting DeleteProductCommand for Product ID: {ProductId}", request.Id);

            var productRepository = _unitOfWork.GetRepository<Product, Guid>();
            var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
            if (product is null)
            {
                _logger.LogWarning("Attempted to delete a non-existing product with ID: {ProductId}", request.Id);
                throw new NotFoundException<Product>(request.Id);
            }
            productRepository.Delete(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Product {Id} deleted successfully", product.Id);

            return true;
        }
    }
}
