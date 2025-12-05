using Khazen.Application.DOTs.InventoryModule.BrandDtos;
using Khazen.Application.UseCases.InventoryModule.BrandUseCases.Commands.Create;
using Khazen.Application.UseCases.InventoryModule.BrandUseCases.Commands.Update;
using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.Mappings.InventoryModule
{
    public class BrandProfile : Profile
    {
        public BrandProfile()
        {
            CreateMap<Brand, BrandDto>();

            CreateMap<Brand, BrandDetailsDto>();

            CreateMap<CreateBrandCommand, Brand>();

            CreateMap<UpdateBrandCommand, Brand>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
