namespace Khazen.Application.DOTs.CongifurationModule.CompanySetting
{
    public class UpdateCompanySettingsDto
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string DomainName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string LogoUrl { get; set; } = null!;
        public string DefaultLanguage { get; set; } = null!;
        public string CurrencyCode { get; set; } = null!;
        public string CurrencySymbol { get; set; } = null!;
        public string ThemeColor { get; set; } = null!;
        public decimal DefaultTaxRate { get; set; }
        public DateTime FiscalYearStart { get; set; }
    }
}
