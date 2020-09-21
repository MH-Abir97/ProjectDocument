using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pronali.Data.Models;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Data.Models.Entity.Core;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Data.Models.Entity.POS;


namespace Pronali.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }


        //Core
        public DbSet<Company> Company { get; set; }
        public DbSet<Branch> Branch { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Designation> Designation { get; set; }
        public DbSet<SisterConcern> SisterConcern { get; set; }
        public DbSet<Division> Division { get; set; }
        public DbSet<Section> Section { get; set; }
        public DbSet<Floor> Floor { get; set; }
        public DbSet<Line> Line { get; set; }
        public DbSet<Machine> Machine { get; set; }
        public DbSet<Email> Email { get; set; }
        public DbSet<SMS> SMS { get; set; }
        public DbSet<SmsHistory> SmsHistory { get; set; }
        public DbSet<UserActivity> UserActivity { get; set; }
        public DbSet<LoginHistory> LoginHistory { get; set; }




        //HR
        public DbSet<Employee> Employee { get; set; }
        public DbSet<EmployeFile> EmployeFile { get; set; }
        public DbSet<Leave> Leave { get; set; }
        public DbSet<EmployeeLeave> EmployeeLeave { get; set; }
        public DbSet<EarnLeave> EarnLeave { get; set; }
        public DbSet<Shift> Shift { get; set; }
        public DbSet<ShiftDetails> ShiftDetails { get; set; }
        public DbSet<Holiday> Holiday { get; set; }
        public DbSet<EmployeeHoliday> EmployeeHoliday { get; set; }
        public DbSet<EmployeeWeekend> EmployeeWeekend { get; set; }
        public DbSet<BusinessApplication> BusinessApplication { get; set; }
        public DbSet<LeaveApplication> LeaveApplication { get; set; }
        public DbSet<ShortLeaveApplication> ShortLeaveApplication { get; set; }
        public DbSet<ShortBusinessApplication> ShortBusinessApplication { get; set; }
        public DbSet<LatePermission> LatePermission { get; set; }
        public DbSet<EarlyOutPermission> EarlyOutPermission { get; set; }
        public DbSet<AttendanceMachineData> AttendanceMachineData { get; set; }
        public DbSet<AttendanceMachineLog> AttendanceMachineLog { get; set; }
        public DbSet<SalaryBreakup> SalaryBreakup { get; set; }
        public DbSet<SalaryStructure> SalaryStructure { get; set; }
        public DbSet<SalaryStructureDetails> SalaryStructureDetails { get; set; }
        public DbSet<EmployeeSalaryBase> EmployeeSalaryBase { get; set; }
        public DbSet<EmployeeGroup> EmployeeGroup { get; set; }
        public DbSet<JobLocation> JobLocation { get; set; }
        public DbSet<BonusType> BonusType { get; set; }
        public DbSet<BonusPolicy> BonusPolicy { get; set; }
        public DbSet<EmployeeBonus> EmployeeBonus { get; set; }
        public DbSet<YearlyBonus> YearlyBonus { get; set; }
        public DbSet<AttendanceProcessedData> AttendanceProcessedData { get; set; }
        public DbSet<DailyAttendance> DailyAttendance { get; set; }
        public DbSet<DataProcessQueue> DataProcessQueue { get; set; }
        public DbSet<AttendanceMachineDataFiltered> AttendanceMachineDataFiltered { get; set; }
        public DbSet<SystemPreference> SystemPreference { get; set; }
        public DbSet<ManualAbsent> ManualAbsent { get; set; }
        public DbSet<ManualAttendance> ManualAttendance { get; set; }
        public DbSet<Roaster> Roaster { get; set; }
        public DbSet<RoasterGroup> RoasterGroup { get; set; }
        public DbSet<RoasterGroupDetails> RoasterGroupDetails { get; set; }
        public DbSet<RoasterGroupEmployee> RoasterGroupEmployee { get; set; }

        public DbSet<Loan> Loan { get; set; }
        public DbSet<LoanApplication> LoanApplication { get; set; }
        public DbSet<LoanInstallment> LoanInstallment { get; set; }
        public DbSet<ProposedLoanInstallment> ProposedLoanInstallment { get; set; }
        public DbSet<ApprovedLoanInstallment> ApprovedLoanInstallment { get; set; }
        public DbSet<LoanDisbursement> LoanDisbursement { get; set; }

        public DbSet<EmployeeAsset> EmployeeAsset { get; set; }
        public DbSet<Insurance> Insurance { get; set; }
        public DbSet<EmployeeIncomeTax> EmployeeIncomeTax { get; set; }
        public DbSet<Promotion> Promotion { get; set; }
        public DbSet<Resignation> Resignation { get; set; }
        public DbSet<CompanyHead> CompanyHead { get; set; }
        public DbSet<SisterConcernHead> SisterConcernHead { get; set; }
        public DbSet<DivisionalHead> DivisionalHead { get; set; }
        public DbSet<BranchHead> BranchHead { get; set; }
        public DbSet<DepartmentalHead> DepartmentalHead { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<ApplicationApproval> ApplicationApproval { get; set; }
        public DbSet<EmployeeTask> EmployeeTask { get; set; }
        public DbSet<EmployeeRequiredField> EmployeeRequiredField { get; set; }
        public DbSet<SalaryHeldup> SalaryHeldup { get; set; }
        public DbSet<Suspended> Suspended { get; set; }
        public DbSet<Terminated> Termination { get; set; }



        //ACCOUNTS
        public DbSet<AccountGroup> AccountGroups { get; set; }
        public DbSet<Agent> Agent { get; set; }
        public DbSet<AccountLedger> AccountLedgers { get; set; }
        public DbSet<AccountLedgerGroup> AccountLedgerGroups { get; set; }
        public DbSet<AccountPayable> AccountPayables { get; set; }
        public DbSet<AccountReceivable> AccountReceivables { get; set; }
        public DbSet<AccountSubLedger> AccountSubLedgers { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<AdministrativeExpense> AdministrativeExpenses { get; set; }
        public DbSet<AdvanceForWork> AdvanceForWorks { get; set; }
        public DbSet<AirConditioner> AirConditioners { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<BrandValue> BrandValues { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<CashInHand> CashInHands { get; set; }
        public DbSet<CashInHandDetail> CashInHandDetails { get; set; }
        public DbSet<Cheque> Cheques { get; set; }
        public DbSet<CommissionIncome> CommissionIncomes { get; set; }
        public DbSet<CommissionIncomeDetail> CommissionIncomeDetails { get; set; }
        public DbSet<CompanyLicense> CompanyLicenses { get; set; }
        public DbSet<CompanyLicenseDetail> CompanyLicenseDetails { get; set; }
        public DbSet<CopyRight> CopyRights { get; set; }
        public DbSet<CorporateReputation> CorporateReputations { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Depriciation> Depriciations { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Employee> Employees { get; set; }        
        public DbSet<Factory> Factory { get; set; }
        public DbSet<FinishedGood> FinishedGoods { get; set; }
        public DbSet<FurnitureAndFixture> FurnitureAndFixtures { get; set; }
        public DbSet<GoodWill> GoodWills { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<HSCode> HSCodes { get; set; }
        public DbSet<IncentiveIncome> IncentiveIncomes { get; set; }
        public DbSet<IncentiveIncomeDetail> IncentiveIncomeDetails { get; set; }
        public DbSet<IncomeTax> IncomeTaxes { get; set; }
        public DbSet<InterestCalculation> InterestCalculations { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Investor> Investors { get; set; }
        public DbSet<LaptopAndTab> LaptopAndTabs { get; set; }
        public DbSet<LoanPayable> LoanPayables { get; set; }
        public DbSet<LoanPayableDetail> LoanPayableDetails { get; set; }
        public DbSet<LoanReceivable> LoanReceivables { get; set; }
        public DbSet<LoanReceivableDetail> LoanReceivableDetails { get; set; }
        public DbSet<LoanType> LoanTypes { get; set; }
        public DbSet<Machinery> Machineries { get; set; }
        public DbSet<MarketableSecurity> MarketableSecurities { get; set; }
        public DbSet<MarketingExpense> MarketingExpenses { get; set; }        
        public DbSet<OfficeDecoration> OfficeDecorations { get; set; }
        public DbSet<OperatingExpense> OperatingExpenses { get; set; }
        public DbSet<OwnersEquity> OwnersEquities { get; set; }
        public DbSet<OwnersEquityDetail> OwnersEquityDetails { get; set; }        
        public DbSet<PrepaidExpense> PrepaidExpenses { get; set; }
        public DbSet<PrinterAndScanner> PrinterAndScanners { get; set; }        
        public DbSet<Project> Projects { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseExpense> PurchaseExpenses { get; set; }
        public DbSet<PurchaseExpenseDetail> PurchaseExpenseDetails { get; set; }
        public DbSet<PurchaseReturn> PurchaseReturns { get; set; }
        public DbSet<PurchaseReturnDetail> PurchaseReturnDetails { get; set; }
        public DbSet<PurchaseWarrenty> PurchaseWarrenties { get; set; }
        public DbSet<SalaryAdvance> SalaryAdvances { get; set; }
        public DbSet<SalesReturnDetail> SaleReturnDetails { get; set; }
        public DbSet<SalesReturn> SalesReturns { get; set; }
        public DbSet<SalesWarrenty> SaleWarrenties { get; set; }
        public DbSet<SecurityAdvance> SecurityAdvances { get; set; }
        public DbSet<ServiceIncome> ServiceIncomes { get; set; }
        public DbSet<ServiceIncomeDetail> ServiceIncomeDetails { get; set; }
        public DbSet<Software> Softwares { get; set; }
        public DbSet<SoftwareLicense> SoftwareLicenses { get; set; }
        public DbSet<SourceCode> SourceCodes { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<TelephoneAndMobile> TelephoneAndMobiles { get; set; }
        public DbSet<TermsReceivable> TermsReceivables { get; set; }
        public DbSet<TradeMark> TradeMarks { get; set; }
        public DbSet<TradeSecret> TradeSecrets { get; set; }
        public DbSet<Vat> Vats { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<VoucherHead> VoucherHeads { get; set; }
        public DbSet<TempVoucher> TempVouchers { get; set; }
        public DbSet<VoucherDetail> VoucherDetails { get; set; }
        public DbSet<TempVoucherHead> TempVoucherHeads { get; set; }
        public DbSet<TempVoucherDetail> TempVoucherDetails { get; set; }
        public DbSet<WorkInProgress> WorkInProgresses { get; set; }
        public DbSet<VoucherNumber> VoucherNumbers { get; set; }
        public DbSet<VoucherHeadHistory> VoucherHeadHistories { get; set; }
        public DbSet<VoucherDetailHistory> VoucherDetailHistories { get; set; }



        //POS
        public DbSet<AccumulatedDepreciation> AccumulatedDepreciations { get; set; }
        public DbSet<AdministrativeExpensesDetail> AdministrativeExpensesDetails { get; set; }
        public DbSet<BankAccountDetail> BankAccountDetails { get; set; }
        public DbSet<Challan> Challans { get; set; }
        public DbSet<ChallanDetail> ChallanDetails { get; set; }
        public DbSet<ComputerAndAccessories> ComputerAndAccessories { get; set; }
        public DbSet<Conversion> Conversions { get; set; }
        public DbSet<Counter> Counters { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CustomerPoint> CustomerPoints { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<DiscountDetail> DiscountDetails { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<MarketingExpensessDetail> MarketingExpensessDetails { get; set; }
        public DbSet<OperatingExpensesDetail> OperatingExpensesDetails { get; set; }
        public DbSet<PackageUnit> PackageUnits { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductGroup> ProductGroups { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<ProductUnit> ProductUnits { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<Quotation> Quotations { get; set; }
        public DbSet<Refrigerator> Refrigerators { get; set; }
        public DbSet<Sales> Sales { get; set; }
        public DbSet<SalesItem> SalesItems { get; set; }
        public DbSet<SalesOrder> SaleOrders { get; set; }
        public DbSet<SalesTarget> SaleTargets { get; set; }
        public DbSet<Scheme> Schemes { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<TempPurchase> TempPurchases { get; set; }
        public DbSet<TempSales> TempSales { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Vechicle> Vechicles { get; set; }





























        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().HasMany(u => u.Claims).WithOne().HasForeignKey(c => c.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationUser>().HasMany(u => u.Roles).WithOne().HasForeignKey(r => r.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationRole>().HasMany(r => r.Claims).WithOne().HasForeignKey(c => c.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationRole>().HasMany(r => r.Users).WithOne().HasForeignKey(r => r.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<EmployeeRequiredField>()
                .HasIndex(e => e.Property)
                .IsUnique();

        }


    }
}

