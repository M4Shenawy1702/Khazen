namespace Khazen.Domain.Entities.ConfigurationModule
{
    public class CompanySetting : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string DomainName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;

        // Localization & Currency
        public string DefaultLanguage { get; set; } = "en-US";
        public string CurrencyCode { get; set; } = "USD";
        public string CurrencySymbol { get; set; } = "$";

        // Branding
        public string ThemeColor { get; set; } = "#000000";

        // Finance
        public decimal DefaultTaxRate { get; set; } = 0m;
        public DateTime FiscalYearStart { get; set; }
    }
}
