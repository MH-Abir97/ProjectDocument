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
    public class SuppliersController : BaseController
    {
        private readonly IUnitOfWork _work;

        public SuppliersController(IUnitOfWork work) : base(work)
        {
            _work = work;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreateView()
        {            
            return PartialView("_SupplierCreateView");
        }
        public IActionResult Create(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _work.Supplier.Add(supplier);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }
            }

            return Json(false);
        }
        public IActionResult EditView(int supplierId)
        {
            var supplier = _work.Supplier.Get(supplierId);
            return PartialView("_SupplierEditView", supplier);
        }

        public IActionResult Edit(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                var supplier1 = _work.Supplier.Get(supplier.Id);

                supplier1.Name = supplier.Name;
                supplier1.Company = supplier.Company;
                supplier1.LandPhone = supplier.LandPhone;
                supplier1.Mobile = supplier.Mobile;
                supplier1.CommisionType = supplier.CommisionType;
                supplier1.MainBusiness = supplier.MainBusiness;
                supplier1.BusinessType = supplier.BusinessType;                
                supplier1.Email = supplier.Email;
                supplier1.Address = supplier.Address;
                supplier1.HasCommission = supplier.HasCommission;
                supplier1.OnAccount = supplier.OnAccount;
                supplier1.BillByBIll = supplier.BillByBIll;

                _work.Supplier.Update(supplier1);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }
                return Json(false);
            }
            return Json(false); 
            //return PartialView("_SupplierEditView");
        }

        public IActionResult Delete(int supplierId)
        {
            var supplier = _work.Supplier.Get(supplierId);

            _work.Supplier.Remove(supplier);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        public IActionResult SupplierList()
        {
            return PartialView("_SupplierList");
        }

        public IActionResult LoadSuppliers()
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

            var suppliers = _work.Supplier.GetAll();

            var supplierList = new List<Supplier>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                suppliers = suppliers.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                suppliers = suppliers.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                suppliers = suppliers.Where(x => x.Name.Contains(searchValue) || x.Name.Contains(searchValue) || x.Email.Contains(searchValue)).ToList();
            }

            foreach (var item in suppliers)
            {
                supplierList.Add(new Supplier
                {
                    Id = item.Id,
                    Name = item.Name,
                    Company = item.Company,
                    CommisionType = item.CommisionType, /*== null ? string.Empty : item.AccountType.AccountTypeName,*/
                    MainBusiness = item.MainBusiness,
                    BusinessType = item.BusinessType,
                    Email = item.Email,
                    Address = item.Address,
                    LandPhone = item.LandPhone,
                    Mobile = item.Mobile,
                    OnAccount = item.OnAccount,
                    BillByBIll = item.BillByBIll,
                    HasCommission = item.HasCommission,

                });
            }

            //total number of rows count     
            recordsTotal = supplierList.Count();

            //Paging     
            var data = supplierList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}