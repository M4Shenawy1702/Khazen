using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Entities.HRModule;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Entities.ReportsModule;
using Khazen.Domain.Entities.UsersModule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Claims;
using System.Text.Json;

namespace Khazen.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            ApplyDecimalPrecision(builder);


            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
        private void ApplyDecimalPrecision(ModelBuilder modelBuilder)
        {
            var decimalProperties = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?));

            foreach (var property in decimalProperties)
            {
                property.SetPrecision(18);
                property.SetScale(2);
            }
        }

        // ================================ Hr Module ================================
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<PerformanceReview> PerformanceReviews { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<Bonus> Bonuses { get; set; }
        public DbSet<Deduction> Deductions { get; set; }
        public DbSet<Advance> Advances { get; set; }

        // ================================ Users Module ================================
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        // ================================ Audit ================================
        public DbSet<AuditLog> AuditLogs { get; set; }

        // ================================ Inventory Module ================================
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Warehouse> WareHouses { get; set; }
        public DbSet<SupplierProduct> SupplierProducts { get; set; }

        // ================================ Sales Module ================================
        public DbSet<Customer> Customers { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderItem> SalesOrderItems { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }

        // ================================ Accounting ================================
        public DbSet<Account> Accounts { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalEntryLine> JournalEntryLines { get; set; }

        // ================================ Purchase Module ================================
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<PurchaseReceipt> PurchaseReceipts { get; set; }

        //================================ Billing Module ================================
        public DbSet<Safe> Saves { get; set; }
        public DbSet<SafeTransaction> SafeTransactions { get; set; }


        // ================================ Reports Module ================================
        public DbSet<SavedReport> SavedReports { get; set; }

        // ================================ SaveChanges Override ================================
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditLogs = new List<AuditLog>();
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditLog = new AuditLog
                {
                    UserId = userId,
                    ModuleName = entry.Entity.GetType().Namespace ?? "Unknown",
                    EntityName = entry.Entity.GetType().Name,
                    Action = GetActionType(entry.State),
                    TimeStamp = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                if (entry.State == EntityState.Added)
                {
                    auditLog.NewValues = SerializeProps(entry.CurrentValues);
                }
                else if (entry.State == EntityState.Deleted)
                {
                    auditLog.OldValues = SerializeProps(entry.OriginalValues);
                }
                else if (entry.State == EntityState.Modified)
                {
                    auditLog.OldValues = SerializeProps(entry.OriginalValues);
                    auditLog.NewValues = SerializeProps(entry.CurrentValues);
                }

                auditLogs.Add(auditLog);
            }

            if (auditLogs.Any())
                AuditLogs.AddRange(auditLogs);

            return await base.SaveChangesAsync(cancellationToken);
        }

        private static ActionType GetActionType(EntityState state) =>
            state switch
            {
                EntityState.Added => ActionType.Create,
                EntityState.Modified => ActionType.Update,
                EntityState.Deleted => ActionType.Delete,
                _ => ActionType.View
            };

        private string SerializeProps(PropertyValues values)
        {
            var dict = new Dictionary<string, object?>();
            foreach (var prop in values.Properties)
            {
                dict[prop.Name] = values[prop];
            }
            return JsonSerializer.Serialize(dict);
        }
    }
}
