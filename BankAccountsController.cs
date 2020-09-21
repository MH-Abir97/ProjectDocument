using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Web.Controllers;
using System.Linq.Dynamic.Core;

namespace Pronali.Web.Areas.POS.Controllers
{
    [Area("POS")]
    public class BankAccountsController : BaseController
    {
        private readonly IUnitOfWork _work;
        public BankAccountsController(IUnitOfWork work) : base(work)
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
            return PartialView("_BankAccountCreateView");
        }

        [HttpPost]
        public IActionResult Create(BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                _work.BankAccount.Add(bankAccount);

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
        public IActionResult EditView(int bankAccountId)
        {

            var bankAccount = _work.BankAccount.Get(bankAccountId);
            return PartialView("_BankAccountEditView", bankAccount);
        }
        [HttpPost]
        public IActionResult Edit(BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                var bankAccount1 = _work.BankAccount.Get(bankAccount.Id);

                bankAccount1.AccountName = bankAccount.AccountName;
                bankAccount1.AccountNumber = bankAccount.AccountNumber;
                bankAccount1.BankName = bankAccount.BankName;
                bankAccount1.BranchName = bankAccount.BranchName;
                bankAccount1.AccountType = bankAccount.AccountType;
                _work.BankAccount.Update(bankAccount1);

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
        public IActionResult Delete(int bankAccountId)
        {
            var bankAccount = _work.BankAccount.Get(bankAccountId);

            _work.BankAccount.Remove(bankAccount);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        [HttpGet]
        public IActionResult BankAccountList()
        {
            return PartialView("_BankAccountList");
        }

        public IActionResult LoadBankAccounts()
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

            var bankAccounts = _work.BankAccount.GetAll();

            var bankAccountList = new List<BankAccount>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                bankAccounts = bankAccounts.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                bankAccounts = bankAccounts.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                bankAccounts = bankAccounts.Where(x => x.AccountName.Contains(searchValue)).ToList();
            }

            foreach (var item in bankAccounts)
            {
                bankAccountList.Add(new BankAccount
                {
                    Id = item.Id,
                    AccountName = item.AccountName,
                    AccountNumber = item.AccountNumber,
                    BankName = item.BankName,
                    BranchName = item.BranchName,
                    AccountType = item.AccountType,
                });
            }

            //total number of rows count     
            recordsTotal = bankAccountList.Count();

            //Paging     
            var data = bankAccountList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
        }
    }
}