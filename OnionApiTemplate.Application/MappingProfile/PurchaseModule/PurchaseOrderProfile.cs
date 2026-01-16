using Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDots;
using Khazen.Application.DOTs.PurchaseModule.PurchaseOrderDtss;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.MappingProfile.PurchaseModule
{
    public class PurchaseOrderProfile : Profile
    {
        public PurchaseOrderProfile()
        {
            CreateMap<CreatePurchaseOrderDto, PurchaseOrder>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<CreatePurchaseOrderItemDto, PurchaseOrderItem>();

            CreateMap<UpdatePurchaseOrderDto, PurchaseOrder>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdatePurchaseOrderItemDto, PurchaseOrderItem>();


            CreateMap<PurchaseOrder, PurchaseOrderDto>()
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier!.Name));

            CreateMap<PurchaseOrder, PurchaseOrderDetailsDto>();

            CreateMap<PurchaseOrderItem, PurchaseOrderItemDto>();
        }
    }
}
