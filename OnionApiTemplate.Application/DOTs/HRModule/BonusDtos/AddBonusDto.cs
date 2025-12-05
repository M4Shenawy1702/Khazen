namespace Khazen.Application.DOTs.HRModule.BonusDtos
{
    public class AddBonusDto
    {
        public Guid EmployeeId { get; set; }
        public DateOnly Date { get; set; }
        public decimal BonusAmount { get; set; }
        public string? Reason { get; set; }
    }
}
