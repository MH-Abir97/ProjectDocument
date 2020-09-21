using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class AccountLedgerController : BaseController
    {
        private readonly IUnitOfWork _work;

        public AccountLedgerController(IUnitOfWork work) : base(work)
        {
            _work = work;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateView()
        {
            ViewData["AccountGroups"] = POSHelper.GetAccountGroupSelectItems(_work.AccountGroup);
            return PartialView("_AccountLedgerCreateView");
        }
        public IActionResult Create(AccountLedger accountLedger)
        {
            if (ModelState.IsValid)
            {
                _work.AccountLedger.Add(accountLedger);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }
            }

            return Json(false);
        }

        public IActionResult EditView(int accountLedgerId)
        {
            var accountLedger = _work.AccountLedger.Get(accountLedgerId);
            ViewData["AccountGroups"] = POSHelper.GetAccountGroupSelectItems(_work.AccountGroup);
            return PartialView("_AccountLedgerEditView", accountLedger);
        }

        public IActionResult Edit(AccountLedger ledger)
        {
            if (ModelState.IsValid)
            {
                var accountLedger = _work.AccountLedger.Get(ledger.Id);

                accountLedger.AccountLedgerName = ledger.AccountLedgerName;
                accountLedger.TrackingId = ledger.TrackingId;

                _work.AccountLedger.Update(accountLedger);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }
            }
            return PartialView("_AccountLedgerEditView");
        }

        public IActionResult Delete(int accountLedgerId)
        {
            var accountLedger = _work.AccountLedger.Get(accountLedgerId);

            _work.AccountLedger.Remove(accountLedger);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        public IActionResult AccountLedgerList()
        {
            return PartialView("_AccountLedgerList");
        }

        public IActionResult LoadAccountLedgers()
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

            var accountLedgers = _work.AccountLedger.GetAllWithGroup();

            var accountLedgerList = new List<AccountLedgerVM>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                accountLedgers = accountLedgers.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                accountLedgers = accountLedgers.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                accountLedgers = accountLedgers.Where(x => x.AccountLedgerName.ToLower().Contains(searchValue) || x.TrackingId.ToLower().Contains(searchValue) || x.AccountGroup.AccountGroupName.ToLower().Contains(searchValue)).ToList();
            }

            foreach (var item in accountLedgers)
            {
                accountLedgerList.Add(new AccountLedgerVM
                {
                    Id = item.Id,
                    AccountLedgerName = item.AccountLedgerName,
                    AccountGroupName = item.AccountGroup == null ? string.Empty : item.AccountGroup.AccountGroupName,
                    TrackingId = item.TrackingId,
                });
            }

            //total number of rows count     
            recordsTotal = accountLedgerList.Count();

            //Paging     
            var data = accountLedgerList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        public JsonResult GetAllLedgerAsSelectList()
        {
            var ledegrSelectList = POSHelper.GetAccountLedgerSelectItems(_work.AccountLedger);
            return Json(ledegrSelectList);
        }
    }
}