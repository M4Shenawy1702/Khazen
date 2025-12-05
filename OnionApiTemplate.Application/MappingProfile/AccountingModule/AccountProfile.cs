using Khazen.Application.DOTs.AccountingModule.AccountDtos;
using Khazen.Application.UseCases.AccountingModule.AccountUseCases.Commands.Create;
using Khazen.Application.UseCases.AccountingModule.AccountUseCases.Commands.Update;
using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Application.MappingProfile.AccountingModule
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Account, AccountDto>();
            CreateMap<Account, AccountDetailsDto>()
                .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.Parent!.Name));
            CreateMap<CreateAccountCommand, Account>();
            CreateMap<UpdateAccountByIdCommand, Account>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
