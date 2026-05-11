using Khazen.Application.DOTs.InventoryModule.BrandDtos;
using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.Mappings.InventoryModule
{
    public class BrandProfile : Profile
    {
        public BrandProfile()
        {
            CreateMap<Brand, BrandDto>();

            CreateMap<Brand, BrandDetailsDto>();

            CreateMap<CreateBrandDto, Brand>();

            CreateMap<UpdateBrandDto, Brand>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
