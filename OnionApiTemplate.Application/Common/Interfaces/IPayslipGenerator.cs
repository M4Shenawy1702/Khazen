namespace Khazen.Application.Common.Interfaces
{
    public interface IPayslipGenerator
    {
        byte[] GeneratePdf(Salary salary);
    }
}
