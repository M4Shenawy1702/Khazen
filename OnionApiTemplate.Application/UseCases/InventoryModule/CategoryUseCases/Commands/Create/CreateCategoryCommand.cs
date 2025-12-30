using Khazen.Application.DOTs.InventoryModule.CategoryDots;

namespace Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Create
{
    public record CreateCategoryCommand(CreateCategoryDto Dto, string CurrentUserId) : IRequest<CategoryDto>;
}
