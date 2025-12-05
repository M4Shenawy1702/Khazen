using Khazen.Application.DOTs.InventoryModule.CategoryDots;

namespace Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Queries.GetAll
{
    public record GetAllCategoriesQuery() : IRequest<IEnumerable<CategoryDto>>;
}
