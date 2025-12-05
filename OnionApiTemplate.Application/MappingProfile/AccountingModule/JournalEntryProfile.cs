using Khazen.Application.DOTs.AccountingModule.JournalEntryDots;
using Khazen.Application.UseCases.AccountingModule.JournalEntryUseCases.Commands.Create;
using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Application.MappingProfile.AccountingModule
{
    public class JournalEntryProfile : Profile
    {
        public JournalEntryProfile()
        {
            CreateMap<JournalEntry, JournalEntryDto>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<JournalEntry, JournalEntryDetailsDto>();
            CreateMap<CreateJournalEntryCommand, JournalEntry>();

            CreateMap<JournalEntryLine, JournalEntryLineDto>();
            CreateMap<CreateJournalEntryLineDto, JournalEntryLine>();
        }
    }
}
