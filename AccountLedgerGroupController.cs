using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Web.Areas.POS.Helper;
using Pronali.Web.Areas.POS.Models.DatatableModels;
using System.Linq.Dynamic.Core;
using Pronali.Web.Controllers;

namespace Pronali.Web.Areas.POS.Controllers
{
    [Area("POS")]
    public class AccountLedgerGroupController : BaseController
    {
        private readonly IUnitOfWork _work;

        public AccountLedgerGroupController(IUnitOfWork work) : base(work)
        {
            _work = work;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateView()
        {
            ViewData["AccountLedgers"] = POSHelper.GetAccountLedgerSelectItems(_work.AccountLedger);
            return PartialView("_AccountLedgerGroupCreateView");
        }

        public IActionResult Create(AccountLedgerGroup accountLedgerGroup)
        {
            if (ModelState.IsValid)
            {
                _work.AccountLedgerGroup.Add(accountLedgerGroup);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }
            }

            return Json(false);
        }

        public IActionResult EditView(int accountLedgerGroupId)
        {
            var accountLedgerGroup = _work.AccountLedgerGroup.Get(accountLedgerGroupId);
            ViewData["AccountLedgers"] = POSHelper.GetAccountLedgerSelectItems(_work.AccountLedger);
            return PartialView("_AccountLedgerGroupEditView", accountLedgerGroup);
        }

        public IActionResult Edit(AccountLedgerGroup ledgerGroup)
        {
            if (ModelState.IsValid)
            {
                var accountLedgerGroup = _work.AccountLedgerGroup.Get(ledgerGroup.Id);

                accountLedgerGroup.AccountLedgerGroupName = ledgerGroup.AccountLedgerGroupName;

                _work.AccountLedgerGroup.Update(accountLedgerGroup);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }
            }
            return PartialView("_AccountLedgerGroupEditView");
        }

        public IActionResult Delete(int accountLedgerGroupId)
        {
            var accountLedgerGroup = _work.AccountLedgerGroup.Get(accountLedgerGroupId);

            _work.AccountLedgerGroup.Remove(accountLedgerGroup);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        public IActionResult AccountLedgerGroupList()
        {
            return PartialView("_AccountLedgerGroupList");
        }

        public IActionResult LoadAccountLedgerGroups()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault().ToLower();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var accountLedgerGroups = _work.AccountLedgerGroup.GetAllWithType();

            var accountLedgerGroupList = new List<AccountLedgerGroupVM>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                accountLedgerGroups = accountLedgerGroups.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                accountLedgerGroups = accountLedgerGroups.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                accountLedgerGroups = accountLedgerGroups.Where(x => x.AccountLedgerGroupName.ToLower().Contains(searchValue) || x.AccountLedger.AccountLedgerName.ToLower().Contains(searchValue)).ToList();
            }

            foreach (var item in accountLedgerGroups)
            {
                accountLedgerGroupList.Add(new AccountLedgerGroupVM
                {
                    Id = item.Id,
                    AccountLedgerGroupName = item.AccountLedgerGroupName,
                    AccountLedgerName = item.AccountLedger == null ? string.Empty : item.AccountLedger.AccountLedgerName,
                });
            }

            //total number of rows count     
            recordsTotal = accountLedgerGroupList.Count();

            //Paging     
            var data = accountLedgerGroupList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        public IActionResult GetAllLedgerGroupAsSelectList()
        {
            var selectList = POSHelper.GetLedgerGroupSelectItems(_work.AccountLedgerGroup);

            return Json(selectList);
        }
    }
}