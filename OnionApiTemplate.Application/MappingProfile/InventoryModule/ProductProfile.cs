using Khazen.Application.DOTs.InventoryModule.ProductDtos;
using Khazen.Application.UseCases.InventoryModule.ProductUseCases.Commands.Add;
using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.MappingProfile.InventoryModule
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<AddProductCommand, Product>()
                .ForMember(dest => dest.Image, opt => opt.Ignore());

            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category!.Name))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand!.Name));

            CreateMap<Product, ProductSnapshotDto>();

            CreateMap<Product, ProductDetailsDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category!.Name))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand!.Name))
                .ForMember(dest => dest.SupplierNames, opt => opt.MapFrom(src => src.SupplierProducts!.Select(x => x.Supplier!.Name)))
                .ForMember(dest => dest.WarehouseNames, opt => opt.MapFrom(src => src.WarehouseProducts!.Select(x => x.Warehouse!.Name)))
                ;
        }
    }

}
