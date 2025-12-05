using Khazen.Application.BaseSpecifications.HRModule.SalarySepcifications;
using Khazen.Application.Common.Interfaces;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.SalaryUseCases.Queries.GetPayslip
{
    internal class GetPayslipQueryHandler : IRequestHandler<GetPayslipQuery, PayslipResult?>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPayslipGenerator _payslipGenerator;
        private readonly ILogger<GetPayslipQueryHandler> _logger;

        public GetPayslipQueryHandler(
            IUnitOfWork unitOfWork,
            IPayslipGenerator payslipGenerator,
            ILogger<GetPayslipQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _payslipGenerator = payslipGenerator;
            _logger = logger;
        }

        public async Task<PayslipResult?> Handle(GetPayslipQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting payslip generation for SalaryId: {SalaryId}", request.SalaryId);

            var salaryRepo = _unitOfWork.GetRepository<Salary, Guid>();

            var salary = await salaryRepo.GetAsync(
                new GetSalaryWithEmployeeByIdSpec(request.SalaryId),
                cancellationToken);

            if (salary == null)
            {
                _logger.LogWarning("Salary not found for SalaryId: {SalaryId}", request.SalaryId);
                throw new NotFoundException<Salary>(request.SalaryId);
            }

            _logger.LogDebug("Salary found. Generating PDF for Employee: {EmployeeId}", salary.EmployeeId);

            var pdfBytes = _payslipGenerator.GeneratePdf(salary);

            var fileName = $"Payslip_{salary.SalaryDate:yyyy_MM}_{salary.Employee?.FirstName} {salary.Employee?.LastName} {salary.EmployeeId.ToString()}.pdf";

            _logger.LogInformation("Payslip PDF generated successfully for SalaryId: {SalaryId}", request.SalaryId);

            return new PayslipResult
            {
                Content = pdfBytes,
                ContentType = "application/pdf",
                FileName = fileName
            };
        }
    }
}
