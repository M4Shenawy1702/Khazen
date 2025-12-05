using Khazen.Domain.Common.Enums;

namespace Khazen.Domain.Entities.UsersModule
{
    public class AuditLog
        : BaseEntity<int>
    {
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public ActionType Action { get; set; } = ActionType.View;

        public string ModuleName { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;

        public DateTime TimeStamp { get; set; }
        public string OldValues { get; set; } = string.Empty;
        public string NewValues { get; set; } = string.Empty;
    }
}
