using Khazen.Application.DOTs.PurchaseModule.PurchaseReceiptDtos;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.MappingProfile.PurchaseModule
{
    public class PurchaseReceiptProfile : Profile
    {
        public PurchaseReceiptProfile()
        {
            CreateMap<CreatePurchaseReceiptDto, PurchaseReceipt>();

            CreateMap<CreatePurchaseReceiptItemDto, PurchaseReceiptItem>();

            CreateMap<UpdatePurchaseReceiptDto, PurchaseReceipt>()
                .ForMember(dest => dest.Items, opt => opt.Ignore());
            CreateMap<UpdatePurchaseReceiptItemDto, PurchaseReceiptItem>();

            CreateMap<PurchaseReceipt, PurchaseReceiptDto>()
             .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
             .ReverseMap();

            CreateMap<PurchaseReceiptItem, PurchaseReceiptItemDto>().ReverseMap();

        }
    }
}
