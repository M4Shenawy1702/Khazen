using Khazen.Application.DOTs.InventoryModule.CategoryDots;

namespace Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Queries.GetById
{
    public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDetailsDto>;
}
