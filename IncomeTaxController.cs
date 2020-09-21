using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Enum;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.IncomeTax;
using Pronali.Web.Controllers;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class IncomeTaxController : BaseController
    {
        public IncomeTaxController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
        }

        public IActionResult Index()
        {
            return View("Index");
        }
        
        public IActionResult Create()
        {
            return PartialView("_Create");
        }

        [HttpPost]
        public IActionResult Create(vmEmployeeIncomeTax vmEmployeeIncomeTax)
        {
            if(ModelState.IsValid)
            {
                EmployeeIncomeTax employeeIncomeTax = new EmployeeIncomeTax()
                {
                    SalaryGroup = vmEmployeeIncomeTax.SalaryGroup,
                    SalaryDown = Convert.ToDecimal(vmEmployeeIncomeTax.SalaryDown),
                    SalaryUp = Convert.ToDecimal(vmEmployeeIncomeTax.SalaryUp),
                    Percentage = vmEmployeeIncomeTax.Percentage
                };
                db.EmployeeIncomeTax.Add(employeeIncomeTax);
                db.Save();
            }
            return View("Index");
        }

        public IActionResult Edit(long id)
        {
            EmployeeIncomeTax employeeIncomeTax = db.EmployeeIncomeTax.GetFirstOrDefault(i => i.Id == id);
            vmEmployeeIncomeTax vmEmployeeIncomeTax = new vmEmployeeIncomeTax()
            {
                Id = employeeIncomeTax.Id,
                SalaryUp = employeeIncomeTax.SalaryUp,
                SalaryDown = employeeIncomeTax.SalaryDown,
                SalaryGroup = employeeIncomeTax.SalaryGroup,
                Percentage = employeeIncomeTax.Percentage
            };
            return PartialView("_Edit", vmEmployeeIncomeTax);
        }

        [HttpPost]
        public IActionResult Edit(vmEmployeeIncomeTax vmEmployeeIncomeTax)
        {
            if (ModelState.IsValid)
            {
                EmployeeIncomeTax employeeIncomeTax = db.EmployeeIncomeTax.GetFirstOrDefault(i => i.Id == vmEmployeeIncomeTax.Id);

                employeeIncomeTax.SalaryDown = vmEmployeeIncomeTax.SalaryDown;
                employeeIncomeTax.SalaryGroup = vmEmployeeIncomeTax.SalaryGroup;
                employeeIncomeTax.SalaryUp = vmEmployeeIncomeTax.SalaryUp;
                employeeIncomeTax.Percentage = vmEmployeeIncomeTax.Percentage;

                db.EmployeeIncomeTax.Update(employeeIncomeTax);
                db.Save();
            }
            return PartialView("_Edit");
        }

        [HttpPost]
        public IActionResult Delete(long id)
        {
            EmployeeIncomeTax employeeIncomeTax = db.EmployeeIncomeTax.GetFirstOrDefault(i => i.Id == id);
            employeeIncomeTax.IsActive = false;
            employeeIncomeTax.IsDeleted = true;
            db.EmployeeIncomeTax.Update(employeeIncomeTax);
            db.Save();
            return Json("Success!");
        }


        public IActionResult LoadIncomeTax()
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

            List<EmployeeIncomeTax> incomeTax = db.EmployeeIncomeTax.GetAll().Where(d => d.IsActive == true && d.IsDeleted == false).ToList();


            var incomeTaxList = new List<vmEmployeeIncomeTax>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                incomeTax = incomeTax.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                incomeTax = incomeTax.OrderBy(x => x.Id).ToList();
            }

            //Search
            //if (!string.IsNullOrEmpty(searchValue))
            //{
            //    incomeTax = incomeTax.Where(x => x.Name.Contains(searchValue)).ToList();
            //}

            foreach (var item in incomeTax)
            {
                IncrementType flag = (IncrementType)Enum.Parse(typeof(IncrementType), item.SalaryGroup);
                incomeTaxList.Add(new vmEmployeeIncomeTax
                {
                    Id = item.Id,
                    SalaryDown = item.SalaryDown,
                    SalaryUp = item.SalaryUp,
                    Percentage = item.Percentage,
                    CreatedDate = item.CreatedDate,
                    //SalaryGroup = item.SalaryGroup,
                    SalaryGroup = ((IncrementType)flag).ToString()
                });
            }

            incomeTaxList = incomeTaxList.OrderByDescending(i => i.CreatedDate.Date)
                .ThenByDescending(i => i.CreatedDate.TimeOfDay).ToList();
            //total number of rows count     
            recordsTotal = incomeTaxList.Count();

            //Paging     
            var data = incomeTaxList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

    }
}