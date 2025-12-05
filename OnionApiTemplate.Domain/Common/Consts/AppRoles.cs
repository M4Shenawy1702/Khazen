namespace Khazen.Domain.Common.Consts
{
    public static class AppRoles
    {
        public static List<string> AllRoles = new List<string>
        {
            SuperAdmin,
            Admin,
            Manager,
            Employee,
            Customer,
            Auditor,
            Accountant,
            FinancialManager,
            Cashier,
            PayrollOfficer,
            HRManager,
            Recruiter,
            EmployeeRelationsOfficer,
            Trainer,
            ProcurementManager,
            PurchasingOfficer,
            InventoryManager,
            StoreKeeper,
            SalesManager,
            SalesRepresentative,
            CustomerServiceAgent,
        };
        // Administrative
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Employee = "Employee";
        public const string Auditor = "Auditor";

        // Accounting & Finance
        public const string Accountant = "Accountant";
        public const string FinancialManager = "FinancialManager";
        public const string Cashier = "Cashier";
        public const string PayrollOfficer = "PayrollOfficer";

        // Human Resources
        public const string HRManager = "HRManager";
        public const string Recruiter = "Recruiter";
        public const string EmployeeRelationsOfficer = "EmployeeRelationsOfficer";
        public const string Trainer = "Trainer";

        // Procurement & Inventory
        public const string ProcurementManager = "ProcurementManager";
        public const string PurchasingOfficer = "PurchasingOfficer";
        public const string InventoryManager = "InventoryManager";
        public const string StoreKeeper = "StoreKeeper";

        // Sales & CRM
        public const string SalesManager = "SalesManager";
        public const string SalesRepresentative = "SalesRepresentative";
        public const string CustomerServiceAgent = "CustomerServiceAgent";
        public const string CRMAdmin = "CRMAdmin";
        public const string Customer = "Customer";

        // Projects & Operations
        public const string ProjectManager = "ProjectManager";
        public const string ProjectCoordinator = "ProjectCoordinator";
        public const string OperationsManager = "OperationsManager";

        // IT & System
        public const string ITAdmin = "ITAdmin";
        public const string Developer = "Developer";
        public const string SupportTechnician = "SupportTechnician";
    }
}
