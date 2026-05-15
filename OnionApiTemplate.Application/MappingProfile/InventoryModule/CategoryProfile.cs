using Khazen.Application.DOTs.InventoryModule.CategoryDots;
using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.MappingProfile.InventoryModule
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>();
            CreateMap<Category, CategoryDetailsDto>();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
