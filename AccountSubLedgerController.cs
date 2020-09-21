using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Web.Areas.POS.Helper;
using Pronali.Web.Areas.POS.Models.DatatableModels;
using Pronali.Web.Controllers;
using System.Linq.Dynamic.Core;

namespace Pronali.Web.Areas.POS.Controllers
{
    [Area("POS")]
    public class AccountSubLedgerController : BaseController
    {
        private readonly IUnitOfWork _work;

        public AccountSubLedgerController(IUnitOfWork work) : base(work)
        {
            _work = work;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region CRUD

        public IActionResult CreateView()
        {
            ViewData["AccountGroups"] = POSHelper.GetAccountGroupSelectItems(_work.AccountGroup);
            ViewData["AccountLedgers"] = POSHelper.GetAccountLedgerSelectItems(_work.AccountLedger);
            return PartialView("_AccountSubLedgerCreateView");
        }

        public IActionResult Create(AccountSubLedger accountSubLedger)
        {
            accountSubLedger.AccountLedgerGroupId = accountSubLedger.AccountLedgerGroupId == 0 ? null : accountSubLedger.AccountLedgerGroupId;

            if (ModelState.IsValid)
            {
                var accountLedger = _work.AccountLedger.Get(accountSubLedger.AccountLedgerId);

                accountSubLedger.DbTrackId = accountLedger.TrackingId;

                _work.AccountSubLedger.Add(accountSubLedger);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }
            }

            return Json(false);
        }

        public IActionResult EditView(int accountSubLedgerId)
        {
            var accountSubLedger = _work.AccountSubLedger.Get(accountSubLedgerId);
            ViewData["AccountGroups"] = POSHelper.GetAccountGroupSelectItems(_work.AccountGroup);
            ViewData["AccountLedgers"] = POSHelper.GetAccountLedgerSelectItems(_work.AccountLedger);
            return PartialView("_AccountSubLedgerEditView", accountSubLedger);
        }

        public IActionResult Edit(AccountSubLedger SubLedger)
        {
            SubLedger.AccountLedgerGroupId = SubLedger.AccountLedgerGroupId == 0 ? null : SubLedger.AccountLedgerGroupId;

            if (ModelState.IsValid)
            {
                var accountSubLedger = _work.AccountSubLedger.Get(SubLedger.Id);

                accountSubLedger.AccountSubLedgerName = SubLedger.AccountSubLedgerName;

                _work.AccountSubLedger.Update(accountSubLedger);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }
            }
            return PartialView("_AccountSubLedgerEditView");
        }

        public IActionResult Delete(int accountSubLedgerId)
        {
            var accountSubLedger = _work.AccountSubLedger.Get(accountSubLedgerId);

            _work.AccountSubLedger.Remove(accountSubLedger);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        public IActionResult AccountSubLedgerList()
        {
            return PartialView("_AccountSubLedgerList");
        }

        public IActionResult LoadAccountSubLedgers()
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

            var accountSubLedgers = _work.AccountSubLedger.GetAllWithType();

            var accountSubLedgerList = new List<AccountSubLedgerVM>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                accountSubLedgers = accountSubLedgers.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                accountSubLedgers = accountSubLedgers.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                accountSubLedgers = accountSubLedgers.Where(x => x.AccountSubLedgerName.ToLower().Contains(searchValue) || x.AccountLedger.AccountLedgerName.ToLower().Contains(searchValue)).ToList();
            }

            foreach (var item in accountSubLedgers)
            {
                accountSubLedgerList.Add(new AccountSubLedgerVM
                {
                    Id = item.Id,
                    AccountSubLedgerName = item.AccountSubLedgerName,
                    AccountLedgerGroupName = item.AccountLedgerGroup == null ? string.Empty : item.AccountLedgerGroup.AccountLedgerGroupName,
                    AccountLedgerName = item.AccountLedger == null ? string.Empty : item.AccountLedger.AccountLedgerName,
                });
            }

            //total number of rows count     
            recordsTotal = accountSubLedgerList.Count();

            //Paging     
            var data = accountSubLedgerList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        #endregion

        public JsonResult GetSubledgerAsSelectList()
        {
            var subLedgers = POSHelper.GetSubLedgerSelectItems(_work.AccountSubLedger);
            return Json(subLedgers);
        }
    }
}