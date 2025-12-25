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
                 .ForMember(dest => dest.Lines, opt => opt.MapFrom(src => src.Lines));

            CreateMap<JournalEntry, JournalEntryDetailsDto>();

            CreateMap<CreateJournalEntryCommand, JournalEntry>();

            CreateMap<JournalEntryLine, JournalEntryLineDto>();

            CreateMap<CreateJournalEntryLineDto, JournalEntryLine>();
        }
    }
}
