using Khazen.Application.DOTs.InventoryModule.CategoryDots;
using Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Create;
using Khazen.Application.UseCases.InventoryModule.CategoryUseCases.Commands.Update;
using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.MappingProfile.InventoryModule
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>();
            CreateMap<Category, CategoryDetailsDto>();
            CreateMap<CreateCategoryCommand, Category>();
            CreateMap<UpdateCategoryCommand, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
