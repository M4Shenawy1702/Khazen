using Khazen.Application.DOTs.InventoryModule.WarehouseDtos;
using Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Commands.Create;
using Khazen.Application.UseCases.InventoryModule.WarehouseUseCases.Commands.Update;
using Khazen.Domain.Entities.InventoryModule;

namespace Khazen.Application.MappingProfile.InventoryModule
{
    public class WareHouseProfile : Profile
    {
        public WareHouseProfile()
        {
            CreateMap<Warehouse, WarehouseDto>();
            CreateMap<CreateWarehouseCommand, Warehouse>();
            CreateMap<UpdateWarehouseCommand, Warehouse>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<WarehouseProduct, WarehouseProductDetailsDto>();
            CreateMap<UpdateWarehouseProductDto, WarehouseProduct>();

        }
    }
}
