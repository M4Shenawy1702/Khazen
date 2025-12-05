namespace Khazen.Application.DOTs.CongifurationModule.CompanySetting
{
    public class CompanySettingDto
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string DomainName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public string DefaultLanguage { get; set; } = "en-US";
        public string CurrencyCode { get; set; } = "USD";
        public string CurrencySymbol { get; set; } = "$";
        public string ThemeColor { get; set; } = "#000000";
        public decimal DefaultTaxRate { get; set; } = 0m;
        public DateTime FiscalYearStart { get; set; } = DateTime.UtcNow;
    }
}
