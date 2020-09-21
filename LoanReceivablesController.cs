 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Web.Controllers;
using System.Linq.Dynamic.Core;
using Pronali.Web.Areas.POS.Helper;
using Pronali.Web.Areas.POS.Models.DatatableModels;
using Pronali.Web.Areas.POS.Models;

namespace Pronali.Web.Areas.POS.Controllers
{
    [Area("POS")]
    public class LoanReceivablesController : BaseController
    {
        private readonly IUnitOfWork _work;
        public LoanReceivablesController(IUnitOfWork work) : base(work)
        {
            _work = work;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateView()
        {
            ViewData["BankAccount"] = POSHelper.GetBankAccountSelectItems(_work.BankAccount);
            ViewData["Employee"] = POSHelper.GetEmployeeSelectItems(_work.Employee);

            return PartialView("_LoanReceivableCreateView");
        }
        [HttpPost]
        public IActionResult Create(LoanReceivableViewModel loanReceivableViewModel)
        {
            loanReceivableViewModel.EmployeeId = loanReceivableViewModel.EmployeeId == 0 ? null : loanReceivableViewModel.EmployeeId;
            loanReceivableViewModel.BankAccountId = loanReceivableViewModel.BankAccountId == 0 ? null : loanReceivableViewModel.BankAccountId;

            if (ModelState.IsValid)
            {
                var loanReceivable = new LoanReceivable
                {
                    ContactPerson = loanReceivableViewModel.CustomerName,
                    CompanyName = loanReceivableViewModel.CompanyName,
                    EmployeeId = loanReceivableViewModel.EmployeeId,
                    Address = loanReceivableViewModel.Address,
                    LandPhone = loanReceivableViewModel.LandPhone,
                    Mobile = loanReceivableViewModel.LandPhone,
                    Email = loanReceivableViewModel.Email,
                    BankAccountId = loanReceivableViewModel.BankAccountId

                };
                _work.LoanReceivable.Add(loanReceivable);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }
                return Json(false);
            }
            return Json(false);
        }

        [HttpGet]
        public IActionResult EditView(int loanReceivableId)
        {
            ViewData["BankAccount"] = POSHelper.GetBankAccountSelectItems(_work.BankAccount);
            ViewData["Employee"] = POSHelper.GetEmployeeSelectItems(_work.Employee);

            var loanReceivable = _work.LoanReceivable.GetWithBankAccountAndEmployee(loanReceivableId);
            return PartialView("_LoanReceivableEditView", loanReceivable);
        }
        [HttpPost]
        public IActionResult Edit(LoanReceivable loanReceivable)
        {
            if (ModelState.IsValid)
            {
                var loanReceivable1 = _work.LoanReceivable.GetWithBankAccountAndEmployee(loanReceivable.Id);

                loanReceivable1.ContactPerson = loanReceivable.ContactPerson;
                loanReceivable1.CompanyName = loanReceivable.CompanyName;
                loanReceivable1.EmployeeId = loanReceivable.EmployeeId;
                loanReceivable1.LandPhone = loanReceivable.LandPhone;
                loanReceivable1.LandPhone = loanReceivable.LandPhone;
                loanReceivable1.Address = loanReceivable.Address;
                loanReceivable1.LandPhone = loanReceivable.LandPhone;
                loanReceivable1.Mobile = loanReceivable.Mobile;
                loanReceivable1.Email = loanReceivable.Email;
                loanReceivable1.BankAccountId = loanReceivable.BankAccountId;
                _work.LoanReceivable.Update(loanReceivable1);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }

                return Json(false);
            }
            return Json(false);
        }
        [HttpGet]
        public IActionResult Delete(int loanReceivableId)
        {
            var loanReceivable = _work.LoanReceivable.Get(loanReceivableId);

            _work.LoanReceivable.Remove(loanReceivable);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        [HttpGet]
        public IActionResult LoanReceivableList()
        {
            return PartialView("_LoanReceivableList");
        }

        public IActionResult LoadLoanReceivables()
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

            var loanReceivables = _work.LoanReceivable.GetAllWithBankAccountAndEmployee();

            var loanReceivableList = new List<LoanReceivableVm>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                loanReceivables = loanReceivables.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                loanReceivables = loanReceivables.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                loanReceivables = loanReceivables.Where(x => x.ContactPerson.Contains(searchValue)).ToList();
            }

            foreach (var item in loanReceivables)
            {
                loanReceivableList.Add(new LoanReceivableVm
                {
                    Id = item.Id,
                    ContactPerson = item.ContactPerson,
                    CompanyName = item.CompanyName,
                    EmployeeName = item.Employee == null ? string.Empty : item.Employee.FullName,
                    Address = item.Address,
                    LandPhone = item.LandPhone,
                    Mobile = item.Mobile,
                    Email = item.Email,
                    BankAccountName = item.BankAccount == null ? string.Empty : item.BankAccount.AccountName,
                });
            }

            //total number of rows count     
            recordsTotal = loanReceivableList.Count();

            //Paging     
            var data = loanReceivableList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
        }
    }
}