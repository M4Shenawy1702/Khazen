using Khazen.Domain.Common.Enums;
using Khazen.Domain.Exceptions;

namespace Khazen.Domain.Entities.AccountingModule
{
    public class Account
        : BaseEntity<Guid>
    {
        public Account(string name, string code, string description, AccountType accountType, string createdBy, Guid? parentId)
        {
            Id = Guid.NewGuid();
            Name = name;
            Code = code;
            Description = description;
            AccountType = accountType;
            ParentId = parentId;
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
            Balance = 0;
        }
        public void Modify(string name, string code, string description, AccountType accountType, string modifiedBy, Guid? parentId)
        {
            Name = name;
            Code = code;
            Description = description;
            AccountType = accountType;
            ParentId = parentId;
            ModifiedBy = modifiedBy;
            ModifiedAt = DateTime.UtcNow;
            Balance = 0;
        }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public AccountType AccountType { get; set; } = AccountType.Asset;

        public Guid? ParentId { get; set; }
        public Account? Parent { get; set; }

        public DateTime? ToggledAt { get; set; }
        public string? ToggledBy { get; set; }

        public decimal Balance { get; set; } //todo : Calculate Balance from JournalEntryLines
        public ICollection<Account> Children { get; set; } = [];
        public ICollection<JournalEntryLine> JournalEntryLines { get; set; } = [];

        public void ApplyDebit(decimal amount) => Balance += amount;
        public void ApplyCredit(decimal amount)
        {
            if (Balance - amount < 0)
                throw new BadRequestException("Insufficient balance for credit operation.");
            Balance -= amount;
        }

        public void Toggle(string toggleBy)
        {
            IsDeleted = !IsDeleted;
            ToggledAt = DateTime.UtcNow;
            ToggledBy = toggleBy;
        }
    }
}
