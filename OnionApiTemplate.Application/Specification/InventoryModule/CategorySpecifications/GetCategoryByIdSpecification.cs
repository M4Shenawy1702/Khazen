using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.BaseSpecifications.InventoryModule.CategorySpecifications
{
    internal class GetCategoryByIdSpecification
        : BaseSpecifications<Category>
    {
        public GetCategoryByIdSpecification(Guid id)
            : base(c => c.Id == id)
        {
            AddInclude(c => c.Products);
        }
    }
}
