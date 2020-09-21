using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.CompanyHead;
using Pronali.Web.Controllers;
using Pronali.Web.Helper;
using System.Linq.Dynamic.Core;
using Pronali.Web.Areas.HR.Models.Employee;
using Pronali.Web.Areas.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    public class CompanyHeadController : BaseController
    {
        private readonly IUnitOfWork _db;
        private readonly IImagePath _imagePath;
        public CompanyHeadController(IUnitOfWork _unitOfWork, IImagePath imagePath) : base(_unitOfWork)
        {
            _db = _unitOfWork;
            _imagePath = imagePath;
        }
       

        public IActionResult GetEmployeeList()
        {
            var result = db.Employee.GetAll().ToList();

            return Json(result);
        }
        public IActionResult GetCompany()
        {
            var data = db.Company.GetAll().ToList();
            return Json(data);
        }
        //public IActionResult Index()
        //{
        //    var name = db.Employee.GetAll();

        //    List<VmEmployeeCreate> vmEmployeeList = new List<VmEmployeeCreate>();

        //    foreach (var item in name)
        //    {
        //        vmEmployeeList.Add(new VmEmployeeCreate
        //        {
        //            Id = item.Id,
        //            Combine = item.Id + " || " + item.FullName
        //        });
        //    }

        //    ViewBag.item = new SelectList(vmEmployeeList, "Id", "Combine");

        //    return View();
        //}

        [HttpGet]
        public IActionResult CreateView()
        {
            //var name = db.Employee.GetAll();

            //List<VmEmployeeCreate> vmEmployeeList = new List<VmEmployeeCreate>();

            //foreach (var item in name)
            //{
            //    vmEmployeeList.Add(new VmEmployeeCreate
            //    {
            //        Id = item.Id,
            //        Combine = item.Id + " || " + item.FullName
            //    });
            //}

            //ViewBag.item = new SelectList(vmEmployeeList, "Id", "Combine");

            //ViewBag.items = new SelectList(db.Company.GetAll(), "Id", "CompanyName");

            //ViewBag.itemsEmployee = new SelectList(db.Employee.GetAll(), "Id", "FullName");

            vmCompanyHead companyHead = new vmCompanyHead();
            return PartialView("Create", companyHead);
        }
        [HttpPost]
        public IActionResult Create(vmCompanyHead vmCompanyHead)
        {
          
            if (ModelState.IsValid)
            {
                CompanyHead companyHead1 = new CompanyHead()
                {
                    CompanyId = vmCompanyHead.CompanyId,
                    EmployeeId = vmCompanyHead.EmployeeId
                };
                db.CompanyHead.Add(companyHead1);
                db.Save();

                if (companyHead1.Id > 0)
                {
                    return Json(true);
                }

            }
            return Json(false);
        }


        //
        [HttpPost]
        public IActionResult Edit(vmCompanyHead companyHead)
        {
            if (ModelState.IsValid)
            {
                CompanyHead company = db.CompanyHead.GetFirstOrDefault(c => c.Id == companyHead.Id);
                company.CompanyId = companyHead.CompanyId;
                company.EmployeeId = companyHead.EmployeeId;

                db.CompanyHead.Update(company);
                db.Save();
                if (company.Id > 0)
                {
                    return Json(true);
                }

            }
            return Json(false);
        }
        public IActionResult Delete(int id)
        {
            var company = db.CompanyHead.Get(id);
            db.CompanyHead.Remove(company);
            db.Save() ;

            return Json(true);
        }


        public IActionResult LoadCompanyHeads()
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

            List<CompanyHead> company = db.CompanyHead.GetAllWithRelatedData();

            var companyHeadList = new List<vmCompanyHead>();

            //replace photoURL path
            foreach (var item in company)
            {
                string photoURL = "";
                if (!string.IsNullOrEmpty(item.Employee.PhotoUrl))
                {
                    photoURL = _imagePath.GetFilePathAsSourceUrl(item.Employee.PhotoUrl);
                }
                else
                {
                    photoURL = _imagePath.GetFilePathAsSourceUrl("/images/Uploads/Employee/AlterImage.png");
                }
                companyHeadList.Add(new vmCompanyHead
                {
                    Id = item.Id,
                    CompanyName = item.Company == null ? string.Empty : item.Company.CompanyName,
                    EmployeeName = item.Employee == null ? string.Empty : item.Employee.FullName,
                    PhotoUrl = photoURL,
                    //CompanyId = item.CompanyId,
                    EmployeeId = item.EmployeeId,
                });
            }

            //Sorting    
            //if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            //{
            //    company = company.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            //}
            //else
            //{
            //    company = company.OrderByDescending(x => x.Id).ToList();
            //}


            //Search
            if (!string.IsNullOrEmpty(searchValue))
            {
                //company = company.Where(x => x.CompanyName.Contains(searchValue)
                //        || (x.Employee != null && x.EmployeeName.Contains(searchValue))

                //        ).ToList();
                //company = company.Where(x => x.CompanyName.Contains(searchValue) || x.ContactPerson.Contains(searchValue)||x.Email.Contains(searchValue)|| x.Address.Contains(searchValue)).ToList();
            }

            

            //companyHeadList = companyHeadList.OrderByDescending(i => i.CreatedDate.Date)
            //    .ThenByDescending(i => i.CreatedDate.TimeOfDay).ToList();
            //total number of rows count     
            recordsTotal = companyHeadList.Count();

            //Paging     
            var data = companyHeadList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        //public JsonResult IsExist(string CompanyName)
        //{
        //    var isFound = _db.CompanyHead.GetFirstOrDefault(c => c.CompanyName == CompanyName);
        //    return Json(isFound);
        //}

    }
}