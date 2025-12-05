namespace Khazen.Application.UseCases.HRModule.SalaryUseCases.Queries.GetPayslip
{
    public class PayslipResult
    {
        public byte[] Content { get; init; } = [];
        public string ContentType { get; init; } = "application/pdf";
        public string FileName { get; init; } = string.Empty;
    }
}
