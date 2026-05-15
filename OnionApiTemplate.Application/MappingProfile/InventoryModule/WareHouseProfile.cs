using Khazen.Application.DOTs.InventoryModule.WarehouseDtos;
using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.MappingProfile.InventoryModule
{
    public class WareHouseProfile : Profile
    {
        public WareHouseProfile()
        {
            CreateMap<Warehouse, WarehouseDto>();
            CreateMap<Warehouse, WarehouseDetailsDto>();
            CreateMap<CreateWarehouseDto, Warehouse>();
            CreateMap<UpdateWarehouseDto, Warehouse>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<WarehouseProduct, WarehouseProductDetailsDto>();
            CreateMap<UpdateWarehouseProductDto, WarehouseProduct>();

        }
    }
}
