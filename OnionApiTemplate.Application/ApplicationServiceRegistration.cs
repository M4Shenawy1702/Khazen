using Khazen.Application.Common.Interfaces;
using Khazen.Application.Common.Interfaces.Authentication;
using Khazen.Application.Common.Interfaces.IHRModule;
using Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseInvoice;
using Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchasePaymentServices;
using Khazen.Application.Common.Interfaces.IPurchaseModule.IPurchaseReceipt;
using Khazen.Application.Common.Interfaces.ISalesModule;
using Khazen.Application.Common.Interfaces.ISalesModule.ISalesInvoicePaymentServices;
using Khazen.Application.Common.Interfaces.ISalesModule.ISalesInvoiceServices;
using Khazen.Application.Common.Interfaces.SalesModule.ISalesOrderModule;
using Khazen.Application.Common.Services;
using Khazen.Application.Common.Services.AuthenticationServices;
using Khazen.Application.Common.Services.HRModuleServices;
using Khazen.Application.Common.Services.PurchaseModuleServices.PurchaseInvoiceServices;
using Khazen.Application.Common.Services.PurchaseModuleServices.PurchasePaymentServices;
using Khazen.Application.Common.Services.PurchaseModuleServices.PurchaseReceiptServices;
using Khazen.Application.Common.Services.SalesInvoiceModuleService;
using Khazen.Application.Common.Services.SalesInvoicePaymentServices;
using Khazen.Application.Common.Services.SalesOrderServices;
using Khazen.Application.MappingProfile;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace Khazen.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
        {

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationServiceRegistration).Assembly));

            // Register all validators in the assembly
            services.AddValidatorsFromAssembly(typeof(ApplicationServiceRegistration).Assembly);
            services.AddAutoMapper(typeof(EmployeeProfile).Assembly);
            services.AddScoped<IPayslipGenerator, PayslipGenerator>();
            services.AddScoped<INumberSequenceService, NumberSequenceService>();
            services.AddScoped<IGetSystemValues, GetSystemValues>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<FormOptions>(cfg =>
            {
                cfg.ValueLengthLimit = int.MaxValue;
                cfg.MultipartBodyLengthLimit = int.MaxValue;
                cfg.MemoryBufferThreshold = int.MaxValue;
            });
            //services.AddTransient<ISmsService, TwilioSmsService>();
            services.AddTransient<ISmsService, MockSmsService>();
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<ISalesInvoiceService, SalesInvoiceService>();
            services.AddScoped<IPurchaseReceiptUpdateService, PurchaseReceiptUpdateService>();
            services.AddScoped<IJournalEntryService, JournalEntryService>();
            services.AddScoped<IStockReservationService, StockReservationService>();
            services.AddScoped<ISalesOrderService, SalesOrderService>();
            services.AddScoped<ISalesPaymentDomainService, SalesPaymentDomainService>();
            services.AddScoped<ISafeTransactionService, SafeTransactionService>();
            services.AddScoped<IUserRegistrationService, UserRegistrationService>();
            services.AddScoped<IDeletePurchaseReceiptService, DeletePurchaseReceiptService>();
            services.AddScoped<IPurchaseReceiptFactory, PurchaseReceiptFactory>();
            services.AddScoped<IPurchasePaymentDomainService, PurchasePaymentDomainService>();
            services.AddScoped<IInvoiceFactoryService, InvoiceFactoryService>();
            services.AddScoped<IWarehouseStockService, WarehouseStockService>();
            services.AddScoped<IPurchaseInvoiceStockCostService, PurchaseInvoiceStockCostService>();
            services.AddScoped<ISalaryCalculationService, SalaryCalculationService>();
            services.AddScoped<ICacheService, RedisCacheService>();
            services.AddScoped<IPurchaseOrderStatusService, PurchaseOrderStatusService>();
            services.AddScoped<ISalaryDomainService, SalaryDomainService>();


            return services;
        }
    }
}
