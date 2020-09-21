using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Web.Controllers;
using System.Linq.Dynamic.Core;

namespace Pronali.Web.Areas.POS.Controllers
{
    [Area("POS")]
    public class VendorController : BaseController
    {
        private readonly IUnitOfWork _work;

        public VendorController(IUnitOfWork work) : base(work)
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
            return PartialView("_VendorCreateView");
        }

        [HttpPost]
        public IActionResult Create(Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                _work.Vendor.Add(vendor);
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
        public IActionResult EditView(int vendorId)
        {
            var vendor = _work.Vendor.Get(vendorId);
            return PartialView("_VendorEditView", vendor);
        }

        [HttpPost]
        public IActionResult Edit(Vendor vendor)
        {
            if (ModelState.IsValid)
            {
                var vendor1 = _work.Vendor.Get(vendor.Id);

                vendor1.CompanyName = vendor.CompanyName;
                vendor1.Name = vendor.Name;
                vendor1.MainBusiness = vendor.MainBusiness;
                vendor1.BusinessType = vendor.BusinessType;
                vendor1.LandPhone = vendor.LandPhone;
                vendor1.Mobile = vendor.Mobile;
                vendor1.Address = vendor.Address;
                vendor1.HasCommission = vendor.HasCommission;
                vendor1.OnAccount = vendor.OnAccount;
                vendor1.BillByBill = vendor.BillByBill;

                _work.Vendor.Update(vendor1);

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
        public IActionResult Delete(int vendorId)
        {
            var vendor = _work.Vendor.Get(vendorId);

            _work.Vendor.Remove(vendor);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        [HttpGet]
        public IActionResult VendorList()
        {
            return PartialView("_VendorList");
        }

        public IActionResult LoadVendors()
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

            var vendors = _work.Vendor.GetAll();

            var vendorList = new List<Vendor>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                vendors = vendors.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                vendors = vendors.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                vendors = vendors.Where(x => x.Name.Contains(searchValue)).ToList();
            }

            foreach (var item in vendors)
            {
                vendorList.Add(new Vendor
                {
                    Id = item.Id,
                    Name = item.Name,
                    CompanyName = item.CompanyName,
                    MainBusiness = item.MainBusiness,
                    BusinessType = item.BusinessType,
                    LandPhone = item.LandPhone,
                    Mobile = item.Mobile,
                    Address = item.Address,
                    HasCommission = item.HasCommission,
                    OnAccount = item.OnAccount,
                    BillByBill = item.BillByBill,
                });
            }

            //total number of rows count     
            recordsTotal = vendorList.Count();

            //Paging     
            var data = vendorList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
        }
    }
}