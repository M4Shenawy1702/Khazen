using Khazen.Domain.Common.Consts;
using Khazen.Domain.Common.Enums;
using Khazen.Domain.Entities;
using Khazen.Domain.Entities.AccountingModule;
using Khazen.Domain.Entities.ConfigurationModule;
using Khazen.Domain.Entities.HRModule;
using Khazen.Domain.Entities.InventoryModule;
using Khazen.Domain.Entities.PurchaseModule;
using Khazen.Domain.Entities.UsersModule;
using Khazen.Domain.IRepositoty;
using Khazen.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using System.Text.Json;

public class DbInitializer(ApplicationDbContext _context, RoleManager<ApplicationRole> _roleManager, UserManager<ApplicationUser> _userManager)
    : IDbInitializer
{
    public async Task InitializeDatabaseAsync()
    {
        try
        {
            await _context.Database.EnsureCreatedAsync();

            if ((await _context.Database.GetPendingMigrationsAsync()).Any())
            {
                await _context.Database.MigrateAsync();
            }

            await SeedEntitiesFromFileAsync<Department>("departments.json");
            await SeedEntitiesFromFileAsync<Account>("accounts.json");
            //await SeedEntitiesFromFileAsync<Customer>("customers.json");
            await SeedEntitiesFromFileAsync<Brand>("brands.json");
            await SeedEntitiesFromFileAsync<Category>("categories.json");
            await SeedEntitiesFromFileAsync<Warehouse>("warehouses.json");
            await SeedEntitiesFromFileAsync<Product>("products.json");
            await SeedEntitiesFromFileAsync<Supplier>("suppliers.json");
            await SeedEntitiesFromFileAsync<WarehouseProduct>("warehouseProducts.json");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database initialization failed: {ex.Message}");
        }
    }

    private async Task SeedEntitiesFromFileAsync<T>(string fileName) where T : class
    {
        if (!await _context.Set<T>().AnyAsync())
        {
            var data = await ReadFileAsync(fileName);
            var entities = JsonSerializer.Deserialize<List<T>>(data, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (entities != null && entities.Any())
            {
                await _context.Set<T>().AddRangeAsync(entities);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ Seeded {entities.Count} {typeof(T).Name}(s) from {fileName}");
            }
        }
    }

    public async Task InitializeIdentityAsync()
    {
        if (!await _roleManager.Roles.AnyAsync())
        {
            var roleType = typeof(AppRoles);
            var allRoles = roleType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                       .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                       .Select(fi => fi.GetValue(null)?.ToString())
                       .Where(role => !string.IsNullOrWhiteSpace(role))
                       .ToList();

            foreach (var roleName in allRoles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName!))
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = roleName!, CreatedBy = "System" });
                }
            }

        }
        if (!await _userManager.Users.AnyAsync() && !await _context.Set<Customer>().AnyAsync())
        {
            // Step 1: Seed core users
            var coreUsers = new List<(ApplicationUser user, string role)>
    {
        (new ApplicationUser { Email = "SuperAdmin@gmail.com", UserName = "SuperAdmin", PhoneNumber = "1234567890", FullName = "Super Admin", IsActive = true }, "SuperAdmin"),
        (new ApplicationUser { Email = "Admin@gmail.com", UserName = "Admin", PhoneNumber = "1234567890", FullName = "Admin User", IsActive = true }, "Admin"),
        (new ApplicationUser { Email = "HR@gmail.com", UserName = "HR", PhoneNumber = "1234567890", FullName = "HR Manager", IsActive = true }, "HRManager")
    };

            foreach (var (user, role) in coreUsers)
            {
                if (await _userManager.FindByEmailAsync(user.Email) == null)
                {
                    await _userManager.CreateAsync(user, "P@ssW0rd123");
                    await _userManager.AddToRoleAsync(user, role);
                }
            }

            // Step 2: Seed customer users
            var customerUsers = new List<ApplicationUser>
    {
        new ApplicationUser
        {
            Id = "d2a3e1a2-8d4e-4f59-9e22-1f3a22a5b1a1",
            UserName = "ahmed.ali",
            Email = "ahmed.ali@example.com",
            PhoneNumber = "01012345678",
            FullName = "Ahmed Ali",
            Address = "Cairo, Egypt",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1990,5,15),
            IsActive = true
        },
        new ApplicationUser
        {
            Id = "e6a4b8c3-1c72-4c89-8a11-2a5e33c6b2b2",
            UserName = "sara.mohamed",
            Email = "sara.mohamed@example.com",
            PhoneNumber = "01123456789",
            FullName = "Sara Mohamed",
            Address = "Giza, Egypt",
            Gender = Gender.Female,
            DateOfBirth = new DateTime(1992,3,20),
            IsActive = true
        },
        new ApplicationUser
        {
            Id = "a1b2c3d4-5678-4e9f-9d01-3c7a44d7c3c3",
            UserName = "omar.hassan",
            Email = "omar.hassan@example.com",
            PhoneNumber = "01234567890",
            FullName = "Omar Hassan",
            Address = "Alexandria, Egypt",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(1988,11,10),
            IsActive = true
        },
        new ApplicationUser
        {
            Id = "f7d8e9a0-2233-4455-6677-8899aabbccdd",
            UserName = "tech.solutions",
            Email = "info@techsolutions.com",
            PhoneNumber = "0223456789",
            FullName = "Tech Solutions Ltd",
            Address = "Smart Village, Cairo, Egypt",
            Gender = Gender.Male,
            DateOfBirth = new DateTime(2000,1,1),
            IsActive = true
        }
    };

            foreach (var user in customerUsers)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);
                if (existingUser == null)
                {
                    await _userManager.CreateAsync(user, "P@ssW0rd123");
                    await _userManager.AddToRoleAsync(user, AppRoles.Customer);

                    // Use the constructor to create Customer
                    var customer = new Customer(
                        name: user.FullName,
                        address: user.Address,
                        userId: user.Id,
                        customerType: CustomerType.Individual, // adjust as needed
                        createdBy: "System"
                    );

                    await _context.Set<Customer>().AddAsync(customer);
                    await _context.SaveChangesAsync();
                }
            }

            Console.WriteLine("Users and Customers seeding completed successfully.");
        }

        if (!await _context.Set<CompanySetting>().AnyAsync())
        {
            var companySetting = new CompanySetting
            {
                Name = "Default Company",
                Address = "123 Main Street, Cairo, Egypt",
                DomainName = "defaultcompany.com",
                Phone = "+20 100 000 0000",
                Email = "info@defaultcompany.com",
                LogoUrl = "/images/logo.png",
                DefaultLanguage = "en-US",
                CurrencyCode = "USD",
                CurrencySymbol = "$",
                ThemeColor = "#0d6efd",
                DefaultTaxRate = 0.15m,
                FiscalYearStart = new DateTime(DateTime.UtcNow.Year, 1, 1)
            };

            _context.Set<CompanySetting>().Add(companySetting);
            await _context.SaveChangesAsync();
        }
        if (!await _context.Set<SystemSetting>().AnyAsync())
        {
            var systemSettings = new List<SystemSetting>
{
            new SystemSetting
            {
                Key = "CashAccountId",
                Value = "11111111-1111-1111-1111-111111111111",
                Description = "Default Cash Account",
                Group = SystemSettingGroubType.Accounting,
                ValueType = SystemSettingValueType.Guid
            },
            new SystemSetting
            {
                Key = "BankAccountId",
                Value = "11111111-1111-1111-1111-111111111112",
                Description = "Default Bank Account",
                Group = SystemSettingGroubType.Accounting,
                ValueType = SystemSettingValueType.Guid
            },
                    new SystemSetting
                {
                    Key = "PurchasesAccountId",
                    Value = "66666666-6666-6666-6666-666666666666",
                    Description = "Default Purchases Expense Account",
                    Group = SystemSettingGroubType.Accounting,
                    ValueType = SystemSettingValueType.Guid
                },
            new SystemSetting
            {
                Key = "AccountsReceivableAccountId",
                Value = "11111111-1111-1111-1111-111111111113",
                Description = "Default Accounts Receivable",
                Group = SystemSettingGroubType.Accounting,
                ValueType = SystemSettingValueType.Guid
            },
            new SystemSetting
            {
                Key = "AccountsPayableAccountId",
                Value = "22222222-2222-2222-2222-222222222221",
                Description = "Default Accounts Payable",
                Group = SystemSettingGroubType.Accounting,
                ValueType = SystemSettingValueType.Guid
            },
            new SystemSetting
            {
                Key = "SalesRevenueAccountId",
                Value = "44444444-4444-4444-4444-444444444441",
                Description = "Default Sales Revenue",
                Group = SystemSettingGroubType.Accounting,
                ValueType = SystemSettingValueType.Guid
            },
            new SystemSetting
            {
                Key = "DiscountAllowedAccountId",
                Value = "55555555-5555-5555-5555-555555555555",
                Description = "Default Discount Allowed Account",
                Group = SystemSettingGroubType.Accounting,
                ValueType = SystemSettingValueType.Guid
            },
            new SystemSetting
            {
                Key = "CashSafeId",
                Value = "11111111-1111-1111-1111-111111111111",
                Description = "Main Cash Safe",
                Group = SystemSettingGroubType.Safe,
                ValueType = SystemSettingValueType.Guid
            },
            new SystemSetting
            {
                Key = "BankSafeId",
                Value = "22222222-2222-2222-2222-222222222222",
                Description = "Main Bank Account",
                Group = SystemSettingGroubType.Safe,
                ValueType = SystemSettingValueType.Guid
            },
            new SystemSetting
            {
                Key = "DigitalWalletId",
                Value = "33333333-3333-3333-3333-333333333333",
                Description = "Digital Wallet",
                Group = SystemSettingGroubType.Safe,
                ValueType = SystemSettingValueType.Guid
            },
        };


            await _context.Set<SystemSetting>().AddRangeAsync(systemSettings);
            await _context.SaveChangesAsync();
        }
        if (!await _context.Set<Safe>().AnyAsync())
        {
            var safes = new List<Safe>
            {
                new Safe
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Main Cash Safe",
                    Type = SafeType.Cash,
                    Balance = 10000m,
                    Description = "Primary cash safe",
                    IsActive = true
                },
                new Safe
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Main Bank Account",
                    Type = SafeType.Bank,
                    Balance = 50000m,
                    Description = "Primary bank account",
                    IsActive = true
                },
                new Safe
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Digital Wallet",
                    Type = SafeType.Wallet,
                    Balance = 1500m,
                    Description = "Main digital wallet",
                    IsActive = true
                }
            };

            await _context.Set<Safe>().AddRangeAsync(safes);
            await _context.SaveChangesAsync();
        }


    }

    private static async Task<string> ReadFileAsync(string relativePath)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Seeding");
        var fullPath = Path.Combine(basePath, relativePath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Seed file not found: {fullPath}");

        return await File.ReadAllTextAsync(fullPath);
    }
}
