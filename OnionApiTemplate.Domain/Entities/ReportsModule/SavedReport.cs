using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities.HRModule;

namespace Khazen.Domain.Entities.ReportsModule
{
    public class SavedReport
        : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ReportType ReportType { get; set; } = ReportType.CashFlowStatement;
    }
}
