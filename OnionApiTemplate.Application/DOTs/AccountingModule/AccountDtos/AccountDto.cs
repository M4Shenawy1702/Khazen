using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.AccountingModule.AccountDtos
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public AccountType AccountType { get; set; }
        public Guid? ParentName { get; set; }
        public bool IsDeleted { get; set; }
        public decimal Balance { get; set; }
    }
}
