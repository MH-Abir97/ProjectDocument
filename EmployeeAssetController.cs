using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.EmployeeAsset;
using Pronali.Web.Controllers;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    public class EmployeeAssetController : BaseController
    {
        public EmployeeAssetController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
        }

        public IActionResult Index()
        {
            return View("Index");
        }

        public IActionResult Create()
        {
            ViewBag.Employee = new SelectList(db.Employee.GetAll().Where(e => e.IsActive == true && e.IsDeleted == false),"Id","FullName");
            return PartialView("_Create");
        }

        [HttpPost]
        public IActionResult Create(vmEmployeeAsset vmEmployeeAsset)
        {
            if(ModelState.IsValid)
            {
                EmployeeAsset employeeAsset = new EmployeeAsset()
                {
                    EmployeeId = vmEmployeeAsset.EmployeeId,
                    Product = vmEmployeeAsset.Product,
                    Description = vmEmployeeAsset.Description,
                    Remarks = vmEmployeeAsset.Remarks
                };
                db.EmployeeAsset.Add(employeeAsset);
                db.Save();
            }
            return PartialView("_Create");
        }


        public IActionResult Edit(long id)
        {
            ViewBag.Employee = db.Employee.GetAll().Where(e => e.IsActive == true && e.IsDeleted == false);

            EmployeeAsset employeeAsset = db.EmployeeAsset.GetFirstOrDefault(e => e.Id == id);
            vmEmployeeAsset vmEmployeeAsset = new vmEmployeeAsset()
            {
                Id = employeeAsset.Id,
                EmployeeId = employeeAsset.EmployeeId,
                Product = employeeAsset.Product,
                Description = employeeAsset.Description,
                Remarks = employeeAsset.Remarks
            };
            return View("_Edit");
        }
        [HttpPost]
        public IActionResult Edit(vmEmployeeAsset vmEmployeeAsset)
        {
            if(ModelState.IsValid)
            {
                EmployeeAsset employeeAsset = db.EmployeeAsset.GetFirstOrDefault(e => e.Id == vmEmployeeAsset.Id);

                employeeAsset.EmployeeId = vmEmployeeAsset.EmployeeId;
                employeeAsset.Product = vmEmployeeAsset.Product;
                employeeAsset.Description = vmEmployeeAsset.Description;
                employeeAsset.Remarks = vmEmployeeAsset.Remarks;

                db.EmployeeAsset.Update(employeeAsset);
                db.Save();
            }
            return PartialView("_Edit");
        }

        public IActionResult Delete(long id)
        {
            EmployeeAsset employeeAsset = db.EmployeeAsset.GetFirstOrDefault(e => e.Id == id);
            employeeAsset.IsActive = false;
            employeeAsset.IsDeleted = true;
            db.EmployeeAsset.Update(employeeAsset);
            db.Save();
            return View("Index");
        }

        public IActionResult LoadEmployeeAsset()
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

            List<EmployeeAsset> employeeAsset = db.EmployeeAsset.GetAll().Where(d => d.IsActive == true && d.IsDeleted == false).ToList();

            var employeeAssetList = new List<vmEmployeeAsset>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                employeeAsset = employeeAsset.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                employeeAsset = employeeAsset.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            //if (!string.IsNullOrEmpty(searchValue))
            //{
            //    EmployeeAsset = employeeAsset.Where(x => x.Name.Contains(searchValue)).ToList();
            //}

            foreach (var item in employeeAsset)
            {
                employeeAssetList.Add(new vmEmployeeAsset
                {
                    Id = item.Id,
                    Product = item.Product,
                    Description = item.Description,
                    Remarks = item.Remarks,
                    Employee = item.Employee,
                    CreatedDate = item.CreatedDate,
                    EmployeeId = item.EmployeeId
                });
            }

            employeeAssetList = employeeAssetList.OrderByDescending(i => i.CreatedDate.Date)
                .ThenByDescending(i => i.CreatedDate.TimeOfDay).ToList();
            //total number of rows count     
            recordsTotal = employeeAssetList.Count();

            //Paging     
            var data = employeeAssetList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

    }
}