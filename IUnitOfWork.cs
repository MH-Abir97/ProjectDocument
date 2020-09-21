using Pronali.Data.Repositories.Interfaces;
using Pronali.Data.Repositories.Interfaces.Accounts;
using Pronali.Data.Repositories.Interfaces.Core;
using Pronali.Data.Repositories.Interfaces.Hr;
using Pronali.Data.Repositories.Interfaces.POS;

namespace Pronali.Data
{
    public interface IUnitOfWork
    {


        //HR//
        ISuspendedRepository Suspended { get; }
        ITerminateRepository Terminate { get; }
        ISalaryHeldupRepository SalaryHeldup { get; }
        IEmployeFileRepository EmployeFile { get; }
        //Core ///


        ILoginHistoryRepository LoginHistory { get; }

        ISmsHistoryRepository SmsHistory { get; }
        IUserActivityRepository UserActivity { get; }



        ICommonRepository Common { get; }
        ICompanyRepository Company { get; }
        IBranchRepository Branch { get; }
        IDepartmentRepository Department { get; }
        IDivisionalHeadRepository DivisionalHead { get; }
        IDesignationRepository Designation { get; }
        IFloorRepository Floor { get; }
        ISectionRepository Section { get; }
        IMachineRepository Machine { get; }
        ILineRepository Line { get; }
        ILeaveRepository Leave { get; }
        IHolidayRepository Holiday { get; }
        IPromotionRepository Promotion { get; }
        IInsuranceRepository Insurance { get; }
        IEmployeeIncomeTaxRepository EmployeeIncomeTax { get; }
        IEmployeeAssetRepository EmployeeAsset { get; }
        IEmployeeLeaveRepository EmployeeLeave { get; }
        IEmployeeTaskRepository EmployeeTask { get; }
        ILeaveApplicationRepository LeaveApplication { get; }
        IShortLeaveApplicationRepository ShortLeaveApplication { get; }
        IShortBusinessApplicationRepository ShortBusinessApplication { get; }
        IEmployeeRepository Employee { get; }
        IDepartmentalHeadRepository DepartmentalHead { get; }
        IBusinessApplicationRepository BusinessApplication { get; }
        IBranchHeadRepository BranchHead { get; }
        ICompanyHeadRepository CompanyHead { get; }
        ILatePermissionRepository LatePermission { get; }
        IEarlyOutPermissionRepository EarlyOutPermission { get; }
        IEmailRepository Email { get; }
        ISmsRepository SMS { get; }
        IEmployeeSalaryBaseRepository EmployeeSalaryBase { get; }
        ISisterConcernHeadRepository SisterConcernHead { get; }
        ISalaryBreakupRepository SalaryBreakup { get; }
        ISalaryStructureRepository SalaryStructure { get; }
        ISalaryStructureDetailsRepository SalaryStructureDetails { get; }
        IShiftRepository Shift { get; }
        IEmployeeGroupRepository EmployeeGroup { get; }
        IJobLocationRepository JobLocation { get; }
        IEmployeeWeekendRepository EmployeeWeekend { get; }
        IAttendanceMachineDataRepository AttendanceMachineData { get; }
        IDailyAttendanceRepository DailyAttendance { get; }
        IDataProcessQueueRepository DataProcessQueue { get; }
        IAttendanceProcessedDataRepository AttendanceProcessedData { get; }
        IAttendanceMachineDataFilteredRepository AttendanceMachineDataFiltered { get; }
        IShiftDetailsRepository ShiftDetails { get; }
        IResignationRepository Resignation { get; }


        IRoasterRepository Roaster { get; }
        IRoasterGroupRepository RoasterGroup { get; }
        IRoasterGroupDetailsRepository RoasterGroupDetails { get; }
        IRoasterGroupEmployeeRepository RoasterGroupEmployee { get; }
        ISisterConcernRepository SisterConcern { get; }
        IDivisionRepository Division { get; }
        ILoanRepository Loan { get; }
        ILoanApplicationRepository LoanApplication { get; }
        ILoanDisbursementRepository LoanDisbursement { get; }
        IProposedLoanInstallmentRepository ProposedLoanInstallment { get; }
        IApprovedLoanInstallmentRepository ApprovedLoanInstallment { get; }
        IApplicationApprovalRepository ApplicationApproval { get; }
        IManualAbsentRepository ManualAbsent { get; }
        IManualAttendanceRepository ManualAttendance { get; }
        INotificationRepository Notification { get; }




