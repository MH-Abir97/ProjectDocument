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

namespace Pronali.Web.Areas.POS.Controllers
{
    [Area("POS")]
    public class AccountGroupController : BaseController
    {
        private readonly IUnitOfWork _work;

        public AccountGroupController(IUnitOfWork work) : base(work)
        {
            _work = work;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateView()
        {
            ViewData["AccountTypes"] = POSHelper.GetAccountTypeSelectItems(_work.AccountType);
            return PartialView("_AccountGroupCreateView");
        }

        public IActionResult Create(AccountGroup accountGroup)
        {
            if (ModelState.IsValid)
            {
                _work.AccountGroup.Add(accountGroup);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }
            }

            return Json(false);
        }

        public IActionResult EditView(int accountGroupId)
        {
            var accountGroup = _work.AccountGroup.Get(accountGroupId);
            ViewData["AccountTypes"] = POSHelper.GetAccountTypeSelectItems(_work.AccountType);
            return PartialView("_AccountGroupEditView", accountGroup);
        }

        public IActionResult Edit(AccountGroup group)
        {
            if (ModelState.IsValid)
            {
                var accountGroup = _work.AccountGroup.Get(group.Id);

                accountGroup.AccountGroupName = group.AccountGroupName;
                accountGroup.TrackingId = group.TrackingId;

                _work.AccountGroup.Update(accountGroup);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }
            }
            return PartialView("_AccountGroupEditView");
        }

        public IActionResult Delete(int accountGroupId)
        {
            var accountGroup = _work.AccountGroup.Get(accountGroupId);

            _work.AccountGroup.Remove(accountGroup);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        public IActionResult AccountGroupList()
        {
            return PartialView("_AccountGroupList");
        }

        public IActionResult LoadAccountGroups()
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

            var accountGroups = _work.AccountGroup.GetAllWithType();

            var accountGroupList = new List<AccountGroupVM>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                accountGroups = accountGroups.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                accountGroups = accountGroups.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                accountGroups = accountGroups.Where(x => x.AccountGroupName.ToLower().Contains(searchValue) || x.TrackingId.ToLower().Contains(searchValue) || x.AccountType.AccountTypeName.ToLower().Contains(searchValue)).ToList();
            }

            foreach (var item in accountGroups)
            {
                accountGroupList.Add(new AccountGroupVM
                {
                    Id = item.Id,
                    AccountGroupName = item.AccountGroupName,
                    AccountTypeName = item.AccountType == null ? string.Empty : item.AccountType.AccountTypeName,
                    TrackingId = item.TrackingId,
                });
            }

            //total number of rows count     
            recordsTotal = accountGroupList.Count();

            //Paging     
            var data = accountGroupList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}