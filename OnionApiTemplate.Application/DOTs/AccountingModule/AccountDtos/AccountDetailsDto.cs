using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.AccountingModule;

namespace Khazen.Application.DOTs.AccountingModule.AccountDtos
{
    public class AccountDetailsDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public AccountType AccountType { get; set; }
        public Guid? ParentName { get; set; }
        public ICollection<AccountDto> Children { get; set; } = [];
        public bool IsDeleted { get; set; }
        public decimal Balance { get; set; }
        public DateTime? ToggledAt { get; set; }
        public string? ToggledBy { get; set; }
        public ICollection<JournalEntryLine> JournalEntryLines { get; set; } = [];
    }
}
