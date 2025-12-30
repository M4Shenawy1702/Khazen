using Khazen.Application.DOTs.InventoryModule.CategoryDots;

namespace Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Update
{
    public record UpdateCategoryCommand(Guid Id, UpdateCategoryDto Dto, string CurrentUserId) : IRequest<CategoryDetailsDto>;
}
