using Khazen.Application.BaseSpecifications;
using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.Specification.InventoryModule.CategorySpecifications
{
    internal class GetCategoryByNameSpecification
        : BaseSpecifications<Category>
    {
        public GetCategoryByNameSpecification(string Name)
            : base(c => c.Name == Name)
        {
        }
    }
}
