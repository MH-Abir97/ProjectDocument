using Pronali.Data.Repositories.Accounts;
using Pronali.Data.Repositories.Core;
using Pronali.Data.Repositories.Hr;
using Pronali.Data.Repositories.Interfaces.Accounts;
using Pronali.Data.Repositories.Interfaces.Core;
using Pronali.Data.Repositories.Interfaces.Hr;
using Pronali.Data.Repositories.Interfaces.POS;
using Pronali.Data.Repositories.POS;
using Pronali.Data.Repositories;
using Pronali.Data.Repositories.Interfaces;
using System;
using Pronali.Data.Models.Entity.Hr;

namespace Pronali.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext db;
        public UnitOfWork(ApplicationDbContext _context)
        {
            db = _context;

        }

        //=============Core Start=====================///

        private ILoginHistoryRepository loginHistoryRepository;
        private ICommonRepository commonRepository;
        private ISmsHistoryRepository smsHistoryRepository;
        private IUserActivityRepository userActivityRepository;
        private ICompanyRepository companyRepository;
        private IBranchRepository branchRepository;
        private IDepartmentRepository departmentRepository;
        private IDesignationRepository designationRepository;
        private IFloorRepository floorRepository;
        private ISectionRepository sectionRepository;
        private ILineRepository lineRepository;
        private IMachineRepository machineRepository;
        private IEmailRepository emailRepository;
        private ISmsRepository smsRepository;
        private ISystemPreferenceRepository systemPreferenceRepository;
        private IDivisionRepository divisionRepository;
        private ISisterConcernRepository sisterConcernRepository;
        private IApplicationApprovalRepository applicationApprovalRepository;
        private INotificationRepository notificationRepository;

        //=============   HR  ============================///

        private ISuspendedRepository suspendedRepository;
        private ITerminateRepository terminateRepository;
        private ISalaryHeldupRepository salaryHeldupRepository;

        private IEmployeeTaskRepository EmployeeTaskRepository;
        private IHolidayRepository holidayRepository;
        private ILeaveRepository leaveRepository;
        private IEmployeeLeaveRepository employeeLeaveRepository;
        private ILeaveApplicationRepository leaveApplicationRepository;
        private IShortLeaveApplicationRepository shortLeaveApplicationRepository;
        private IShortBusinessApplicationRepository shortBusinessApplicationRepository;
        private IEmployeeRepository employeeRepository;
        private IBusinessApplicationRepository businessApplicationRepository;
        private IBranchHeadRepository branchHeadRepository;
        private ICompanyHeadRepository companyHeadRepository;
        private IEmployeeSalaryBaseRepository employeeSalaryBaseRepository;
        private IDepartmentalHeadRepository departmentalHeadRepository;
        private IDivisionalHeadRepository divisionalHeadRepository;
        private ISisterConcernHeadRepository sisterConcernHeadRepository;
        private ISalaryBreakupRepository salaryBreakupRepository;
        private ISalaryStructureRepository salaryStructureRepository;
        private ISalaryStructureDetailsRepository salaryStructureDetailsRepository;
        private IShiftRepository shiftRepository;
        private IJobLocationRepository jobLocationRepository;
        private IEmployeeGroupRepository employeeGroupRepository;
        private IEmployeeWeekendRepository employeeWeekendRepository;
        private ILatePermissionRepository latePermissionRepository;
        private IEarlyOutPermissionRepository earlyOutPermissionRepository;
        private IAttendanceMachineDataRepository attendanceMachineDataRepository;
        private IDailyAttendanceRepository dailyAttendanceRepository;
        private IDataProcessQueueRepository dataProcessQueueRepository;
        private IAttendanceProcessedDataRepository attendanceProcessedDataRepository;
        private IAttendanceMachineDataFilteredRepository attendanceMachineDataFilteredRepository;
        private IShiftDetailsRepository shiftDetailsRepository;
        private IRoasterRepository roasterRepository;
        private IRoasterGroupRepository roasterGroupRepository;
        private IRoasterGroupDetailsRepository roasterGroupDetailsRepository;
        private IRoasterGroupEmployeeRepository roasterGroupEmployeeRepository;

        private ILoanRepository loanRepository;
        private ILoanApplicationRepository loanApplicationRepository;
        private ILoanDisbursementRepository loanDisbursementRepository;
        private IProposedLoanInstallmentRepository proposedLoanInstallmentRepository;
        private IApprovedLoanInstallmentRepository approvedLoanInstallmentRepository;
        private IPromotionRepository promotionRepository;
        private IInsuranceRepository insuranceRepository;
        private IEmployeeIncomeTaxRepository employeeIncomeTaxRepository;
        private IEmployeeAssetRepository employeeAssetRepository;
        private IResignationRepository resignationRepository;

        private IManualAbsentRepository manualAbsentRepository;
        private IManualAttendanceRepository manualAttendanceRepository;
        private IEmployeFileRepository employeFileRepository;


        //=============   Accounts  ============================///

        private ILoanReceivableRepository loanReceivableRepository;
        private IAgentsRepository AgentsRepository;
        private IInvestorRepository InvestorRepository;
        private IOwnersEquityRepository OwnersEquityRepository;
        private IBankAccountRepository bankAccountRepository;
        private IAccountLedgerRepository accountLedgerRepository;

        private IAccountTypeRepository accountTypeRepository;
        private IAccountGroupRepository accountGroupRepository;
        private ISupplierRepository supplierRepository;
        private IDoctorRepository doctorRepository;
        private IHospitalRepository hospitalRepository;
        private IVendorRepository vendorRepository;
        private IVoucherHeadRepository voucherHeadRepository;
        private IVoucherDetailRepository voucherDetailRepository;
        private ITempVoucherHeadRepository tempVoucherHeadRepository;
        private ITempVoucherDetailRepository tempVoucherDetailRepository;
        private ITempVoucherRepository tempVoucherRepository;
        private IAccountSubLedgerRepository subLedgerRepository;
        private IAccountLedgerGroupRepository ledgerGroupRepository;
        private ICashInHandRepository cashInHandRepository;
        private IPrepaidExpenseRepository prepaidExpenseRepository;
        private IAdvanceForWorkRepository advanceForWorkRepository;
        private ISalaryAdvanceRepository salaryAdvanceRepository;
        private IVoucherRepository voucherRepository;
        private IVoucherNumberRepository voucherNumberRepository;
        private IWorkInProgressRepository workInProgressRepository;
        private IEquipmentRepository equipmentRepository;
        private IFurnitureAndFixtureRepository furnitureAndFixtureRepository;
        private IMachineryRepository machineryRepository;
        private IVechicleRepository vechicleRepository;
        private ITelephoneAndMobileRepository telephoneAndMobileRepository;
        private IRefrigeratorRepository refrigeratorRepository;
        private IPrinterAndScannerRepository printerAndScannerRepository;
        private ILaptopAndTabRepository laptopAndTabRepository;
        private ISecurityAdvanceRepository securityAdvanceRepository;
        private IAirConditionerRepository airConditionerRepository;
        private IComputerAndAccessoriesRepository computerAndAccessoriesRepository;
        private IOfficeDecorationRepository officeDecorationRepository;
        private IBuildingRepository buildingRepository;
        private IFactoryRepository factoryRepository;
        private ISoftwareRepository softwareRepository;
        private ICopyRightRepository copyRightRepository;
        private IAccountReceivableRepository accountReceivableRepository;
        private IMarketableSecurityRepository marketableSecurityRepository;
        private IVoucherHeadHistoryRepository voucherHeadHistoryRepository;
        private IVoucherDetailHistoryRepository voucherDetailHistoryRepository;



        


        public ICommonRepository Common
        {
            get
            {
                return commonRepository = commonRepository ?? new CommonRepository(db);
            }
        }

        //====================  Accounts Start  =====================///

        public IAccountReceivableRepository AccountReceivable
        {
            get
            {
                return accountReceivableRepository = accountReceivableRepository ?? new AccountReceivableRepository(db);
            }
        }

        public ILoanReceivableRepository LoanReceivable
        {
            get
            {
                return loanReceivableRepository = loanReceivableRepository ?? new LoanReceivableRepository(db);
            }
        }

        public IInvestorRepository Investor
        {
            get
            {
                return InvestorRepository = InvestorRepository ?? new InvestorRepository(db);
            }
        }

        public IOwnersEquityRepository OwnerEquity
        {
            get
            {
                return OwnersEquityRepository = OwnersEquityRepository ?? new OwnersEquityRepository(db);
            }
        }

        public IBankAccountRepository BankAccount
        {
            get
            {
                return bankAccountRepository = bankAccountRepository ?? new BankAccountRepository(db);
            }
        }

        public IAccountTypeRepository AccountType
        {
            get
            {
                return accountTypeRepository = accountTypeRepository ?? new AccountTypeRepository(db);
            }
        }

        public IAccountGroupRepository AccountGroup
        {
            get
            {
                return accountGroupRepository = accountGroupRepository ?? new AccountGroupRepository(db);
            }
        }

        public IAccountLedgerRepository AccountLedger
        {
            get
            {
                return accountLedgerRepository = accountLedgerRepository ?? new AccountLedgerRepository(db);
            }
        }

        public ISupplierRepository Supplier
        {
            get
            {
                return supplierRepository = supplierRepository ?? new SupplierRepository(db);
            }
        }

        public IDoctorRepository Doctor
        {
            get
            {
                return doctorRepository = doctorRepository ?? new DoctorRepository(db);
            }
        }

        public IHospitalRepository Hospital
        {
            get
            {
                return hospitalRepository = hospitalRepository ?? new HospitalRepository(db);
            }
        }

        public IVendorRepository Vendor
        {
            get
            {
                return vendorRepository = vendorRepository ?? new VendorRepository(db);
            }
        }
        public IVoucherHeadRepository VoucherHead
        {
            get
            {
                return voucherHeadRepository = voucherHeadRepository ?? new VoucherHeadRepository(db);
            }
        }

        public IVoucherDetailRepository VoucherDetail
        {
            get
            {
                return voucherDetailRepository = voucherDetailRepository ?? new VoucherDetailRepository(db);
            }
        }

        public ITempVoucherHeadRepository TempVoucherHead
        {
            get
            {
                return tempVoucherHeadRepository = tempVoucherHeadRepository ?? new TempVoucherHeadRepository(db);
            }
        }
        public ITempVoucherDetailRepository TempVoucherDetail
        {
            get
            {
                return tempVoucherDetailRepository = tempVoucherDetailRepository ?? new TempVoucherDetailRepository(db);
            }
        }

        public ITempVoucherRepository TempVoucher
        {
            get
            {
                return tempVoucherRepository = tempVoucherRepository ?? new TempVoucherRepository(db);
            }
        }

        public IAccountSubLedgerRepository AccountSubLedger
        {
            get
            {
                return subLedgerRepository = subLedgerRepository ?? new AccountSubLedgerRepository(db);
            }
        }

        public IAccountLedgerGroupRepository AccountLedgerGroup
        {
            get
            {
                return ledgerGroupRepository = ledgerGroupRepository ?? new AccountLedgerGroupRepository(db);
            }
        }

        public ICashInHandRepository CashInHand
        {
            get
            {
                return cashInHandRepository = cashInHandRepository ?? new CashInHandRepository(db);
            }
        }

        public IPrepaidExpenseRepository PrepaidExpense
        {
            get
            {
                return prepaidExpenseRepository = prepaidExpenseRepository ?? new PrepaidExpenseRepository(db);
            }
        }

        public IAdvanceForWorkRepository AdvanceForWork
        {
            get
            {
                return advanceForWorkRepository = advanceForWorkRepository ?? new AdvanceForWorkRepository(db);
            }
        }

        public ISalaryAdvanceRepository SalaryAdvance
        {
            get
            {
                return salaryAdvanceRepository = salaryAdvanceRepository ?? new SalaryAdvanceRepository(db);
            }
        }


        public IVoucherRepository Voucher
        {
            get
            {
                return voucherRepository = voucherRepository ?? new VoucherRepository(db);
            }
        }


        public IVoucherNumberRepository VoucherNumber
        {
            get
            {
                return voucherNumberRepository = voucherNumberRepository ?? new VoucherNumberRepository(db);
            }
        }


        public IWorkInProgressRepository WorkInProgress
        {
            get
            {
                return workInProgressRepository = workInProgressRepository ?? new WorkInProgressRepository(db);
            }
        }

        public IEquipmentRepository Equipment
        {
            get
            {
                return equipmentRepository = equipmentRepository ?? new EquipmentRepository(db);
            }
        }

        public IFurnitureAndFixtureRepository FurnitureAndFixture
        {
            get
            {
                return furnitureAndFixtureRepository = furnitureAndFixtureRepository ?? new FurnitureAndFixtureRepository(db);
            }
        }

        public IMachineryRepository Machinery
        {
            get
            {
                return machineryRepository = machineryRepository ?? new MachineryRepository(db);
            }
        }

        public ITelephoneAndMobileRepository TelephoneAndMobile
        {
            get
            {
                return telephoneAndMobileRepository = telephoneAndMobileRepository ?? new TelephoneAndMobileRepository(db);
            }
        }

        public IRefrigeratorRepository Refrigerator
        {
            get
            {
                return refrigeratorRepository = refrigeratorRepository ?? new RefrigeratorRepository(db);
            }
        }

        public IPrinterAndScannerRepository PrinterAndScanner
        {
            get
            {
                return printerAndScannerRepository = printerAndScannerRepository ?? new PrinterAndScannerRepository(db);
            }
        }

        public ILaptopAndTabRepository LaptopAndTab
        {
            get
            {
                return laptopAndTabRepository = laptopAndTabRepository ?? new LaptopAndTabRepository(db);
            }
        }

        public IMarketableSecurityRepository MarketableSecurity
        {
            get
            {
                return marketableSecurityRepository = marketableSecurityRepository ?? new MarketableSecurityRepository(db);
            }
        }

        public ISecurityAdvanceRepository SecurityAdvance
        {
            get
            {
                return securityAdvanceRepository = securityAdvanceRepository ?? new SecurityAdvanceRepository(db);
            }
        }

        public IAirConditionerRepository AirConditioner
        {
            get
            {
                return airConditionerRepository = airConditionerRepository ?? new AirConditionerRepository(db);
            }
        }

        public IComputerAndAccessoriesRepository ComputerAndAccessories
        {
            get
            {
                return computerAndAccessoriesRepository = computerAndAccessoriesRepository ?? new ComputerAndAccessoriesRepository(db);
            }
        }

        public IOfficeDecorationRepository OfficeDecoration
        {
            get
            {
                return officeDecorationRepository = officeDecorationRepository ?? new OfficeDecorationRepository(db);
            }
        }

        public IBuildingRepository Building
        {
            get
            {
                return buildingRepository = buildingRepository ?? new BuildingRepository(db);
            }
        }

        public IFactoryRepository Factory
        {
            get
            {
                return factoryRepository = factoryRepository ?? new FactoryRepository(db);
            }
        }

        public ISoftwareRepository Software
        {
            get
            {
                return softwareRepository = softwareRepository ?? new SoftwareRepository(db);
            }
        }

        public IVechicleRepository Vechicle
        {
            get
            {
                return vechicleRepository = vechicleRepository ?? new VechicleRepository(db);
            }
        }

        public ICopyRightRepository CopyRight
        {
            get
            {
                return copyRightRepository = copyRightRepository ?? new CopyRightRepository(db);
            }
        }

        public IVoucherHeadHistoryRepository VoucherHeadHistory
        {
            get
            {
                return voucherHeadHistoryRepository = voucherHeadHistoryRepository ?? new VoucherHeadHistoryRepository(db);
            }
        }

        public IVoucherDetailHistoryRepository VoucherDetailHistory
        {
            get
            {
                return voucherDetailHistoryRepository = voucherDetailHistoryRepository ?? new VoucherDetailHistoryRepository(db);
            }
        }

        public ILoginHistoryRepository LoginHistory
        {
            get
            {
                return loginHistoryRepository = loginHistoryRepository ?? new LogginHistoryRepository(db);
            }
        }

        public ISuspendedRepository Suspended
        {
            get
            {
                return suspendedRepository = suspendedRepository ?? new SuspendedRepository(db);
            }
        }

        public ITerminateRepository Terminate
        {
            get
            {
                return terminateRepository = terminateRepository ?? new TerminatedRepository(db);
            }
        }
        public ISalaryHeldupRepository SalaryHeldup
        {
            get
            {
                return salaryHeldupRepository = salaryHeldupRepository ?? new SalaryHeldupRepository(db);
            }
        }

        //=============   POS  ============================///
        private IProductRepository productRepository;
        private IProductGroupRepository productGroupRepository;
        private IProductCategoryRepository productCategoryRepository;
        private IProductPriceRepository productPriceRepository;
        private IConversionRepository conversionRepository;
        private IChallanDetailRepository challanDetailRepository;
        private IChallanRepository challanRepository;
        private ICounterRepository counterRepository;
        private ICouponRepository couponRepository;
        private ICustomerPointRepository customerPointRepository;
        private ICustomerPriceRepository customerPriceRepository;
        private IDiscountDetailRepository discountDetailRepository;
        private IDiscountRepository discountRepository;
        private IPackageUnitRepository packageUnitRepository;
        private IPrescriptionRepository prescriptionRepository;
        private IPriceRepository priceRepository;
        private IProductColorRepository productColorRepository;
        private IProductImageRepository productImageRepository;
        private IProductSizeRepository productSizeRepository;
        private IProductUnitRepository productUnitRepository;
        private IPromoCodeRepository promoCodeRepository;
        private IPurchaseItemRepository purchaseItemRepository;
        private IPurchaseOrderRepository purchaseOrderRepository;
        private ISalesItemRepository salesItemRepository;
        private ISalesOrderRepository salesOrderRepository;
        private ISalesTargetRepository salesTargetRepository;
        private ISchemeRepository schemeRepository;
        private IStoreRepository storeRepository;
        private ITempPurchaseRepository tempPurchaseRepository;
        private ITempSalesRepository tempSalesRepository;
        private ITransactionDetailRepository transactionDetailRepository;
        private ITransactionRepository transactionRepository;
        private IUnitRepository unitRepository;

        //====================  POS Start  =====================///

        public IAgentsRepository Agent
        {
            get
            {
                return AgentsRepository = AgentsRepository ?? new AgentsRepository(db);
            }
        }     

        public IProductRepository Product
        {
            get
            {
                return productRepository = productRepository ?? new ProductRepository(db);
            }
        }
        public IProductGroupRepository ProductGroup
        {
            get
            {
                return productGroupRepository = productGroupRepository ?? new ProductGroupRepository(db);
            }
        }
        public IProductCategoryRepository ProductCategory
        {
            get
            {
                return productCategoryRepository = productCategoryRepository ?? new ProductCategoryRepository(db);
            }
        }
        public IProductPriceRepository ProductPrice
        {
            get
            {
                return productPriceRepository = productPriceRepository ?? new ProductPriceRepository(db);
            }
        }
        public IConversionRepository Conversion
        {
            get
            {
                return conversionRepository = conversionRepository ?? new ConversionRepository(db);
            }
        }
        public IChallanDetailRepository ChallanDetail
        {
            get
            {
                return challanDetailRepository = challanDetailRepository ?? new ChallanDetailRepository(db);
            }
        }
        public IChallanRepository Challan
        {
            get
            {
                return challanRepository = challanRepository ?? new ChallanRepository(db);
            }
        }
        public ICounterRepository Counter
        {
            get
            {
                return counterRepository = counterRepository ?? new CounterRepository(db);
            }
        }
        public ICouponRepository Coupon
        {
            get
            {
                return couponRepository = couponRepository ?? new CouponRepository(db);
            }
        }
        public ICustomerPointRepository CustomerPoint
        {
            get
            {
                return customerPointRepository = customerPointRepository ?? new CustomerPointRepository(db);
            }
        }
        public ICustomerPriceRepository CustomerPrice
        {
            get
            {
                return customerPriceRepository = customerPriceRepository ?? new CustomerPriceRepository(db);
            }
        }
        public IDiscountDetailRepository DiscountDetail
        {
            get
            {
                return discountDetailRepository = discountDetailRepository ?? new DiscountDetailRepository(db);
            }
        }
        public IDiscountRepository Discount
        {
            get
            {
                return discountRepository = discountRepository ?? new DiscountRepository(db);
            }
        }
        public IPackageUnitRepository PackageUnit
        {
            get
            {
                return packageUnitRepository = packageUnitRepository ?? new PackageUnitRepository(db);
            }
        }
        public IPrescriptionRepository Prescription
        {
            get
            {
                return prescriptionRepository = prescriptionRepository ?? new PrescriptionRepository(db);
            }
        }
        public IPriceRepository Price
        {
            get
            {
                return priceRepository = priceRepository ?? new PriceRepository(db);
            }
        }
        public IProductColorRepository ProductColor
        {
            get
            {
                return productColorRepository = productColorRepository ?? new ProductColorRepository(db);
            }
        }
        public IProductImageRepository ProductImage
        {
            get
            {
                return productImageRepository = productImageRepository ?? new ProductImageRepository(db);
            }
        }
        public IProductSizeRepository ProductSize
        {
            get
            {
                return productSizeRepository = productSizeRepository ?? new ProductSizeRepository(db);
            }
        }
        public IProductUnitRepository ProductUnit
        {
            get
            {
                return productUnitRepository = productUnitRepository ?? new ProductUnitRepository(db);
            }
        }
        public IPromoCodeRepository PromoCode
        {
            get
            {
                return promoCodeRepository = promoCodeRepository ?? new PromoCodeRepository(db);
            }
        }
        public IPurchaseItemRepository PurchaseItem
        {
            get
            {
                return purchaseItemRepository = purchaseItemRepository ?? new PurchaseItemRepository(db);
            }
        }
        public IPurchaseOrderRepository PurchaseOrder
        {
            get
            {
                return purchaseOrderRepository = purchaseOrderRepository ?? new PurchaseOrderRepository(db);
            }
        }
        public ISalesItemRepository SalesItem
        {
            get
            {
                return salesItemRepository = salesItemRepository ?? new SalesItemRepository(db);
            }
        }
        public ISalesOrderRepository SalesOrder
        {
            get
            {
                return salesOrderRepository = salesOrderRepository ?? new SalesOrderRepository(db);
            }
        }
        public ISalesTargetRepository SalesTarget
        {
            get
            {
                return salesTargetRepository = salesTargetRepository ?? new SalesTargetRepository(db);
            }
        }
        public ISchemeRepository Scheme
        {
            get
            {
                return schemeRepository = schemeRepository ?? new SchemeRepository(db);
            }
        }
        public IStoreRepository Store
        {
            get
            {
                return storeRepository = storeRepository ?? new StoreRepository(db);
            }
        }
        public ITempPurchaseRepository TempPurchase
        {
            get
            {
                return tempPurchaseRepository = tempPurchaseRepository ?? new TempPurchaseRepository(db);
            }
        }
        public ITempSalesRepository TempSales
        {
            get
            {
                return tempSalesRepository = tempSalesRepository ?? new TempSalesRepository(db);
            }
        }
        public ITransactionDetailRepository TransactionDetail
        {
            get
            {
                return transactionDetailRepository = transactionDetailRepository ?? new TransactionDetailRepository(db);
            }
        }
        public ITransactionRepository Transaction
        {
            get
            {
                return transactionRepository = transactionRepository ?? new TransactionRepository(db);
            }
        }
        public IUnitRepository Unit
        {
            get
            {
                return unitRepository = unitRepository ?? new UnitRepository(db);
            }
        }


        //====================  Core Start  =====================///

        //private ISmsHistoryRepository smsHistoryRepository;
       // private IUserActivityRepository userActivityRepository;
        public ISmsHistoryRepository SmsHistory
        {
            get
            {
                return smsHistoryRepository = smsHistoryRepository ?? new SmsHistoryRepository(db);
            }
        }

        public IUserActivityRepository UserActivity
        {
            get
            {
                return userActivityRepository = userActivityRepository ?? new UserActivityRepository(db);
            }
        }

        public ICompanyRepository Company
        {
            get
            {
                return companyRepository = companyRepository ?? new CompanyRepository(db);
            }
        }
        public ISisterConcernRepository SisterConcern
        {
            get
            {
                return sisterConcernRepository = sisterConcernRepository ?? new SisterConcernRepository(db);
            }
        }
        public IDivisionRepository Division
        {
            get
            {
                return divisionRepository = divisionRepository ?? new DivisionRepository(db);
            }
        }
        public IBranchRepository Branch
        {
            get
            {
                return branchRepository = branchRepository ?? new BranchRepository(db);
            }
        }
        public IDepartmentRepository Department
        {
            get
            {
                return departmentRepository = departmentRepository ?? new DepartmentRepository(db);
            }
        }
        public IDesignationRepository Designation
        {
            get
            {
                return designationRepository = designationRepository ?? new DesignationRepository(db);
            }
        }
        public ISectionRepository Section
        {
            get
            {
                return sectionRepository = sectionRepository ?? new SectionRepository(db);
            }
        }
        public IFloorRepository Floor
        {
            get
            {
                return floorRepository = floorRepository ?? new FloorRepository(db);
            }
        }
        public ILineRepository Line
        {
            get
            {
                return lineRepository = lineRepository ?? new LineRepository(db);
            }
        }
        public IMachineRepository Machine
        {
            get
            {
                return machineRepository = machineRepository ?? new MachineRepository(db);
            }
        }
        public IManualAbsentRepository ManualAbsent
        {
            get
            {
                return manualAbsentRepository = manualAbsentRepository ?? new ManualAbsentRepository(db);
            }
        }
        public IManualAttendanceRepository ManualAttendance
        {
            get
            {
                return manualAttendanceRepository = manualAttendanceRepository ?? new ManualAttendanceRepository(db);
            }
        }

        public IEmailRepository Email
        {
            get
            {
                return emailRepository = emailRepository ?? new EmailRepository(db);
            }
        }

        public ISmsRepository SMS
        {
            get
            {
                return smsRepository = smsRepository ?? new SmsRepository(db);
            }
        }
        public ISystemPreferenceRepository SystemPreference
        {
            get
            {
                return systemPreferenceRepository = systemPreferenceRepository ?? new SystemPreferenceRepository(db);
            }
        }

        public IApplicationApprovalRepository ApplicationApproval
        {
            get
            {
                return applicationApprovalRepository = applicationApprovalRepository ?? new ApplicationApprovalRepository(db);
            }
        }

        public INotificationRepository Notification
        {
            get
            {
                return notificationRepository = notificationRepository ?? new NotificationRepository(db);
            }
        }


        /////================== HR  Start ============================///



        public IEmployeFileRepository EmployeFile
        {
            get
            {
                return employeFileRepository = employeFileRepository ?? new EmployeFileRepository(db);
            }
        }
        public IResignationRepository Resignation
        {
            get
            {
                return resignationRepository = resignationRepository ?? new ResignationRepository(db);
            }
        }
        public IBranchHeadRepository BranchHead
        {
            get
            {
                return branchHeadRepository = branchHeadRepository ?? new BranchHeadRepository(db);
            }
        }

        public ICompanyHeadRepository CompanyHead
        {
            get
            {
                return companyHeadRepository = companyHeadRepository ?? new CompanyHeadRepository(db);
            }
        }

        public IDepartmentalHeadRepository DepartmentalHead
        {
            get
            {
                return departmentalHeadRepository = departmentalHeadRepository ?? new DepartmentalHeadRepository(db);
            }
        }

        public IDivisionalHeadRepository DivisionalHead
        {
            get
            {
                return divisionalHeadRepository = divisionalHeadRepository ?? new DivisionalHeadRepository(db);
            }
        }

        public ISisterConcernHeadRepository SisterConcernHead
        {
            get
            {
                return sisterConcernHeadRepository = sisterConcernHeadRepository ?? new SisterConcernHeadRepository(db);
            }
        }

        public IEmployeeTaskRepository EmployeeTask
        {
            get
            {
                return EmployeeTaskRepository = EmployeeTaskRepository ?? new EmployeeTaskRepository(db);
            }
        }

        public IEmployeeAssetRepository EmployeeAsset
        {
            get
            {
                return employeeAssetRepository = employeeAssetRepository ?? new EmployeeAssetRepository(db);
            }
        }
        public IEmployeeIncomeTaxRepository EmployeeIncomeTax
        {
            get
            {
                return employeeIncomeTaxRepository = employeeIncomeTaxRepository ?? new EmployeeIncomeTaxRepository(db);
            }
        }
        public IInsuranceRepository Insurance
        {
            get
            {
                return insuranceRepository = insuranceRepository ?? new InsuranceRepository(db);
            }
        }
        public IHolidayRepository Holiday
        {
            get
            {
                return holidayRepository = holidayRepository ?? new HolidayRepository(db);
            }
        }
        public IPromotionRepository Promotion
        {
            get
            {
                return promotionRepository = promotionRepository ?? new PromotionRepository(db);
            }
        }

        public ILeaveRepository Leave
        {
            get
            {
                return leaveRepository = leaveRepository ?? new LeaveRepository(db);
            }
        }

        public IEmployeeLeaveRepository EmployeeLeave
        {
            get
            {
                return employeeLeaveRepository = employeeLeaveRepository ?? new EmployeeLeaveRepository(db);
            }
        }
        public IEmployeeWeekendRepository EmployeeWeekend
        {
            get
            {
                return employeeWeekendRepository = employeeWeekendRepository ?? new EmployeeWeekendRepository(db);
            }
        }
        public ILeaveApplicationRepository LeaveApplication
        {
            get
            {
                return leaveApplicationRepository = leaveApplicationRepository ?? new LeaveApplicationRepository(db);
            }
        }

        public IShortLeaveApplicationRepository ShortLeaveApplication
        {
            get
            {
                return shortLeaveApplicationRepository = shortLeaveApplicationRepository ?? new ShortLeaveApplicationRepository(db);
            }
        }

        public IShortBusinessApplicationRepository ShortBusinessApplication
        {
            get
            {
                return shortBusinessApplicationRepository = shortBusinessApplicationRepository ?? new ShortBusinessApplicationRepository(db);
            }
        }

        public IEmployeeRepository Employee
        {
            get
            {
                return employeeRepository = employeeRepository ?? new EmployeeRepository(db);
            }
        }

        public IBusinessApplicationRepository BusinessApplication
        {
            get
            {
                return businessApplicationRepository = businessApplicationRepository ?? new BusinessApplicationRepository(db);
            }
        }


        public IEmployeeSalaryBaseRepository EmployeeSalaryBase
        {
            get
            {
                return employeeSalaryBaseRepository = employeeSalaryBaseRepository ?? new EmployeeSalaryBaseRepository(db);
            }
        }
        public ISalaryBreakupRepository SalaryBreakup
        {
            get
            {
                return salaryBreakupRepository = salaryBreakupRepository ?? new SalaryBreakupRepository(db);
            }
        }
        public ISalaryStructureRepository SalaryStructure
        {
            get
            {
                return salaryStructureRepository = salaryStructureRepository ?? new SalaryStructureRepository(db);
            }
        }
        public ISalaryStructureDetailsRepository SalaryStructureDetails
        {
            get
            {
                return salaryStructureDetailsRepository = salaryStructureDetailsRepository ?? new SalaryStructureDetailsRepository(db);
            }
        }

        public IShiftRepository Shift
        {
            get { return shiftRepository = shiftRepository ?? new ShiftRepository(db); }
        }

        public IJobLocationRepository JobLocation
        {
            get { return jobLocationRepository = jobLocationRepository ?? new JobLocationRepository(db); }
        }
        public IEmployeeGroupRepository EmployeeGroup
        {
            get { return employeeGroupRepository = employeeGroupRepository ?? new EmployeeGroupRepository(db); }
        }

        public ILatePermissionRepository LatePermission
        {
            get { return latePermissionRepository = latePermissionRepository ?? new LatePermissionRepository(db); }
        }

        public IEarlyOutPermissionRepository EarlyOutPermission
        {
            get { return earlyOutPermissionRepository = earlyOutPermissionRepository ?? new EarlyOutPermissionRepository(db); }
        }


       
        public IAttendanceMachineDataRepository AttendanceMachineData
        {
            get { return attendanceMachineDataRepository = attendanceMachineDataRepository ?? new AttendanceMachineDataRepository(db); }
        }

        public IAttendanceProcessedDataRepository AttendanceProcessedData
        {
            get { return attendanceProcessedDataRepository = attendanceProcessedDataRepository ?? new AttendanceProcessedDataRepository(db); }
        }
        public IDataProcessQueueRepository DataProcessQueue
        {
            get { return dataProcessQueueRepository = dataProcessQueueRepository ?? new DataProcessQueueRepository(db); }
        }
        public IDailyAttendanceRepository DailyAttendance
        {
            get { return dailyAttendanceRepository = dailyAttendanceRepository ?? new DailyAttendanceRepository(db); }
        }

        public IAttendanceMachineDataFilteredRepository AttendanceMachineDataFiltered
        {
            get { return attendanceMachineDataFilteredRepository = attendanceMachineDataFilteredRepository ?? new AttendanceMachineDataFilteredRepository(db); }
        }

        public IShiftDetailsRepository ShiftDetails
        {
            get { return shiftDetailsRepository = shiftDetailsRepository ?? new ShiftDetailsRepository(db); }
        }

        public IRoasterRepository Roaster
        {
            get { return roasterRepository = roasterRepository ?? new RoasterRepository(db); }
        }

        public IRoasterGroupRepository RoasterGroup
        {
            get { return roasterGroupRepository = roasterGroupRepository ?? new RoasterGroupRepository(db); }
        }

        public IRoasterGroupDetailsRepository RoasterGroupDetails
        {
            get { return roasterGroupDetailsRepository = roasterGroupDetailsRepository ?? new RoasterGroupDetailsRepository(db); }
        }

        public IRoasterGroupEmployeeRepository RoasterGroupEmployee
        {
            get { return roasterGroupEmployeeRepository = roasterGroupEmployeeRepository ?? new RoasterGroupEmployeeRepository(db); }
        }
        public ILoanRepository Loan
        {
            get { return loanRepository = loanRepository ?? new LoanRepository(db); }
        }


        public ILoanApplicationRepository LoanApplication
        {
            get { return loanApplicationRepository = loanApplicationRepository ?? new LoanApplicationRepository(db); }
        }

        public ILoanDisbursementRepository LoanDisbursement
        {
            get { return loanDisbursementRepository = loanDisbursementRepository ?? new LoanDisbursementRepository(db); }
        }

        public IProposedLoanInstallmentRepository ProposedLoanInstallment
        {
            get { return proposedLoanInstallmentRepository = proposedLoanInstallmentRepository ?? new ProposedLoanInstallmentRepository(db); }
        }

        public IApprovedLoanInstallmentRepository ApprovedLoanInstallment
        {
            get
            {
                return approvedLoanInstallmentRepository = approvedLoanInstallmentRepository ?? new ApprovedLoanInstallmentRepository(db);
            }
        }

        //public int Save()
        //{
        //    return db.SaveChanges();
        //}

        public int Save()
        {
            var code = 0;

            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    code = db.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                }
            }
            return code;
        }

        public void Dispose()
        {
            db.Dispose();
        }

    }
}
