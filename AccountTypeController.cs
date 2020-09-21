using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Web.Controllers;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Pronali.Web.Areas.POS.Controllers
{
    [Area("POS")]
    public class AccountTypeController : BaseController
    {
        private readonly IUnitOfWork _work;

        public AccountTypeController(IUnitOfWork work) : base(work)
        {
            _work = work;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateView()
        {
            return PartialView("_AccountTypeCreateView");
        }
        public IActionResult Create(AccountType accountType)
        {
            if (ModelState.IsValid)
            {
                _work.AccountType.Add(accountType);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }
            }

            return Json(false);
        }

        public IActionResult EditView(int accountTypeId)
        {
            var accountType = _work.AccountType.Get(accountTypeId);
            return PartialView("_AccountTypeEditView", accountType);
        }

        public IActionResult Edit(AccountType type)
        {
            if (ModelState.IsValid)
            {
                var accountType = _work.AccountType.Get(type.Id);

                accountType.AccountTypeName = type.AccountTypeName;
                accountType.TrackingId = type.TrackingId;

                _work.AccountType.Update(accountType);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }
            }
            return PartialView("_AccountTypeCreateView");
        }

        public IActionResult Delete(int accountTypeId)
        {
            var accountType = _work.AccountType.Get(accountTypeId);

            _work.AccountType.Remove(accountType);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        public IActionResult AccountTypeList()
        {
            return PartialView("_AccountTypeList");
        }

        public IActionResult LoadAccountTypes()
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

            var accountTypes = _work.AccountType.GetAll();

            var accountTypeList = new List<AccountType>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                accountTypes = accountTypes.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                accountTypes = accountTypes.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                accountTypes = accountTypes.Where(x => x.AccountTypeName.Contains(searchValue) || x.TrackingId.Contains(searchValue)).ToList();
            }

            foreach (var item in accountTypes)
            {
                accountTypeList.Add(new AccountType
                {
                    Id = item.Id,
                    AccountTypeName = item.AccountTypeName,
                    TrackingId = item.TrackingId,
                });
            }

            //total number of rows count     
            recordsTotal = accountTypeList.Count();

            //Paging     
            var data = accountTypeList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}