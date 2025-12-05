namespace Khazen.Application.UseCases.HRModule.SalaryUseCases.Queries.GetPayslip
{
    public record GetPayslipQuery(Guid SalaryId) : IRequest<PayslipResult?>;
}
