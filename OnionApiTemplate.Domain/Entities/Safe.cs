using Khazen.Domain.Entities.AccountingModule;
using System.ComponentModel.DataAnnotations;

namespace Khazen.Domain.Entities
{
    public class Safe : BaseEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public SafeType Type { get; set; }
        public decimal Balance { get; set; } = 0;
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        public Account Account { get; set; } = null!;

        public ICollection<SafeTransaction> SafeTransactions { get; set; } = [];

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }

    public enum SafeType
    {
        Cash,
        Bank,
        Wallet
    }

}
