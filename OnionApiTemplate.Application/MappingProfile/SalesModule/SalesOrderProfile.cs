using Khazen.Application.DOTs.SalesModule.SalesOrderDtos;
using Khazen.Application.DOTs.SalesModule.SalesOrderItemDtos;
using Khazen.Domain.Entities.SalesModule;

namespace Khazen.Application.MappingProfile.SalesModule
{
    public class SalesOrderProfile : Profile
    {
        public SalesOrderProfile()
        {

            CreateMap<SalesOrder, SalesOrderDto>();
            CreateMap<SalesOrder, SalesOrderDetailsDto>();
            CreateMap<CreateSalesOrderDto, SalesOrder>();
            CreateMap<UpdateSalesOrderDto, SalesOrder>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<SalesOrderItem, SalesOrderItemDto>();
        }
    }
}
