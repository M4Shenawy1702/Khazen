using Khazen.Application.DOTs.PurchaseModule.PurchasePaymentDots;
using Khazen.Domain.Entities.PurchaseModule;

namespace Khazen.Application.MappingProfile.PurchaseModule
{
    public class PurchasePaymentProfile : Profile
    {
        public PurchasePaymentProfile()
        {
            CreateMap<CreatePurchasePaymentDto, PurchasePayment>();
            CreateMap<UpdatePurchasePaymentDto, PurchasePayment>();
            CreateMap<PurchasePayment, PurchasePaymentDto>();
            CreateMap<PurchasePayment, PurchasePaymentDetailsDto>();
        }
    }
}
