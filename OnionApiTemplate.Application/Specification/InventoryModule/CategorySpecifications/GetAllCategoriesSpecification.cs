using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.BaseSpecifications.InventoryModule.CategorySpecifications
{
    internal class GetAllCategoriesSpecification
        : BaseSpecifications<Category>
    {
        public GetAllCategoriesSpecification() : base(c => true)
        {
        }
    }
}
