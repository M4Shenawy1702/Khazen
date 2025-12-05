using Khazen.Domain.Common.Enums;

namespace Khazen.Application.DOTs.AccountingModule.AccountDtos
{
    public class CreateAccountDto
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public AccountType AccountType { get; set; }
        public Guid? ParentId { get; set; }
    }
}