        //Accounts 

        IAgentsRepository Agent { get; }
        IOwnersEquityRepository OwnerEquity { get; }
        IInvestorRepository Investor { get; }
        ILoanReceivableRepository LoanReceivable { get; }
        IBankAccountRepository BankAccount { get; }
        IAccountTypeRepository AccountType { get; }
        IAccountGroupRepository AccountGroup { get; }
        IAccountLedgerRepository AccountLedger { get; }

        ISupplierRepository Supplier { get; }
        IDoctorRepository Doctor { get; }
        IHospitalRepository Hospital { get; }
        IVendorRepository Vendor { get; }
        IVoucherRepository Voucher { get; }
        IVoucherDetailRepository VoucherDetail { get; }
        ITempVoucherHeadRepository TempVoucherHead { get; }
        ITempVoucherDetailRepository TempVoucherDetail { get; }
        ITempVoucherRepository TempVoucher { get; }
        IAccountSubLedgerRepository AccountSubLedger { get; }
        IAccountLedgerGroupRepository AccountLedgerGroup { get; }
        ICashInHandRepository CashInHand { get; }
        IVoucherHeadRepository VoucherHead { get; }
        IVoucherNumberRepository VoucherNumber { get; }
        IPrepaidExpenseRepository PrepaidExpense { get; }
        IAdvanceForWorkRepository AdvanceForWork { get; }
        IWorkInProgressRepository WorkInProgress { get; }
        IEquipmentRepository Equipment { get; }
        IFurnitureAndFixtureRepository FurnitureAndFixture { get; }
        IMachineryRepository Machinery { get; }
        IVechicleRepository Vechicle { get; }
        ITelephoneAndMobileRepository TelephoneAndMobile { get; }
        IRefrigeratorRepository Refrigerator { get; }
        IPrinterAndScannerRepository PrinterAndScanner { get; }
        ILaptopAndTabRepository LaptopAndTab { get; }
        ISecurityAdvanceRepository SecurityAdvance { get; }
        IAirConditionerRepository AirConditioner { get; }
        IComputerAndAccessoriesRepository ComputerAndAccessories { get; }
        IOfficeDecorationRepository OfficeDecoration { get; }
        IBuildingRepository Building { get; }
        IFactoryRepository Factory { get; }
        ISoftwareRepository Software { get; }
        ICopyRightRepository CopyRight { get; }
        IAccountReceivableRepository AccountReceivable { get; }
        ISalaryAdvanceRepository SalaryAdvance { get; }
        IMarketableSecurityRepository MarketableSecurity { get; }
        IVoucherHeadHistoryRepository VoucherHeadHistory { get; }
        IVoucherDetailHistoryRepository VoucherDetailHistory { get; }





        //Pos
        IProductRepository Product { get; }
        IProductGroupRepository ProductGroup { get; }
        IProductCategoryRepository ProductCategory { get; }
        IProductPriceRepository ProductPrice { get; }
        IConversionRepository Conversion { get; }
        IUnitRepository Unit { get; }
        ITransactionRepository Transaction { get; }
        ITransactionDetailRepository TransactionDetail { get; }
        ITempSalesRepository TempSales { get; }
        ITempPurchaseRepository TempPurchase { get; }
        IStoreRepository Store { get; }
        ISchemeRepository Scheme { get; }
        ISalesTargetRepository SalesTarget { get; }
        ISalesOrderRepository SalesOrder { get; }
        ISalesItemRepository SalesItem { get; }
        IPurchaseOrderRepository PurchaseOrder { get; }
        IPurchaseItemRepository PurchaseItem { get; }
        IPromoCodeRepository PromoCode { get; }
        IProductUnitRepository ProductUnit { get; }
        IProductSizeRepository ProductSize { get; }
        IProductImageRepository ProductImage { get; }
        IProductColorRepository ProductColor { get; }
        IPriceRepository Price { get; }
        IPrescriptionRepository Prescription { get; }
        IPackageUnitRepository PackageUnit { get; }
        IDiscountRepository Discount { get; }
        IDiscountDetailRepository DiscountDetail { get; }
        ICustomerPriceRepository CustomerPrice { get; }
        ICustomerPointRepository CustomerPoint { get; }
        ICouponRepository Coupon { get; }
        ICounterRepository Counter { get; }
        IChallanRepository Challan { get; }
        IChallanDetailRepository ChallanDetail { get; }
        ISystemPreferenceRepository SystemPreference { get; }


        int Save();
    }
}
