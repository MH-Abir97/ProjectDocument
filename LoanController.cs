using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pronali.Web.Controllers;
using Pronali.Web.Areas.HR.Models.Loan;
using Pronali.Data.Models.Entity.Core;
using Pronali.Web.Areas.HR.Models.Employee;
using Pronali.Web.Extension;
using Pronali.Data;
using Pronali.Data.Enum;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.LoanApplication;
using Pronali.Web.Areas.HR.Models.LoanDisbursement;
using Pronali.Web.Areas.HR.Models.ProposedLoanInstallment;
using Pronali.Web.Areas.HR.Models.ApprovedLoanInstallment;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class LoanController : BaseController
    {
        private readonly IUnitOfWork db;

        public LoanController( IUnitOfWork _unitOfWork ):base (_unitOfWork)
        {
            db = _unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MainLoanApplication()
        {
            return View();
        }

        public IActionResult MainLoanDisbursement()
        {
            return View();
        }
        public IActionResult LoanApplication()
        {
            return PartialView("LoanApplication");
        }

        public IActionResult Create()
        {
            return PartialView("Create");
        }

        public IActionResult GetDropdownList()
        {
            var result = db.Loan.GetAll().ToList();
            return Json(result);
        }

        //GetLoanApplication Id

        [HttpPost]
        public IActionResult GetLoanApplicationId( int Id)
        {
            var result = db.LoanApplication.GetAll().Where(model => model.ApplierId == Id).ToList();
            return Json(result);
        }

        //Get Proposed Loan Application values
        [HttpPost]
        public IActionResult ProposedLoanInstallemtSchedule( int Id )
        {
            var data = db.ProposedLoanInstallment.GetAll().Where( modelData => modelData.LoanApplicationId == Id);
            List<vmProposedLoanInstallment> Lists = new List<vmProposedLoanInstallment>();
            foreach( var items in data )
            {
                vmProposedLoanInstallment getData = new vmProposedLoanInstallment();
                getData.InstallmentDate = items.InstallmentDate;
                getData.Amount = items.Amount;
                getData.LoanApplicationId = items.LoanApplicationId;
                Lists.Add(getData);
                
            }
            return Json(Lists);
        }

        //GetAll things in Loan Disbursement

        [HttpPost]
        public IActionResult GetAllThings( int Id )
        {
            var ProposedAmount = db.LoanApplication.GetAll().Where(model => model.Id == Id);
            var AcceptList = db.ApprovedLoanInstallment.GetAll().Where(model=>model.LoanApplicationId == Id).ToList();
            var AcceptedSum = AcceptList.Select(model => model.Amount).Sum();
            var ProposedLoanList = db.ProposedLoanInstallment.GetAll().Where(model => model.LoanApplicationId == Id).ToList();
            return Json( new { ProposedAmount, AcceptList, ProposedLoanList, AcceptedSum } );

        }

        // Save Acccept Loan Installment

        [HttpPost]
        public IActionResult SaveApprovedLoanApplication( vmLoanApplication modelData )
        {
            if(ModelState.IsValid)
            {
                decimal FinalAmount = Math.Round(modelData.AppliedAmount / modelData.InstallmentNo, 3);
                for (int i = 0; i < modelData.ScheduleDate.Count; i++) {
                    ApprovedLoanInstallment saveData = new ApprovedLoanInstallment();
                    saveData.LoanApplicationId = modelData.Id;
                    saveData.Amount = FinalAmount;
                    saveData.InstallmentDate = modelData.ScheduleDate[i];
                    db.ApprovedLoanInstallment.Add(saveData);
                    db.Save();
                }
                return Json(true);
            } 
            return Json(false);
        }

        [HttpPost]
        public IActionResult SaveLoanApplication( vmLoanApplication modelData)
        {
            LoanApplication save = new LoanApplication();
            save.LoanId = modelData.LoanId;
            save.Reason = modelData.Reason;
            save.AppliedAmount = modelData.AppliedAmount;
            save.InstallmentNo = modelData.InstallmentNo;
            save.AppliedDate = modelData.AppliedDate;
            save.LoanPeriod = modelData.LoanPeriod;
            save.ApplierId = modelData.ApplierId;
            db.LoanApplication.Add(save);
            db.Save();
            int LoanApplicationId = save.Id;
            decimal InstallmentAmount = Math.Round(modelData.AppliedAmount / modelData.InstallmentNo, 2);
            for( int i = 0; i < modelData.ScheduleDate.Count; i++)
            {
                ProposedLoanInstallment SaveData = new ProposedLoanInstallment();
                SaveData.InstallmentDate = Convert.ToDateTime(modelData.ScheduleDate[i]);
                SaveData.LoanApplicationId = LoanApplicationId;
                SaveData.Amount = InstallmentAmount;
                db.ProposedLoanInstallment.Add(SaveData);
                db.Save();
            }
            return Json("success");
        }

        [HttpPost]
        public IActionResult LoadLoanApplicationTableData()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            
            List<vmLoanApplication> LoanApplicationList = new List<vmLoanApplication>();
            var AllLoanApplicaton = db.LoanApplication.GetAll();

            //Sorting
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                // AllLoanApplicaton = AllLoanApplicaton.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                AllLoanApplicaton = AllLoanApplicaton.OrderByDescending(model => model.Id).ToList();
            }

            //Search
            if (!string.IsNullOrEmpty(searchValue))
            {
               AllLoanApplicaton = AllLoanApplicaton.Where(x => x.Loan.Name.Contains(searchValue) || x.Reason.Contains(searchValue) ||
                x.InstallmentNo == Convert.ToInt32(searchValue) || x.AppliedAmount == Convert.ToInt32(searchValue)
                ).ToList();

            }

            foreach( var items in AllLoanApplicaton)
            {
                vmLoanApplication getData = new vmLoanApplication();
                getData.Id = items.Id;
                getData.LoanId = items.LoanId;
                getData.Reason = items.Reason;
                getData.InstallmentNo = items.InstallmentNo;
                getData.AppliedAmount = items.AppliedAmount;
                getData.LoanPeriod = items.LoanPeriod;
                getData.AppliedDate = items.AppliedDate;
                getData.ApplierId = items.ApplierId;
                LoanApplicationList.Add(getData);
            }


            recordsTotal = LoanApplicationList.Count();

            //Paging     
            var data = LoanApplicationList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            
        }

        [HttpPost]

        public IActionResult Create( vmLoan modelData )
        {
           if(ModelState.IsValid)
            {
                Loan loan = new Loan
                {
                    Name = modelData.Name,
                    LoanType = modelData.LoanType,
                    CalculationMethod = modelData.CalculationMethod,
                    EligiblePerson = modelData.EligiblePerson,
                    Percentage = modelData.Percentage,
                    SalaryBreakup = modelData.SalaryBreakup
                };
                db.Loan.Add(loan);
                db.Save();
                ModelState.Clear();
            }
            return Json(true);
        }

        [HttpPost]
        public IActionResult UpdateLoan(vmLoan modelData)
        {
            var getLoan = db.Loan.Get(modelData.Id);
            if( getLoan != null)
            {
                getLoan.Name = modelData.Name;
                getLoan.LoanType = modelData.LoanType;
                getLoan.CalculationMethod = modelData.CalculationMethod;
                getLoan.EligiblePerson = modelData.EligiblePerson;
                getLoan.Percentage = modelData.Percentage;
                getLoan.SalaryBreakup = modelData.SalaryBreakup;
                db.Save();
                return Json("success");
            }

            return Json("error");
        }

        [HttpPost]
        public IActionResult DeleteLoan( int Id)
        {
            var result = db.Loan.Get(Id);
            if( result != null)
            {
                db.Loan.Remove(result);
                db.Save();
                return Json("success");
            }

            return Json("error");
        }

        [HttpPost]
        public IActionResult LoadLoanTableData()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var AllLoans = db.Loan.GetAll().Where(d => d.IsActive == true && d.IsDeleted == false);
            List<vmLoan> LoanList = new List<vmLoan>();

            //Sorting
            if(!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
               // AllLoans = AllLoans.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                AllLoans = AllLoans.OrderByDescending(model => model.Id).ToList();
            }

            //Search
            if( !string.IsNullOrEmpty(searchValue))
            {
                AllLoans = AllLoans.Where(model => model.Name.Contains(searchValue) || model.LoanType.Contains(searchValue) ||
                           model.CalculationMethod.Contains(searchValue) || model.EligiblePerson.Contains(searchValue)).ToList();

            }

            foreach( var Items in AllLoans)
            {
                vmLoan getData = new vmLoan();
                getData.Id = Items.Id;
                getData.Name = Items.Name;
                getData.LoanType = Items.LoanType;
                getData.CalculationMethod = Items.CalculationMethod;
                getData.EligiblePerson = Items.EligiblePerson;
                getData.Percentage = Items.Percentage;
                getData.SalaryBreakup = Items.SalaryBreakup;
                LoanList.Add(getData);
            }
            //total number of rows count     
            recordsTotal = LoanList.Count();

            //Paging     
            var data = LoanList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json( new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data } );
        }

        [HttpPost]
        public IActionResult UpdateLoanApplication(vmLoanApplication modelData)
        {
            var result = db.LoanApplication.Get(modelData.Id);
            if( result != null)
            {
                result.LoanId = modelData.LoanId;
                result.AppliedAmount = modelData.AppliedAmount;
                result.Reason = modelData.Reason;
                result.InstallmentNo = modelData.InstallmentNo;
                db.Save();
                return Json("success");
            }
            return Json("error");
        }

        [HttpPost]
        public IActionResult DeleteLoanApplication(int Id)
        {
            var result = db.LoanApplication.Get(Id);
            if( result != null)
            {
                db.LoanApplication.Remove(result);
                db.Save();
                return Json("success");
            }
            return Json("error");
        }


        // Loan Disbursement Operations

        public IActionResult LoanDisbursementView()
        {
            return PartialView("LoanDisbursementView");
        }

        public IActionResult GetEmployeeDropdown()
        {
            var data = db.Employee.GetAll().ToList();
            return Json(data);
        }

        [HttpPost]
        public IActionResult SaveDisbursementData(vmLoanDisbursement DisbursementData )
        {
            if(ModelState.IsValid)
            {
                LoanDisbursement loanDisbursement = new LoanDisbursement();
                loanDisbursement.ApprovedAmount = DisbursementData.Amount;
                loanDisbursement.DisbursementAmount = DisbursementData.DisbursementAmount;
                loanDisbursement.LoanApplicationId = DisbursementData.LoanApplicationId;
                db.LoanDisbursement.Add(loanDisbursement);
                db.Save();
                if (loanDisbursement.Id > 0)
                {
                    return Json("success");
                }
            }
            return Json("Error");
        }

        [HttpPost]
        public IActionResult LoadLoanDisbursementTableData()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var AllLoans = (from loanDisbursement in db.LoanDisbursement.GetAll()
                         join loanApplication in db.LoanApplication.GetAll() on loanDisbursement.LoanApplicationId equals loanApplication.Id into Mixed
                         from finalLoanApplication in Mixed.DefaultIfEmpty()
                         join employee in db.Employee.GetAll() on finalLoanApplication.ApplierId equals employee.Id into EmployeeMixed
                         from finalEmployee in EmployeeMixed.DefaultIfEmpty()
                         select new
                         {
                             finalEmployee.FullName,
                             loanDisbursement.DisbursementAmount,
                             loanDisbursement.ApprovedAmount,
                             loanDisbursement.Id,
                             loanDisbursement.LoanApplicationId,
                             finalLoanApplication.Reason,
                             finalLoanApplication.ApplierId,
                         }).ToList();



            //var AllLoans = db.Loan.GetAll().Where(d => d.IsActive == true && d.IsDeleted == false);
            List<vmLoan> LoanList = new List<vmLoan>();

            //Sorting
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                // AllLoans = AllLoans.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                AllLoans = AllLoans.OrderByDescending(model => model.Id).ToList();
            }

            //Search
            if (!string.IsNullOrEmpty(searchValue))
            {
                AllLoans = AllLoans.Where(model => model.FullName.Contains(searchValue) || model.Reason.Contains(searchValue) ||
                           model.ApprovedAmount == Convert.ToInt32(searchValue) || model.DisbursementAmount == Convert.ToInt32(searchValue)).ToList();

            }

            
            //total number of rows count     
            recordsTotal = AllLoans.Count();

            //Paging     
            var data = AllLoans.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        public IActionResult DeleteLoanDisbursement( int Id)
        {
            var result = db.LoanDisbursement.Get(Id);
            if( result != null)
            {
                db.LoanDisbursement.Remove(result);
                db.Save();
                return Json(true);
            }
            return Json(false);
        }

        private IActionResult json(bool v)
        {
            throw new NotImplementedException();
        }

        //end Loan Disbursement Operations
    }
}