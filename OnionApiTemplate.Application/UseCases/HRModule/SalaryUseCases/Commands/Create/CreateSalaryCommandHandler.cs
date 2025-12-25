using Khazen.Application.BaseSpecifications.HRModule.EmployeeSpecifications;
using Khazen.Application.BaseSpecifications.HRModule.SalarySepcifications;
using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.Interfaces.IHRModule.ISalaryServices;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.SalaryUseCases.Commands.Create
{
    internal class CreateSalaryCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateSalaryCommand> validator,
        ILogger<CreateSalaryCommandHandler> logger,
        UserManager<ApplicationUser> userManager,
        IJournalEntryService journalEntryService,
        ISalaryDomainService salaryDomainService)
        : IRequestHandler<CreateSalaryCommand, SalaryDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CreateSalaryCommand> _validator = validator;
        private readonly ILogger<CreateSalaryCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJournalEntryService _journalEntryService = journalEntryService;
        private readonly ISalaryDomainService _salaryDomainService = salaryDomainService;

        public async Task<SalaryDto> Handle(CreateSalaryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Initiating salary creation: Employee {Id}, Period {Date}",
                request.Dto.EmployeeId, request.Dto.SalaryDate);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateSalaryCommand.");
                throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            }

            var user = await _userManager.FindByNameAsync(request.CurrentUserId);
            if (user is null)
            {
                _logger.LogError("User {UserId} not found", request.CurrentUserId);
                throw new NotFoundException<ApplicationUser>(request.CurrentUserId);
            }

            var salaryRepo = _unitOfWork.GetRepository<Salary, Guid>();
            var employeeRepo = _unitOfWork.GetRepository<Employee, Guid>();

            var existingTask = salaryRepo.GetAsync(new GetSalaryByEmployeeIdAndSalaryDateSpec(request.Dto.EmployeeId, request.Dto.SalaryDate), cancellationToken);
            var employeeTask = employeeRepo.GetAsync(new GetEmployeeByIdSpecification(request.Dto.EmployeeId), cancellationToken);

            await Task.WhenAll(existingTask, employeeTask);

            var employee = employeeTask.Result;
            if (employee == null)
            {
                _logger.LogWarning("Employee not found: {Id}", request.Dto.EmployeeId);
                throw new NotFoundException<Employee>(request.Dto.EmployeeId);
            }

            if (existingTask.Result != null)
            {
                _logger.LogWarning("Duplicate salary attempt for Employee {Id}", request.Dto.EmployeeId);
                throw new AlreadyExistsException<Salary>($"Salary already processed for {request.Dto.SalaryDate:MMMM yyyy}");
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var salary = _salaryDomainService.CreateSalary(employee, request.Dto.SalaryDate, user.Id, request.Dto.Notes);
                await salaryRepo.AddAsync(salary, cancellationToken);

                var journalEntry = await _journalEntryService.CreateSalaryJournalEntryAsync(employee, salary, user.Id, cancellationToken);

                await _unitOfWork.GetRepository<JournalEntry, Guid>().AddAsync(journalEntry, cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Salary and Journal Entry committed for Employee {Id}. Net Pay: {Net}",
                    employee.Id, salary.NetSalary);

                return _mapper.Map<SalaryDto>(salary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transaction failed for Employee {Id}. Rolling back.", request.Dto.EmployeeId);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
