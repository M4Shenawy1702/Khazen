using Khazen.Application.BaseSpecifications.HRModule.EmployeeSpecifications;
using Khazen.Application.BaseSpecifications.HRModule.SalarySepcifications;
using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.Interfaces.IHRModule;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Entities.ConfigurationModule;
using Khazen.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Khazen.Application.UseCases.HRModule.SalaryUseCases.Commands.Create
{
    internal class CreateSalaryCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateSalaryCommand> validator,
        INumberSequenceService numberSequenceService,
        IGetSystemValues getSystemValues,
        ILogger<CreateSalaryCommandHandler> logger,
        UserManager<ApplicationUser> userManager,
        ISalaryCalculationService salaryCalculationService
        , IJournalEntryService journalEntryService,
        ISalaryDomainService salaryDomainService)
        : IRequestHandler<CreateSalaryCommand, SalaryDto>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<CreateSalaryCommand> _validator = validator;
        private readonly INumberSequenceService _numberSequenceService = numberSequenceService;
        private readonly IGetSystemValues _getSystemValues = getSystemValues;
        private readonly ILogger<CreateSalaryCommandHandler> _logger = logger;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ISalaryCalculationService _salaryCalculationService = salaryCalculationService;
        private readonly IJournalEntryService _journalEntryService = journalEntryService;
        private readonly ISalaryDomainService _salaryDomainService = salaryDomainService;

        public async Task<SalaryDto> Handle(CreateSalaryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting salary creation for EmployeeId: {EmployeeId}, Date: {SalaryDate}",
                request.Dto.EmployeeId, request.Dto.SalaryDate);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validation failed for CreateSalaryCommand: {Errors}",
                        string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    throw new BadRequestException(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
                }

                var salaryRepo = _unitOfWork.GetRepository<Salary, Guid>();
                var employeeRepo = _unitOfWork.GetRepository<Employee, Guid>();

                _logger.LogDebug("Checking for existing salary records for EmployeeId: {EmployeeId}", request.Dto.EmployeeId);

                var existing = await salaryRepo.GetAsync(
                    new GetSalaryByEmployeeIdAndSalaryDateSpec(request.Dto.EmployeeId, request.Dto.SalaryDate),
                    cancellationToken);

                if (existing != null)
                {
                    _logger.LogWarning("Salary already exists for EmployeeId: {EmployeeId} in month {SalaryDate}",
                        request.Dto.EmployeeId, request.Dto.SalaryDate);
                    throw new AlreadyExistsException<Salary>(
                        $"Salary for employee with id: {request.Dto.EmployeeId} already exists for this month: {request.Dto.SalaryDate}");
                }

                var user = await _userManager.FindByNameAsync(request.CreatedBy);
                if (user is null)
                {
                    _logger.LogInformation("User not found. UserId: {ModifiedBy}", request.CreatedBy);
                    throw new NotFoundException<ApplicationUser>(request.CreatedBy);
                }

                var employee = await employeeRepo.GetAsync(new GetEmployeeByIdSpecification(request.Dto.EmployeeId), cancellationToken);
                if (employee == null)
                {
                    _logger.LogWarning("Employee not found for EmployeeId: {EmployeeId}", request.Dto.EmployeeId);
                    throw new NotFoundException<Employee>(request.Dto.EmployeeId);
                }

                var salary = _salaryDomainService.CreateSalary(employee, request.Dto.SalaryDate, request.CreatedBy, request.Dto.Notes);
                await salaryRepo.AddAsync(salary, cancellationToken);

                _logger.LogInformation("Salary record created successfully for EmployeeId: {EmployeeId}", request.Dto.EmployeeId);

                var journalEntryRepo = _unitOfWork.GetRepository<JournalEntry, Guid>();
                var systemSettingsRepo = _unitOfWork.GetRepository<SystemSetting, int>();
                var systemSettings = await systemSettingsRepo.GetAllAsync(new GetAllSystemSettingsSpec(), cancellationToken);

                var salaryExpenseAccountValue = _getSystemValues.GetSettingValue(systemSettings, "SalaryExpenseAccountId");
                if (!Guid.TryParse(salaryExpenseAccountValue, out var salaryExpenseAccountId))
                {
                    _logger.LogError("Salary Expense Account ID is missing or invalid in system settings.");
                    throw new DomainException("Salary Expense Account ID is missing or invalid.");
                }
                var cashAccountValue = _getSystemValues.GetSettingValue(systemSettings, "CashAccountId");
                if (!Guid.TryParse(cashAccountValue, out var cashAccountId))
                {
                    _logger.LogError("Cash Account ID is missing or invalid in system settings.");
                    throw new DomainException("Cash Account ID is missing or invalid.");
                }

                var journalEntry = await _journalEntryService.CreateSalaryJournalEntryAsync(employee, salary, request.CreatedBy, cancellationToken);
                await journalEntryRepo.AddAsync(journalEntry, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Journal entry and salary committed successfully for EmployeeId: {EmployeeId}",
                    request.Dto.EmployeeId);

                return _mapper.Map<SalaryDto>(salary);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(ex, "Error occurred while creating salary for EmployeeId: {EmployeeId}", request.Dto.EmployeeId);
                throw;
            }
        }
    }
}
