using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.EmployeeWeekend;
using Pronali.Web.Controllers;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    public class EmployeeWeekendController : BaseController
    {
        public EmployeeWeekendController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
        }

        public IActionResult GetDropDownList()
        {
            var result = db.Employee.GetAll().ToList();
            return Json(result);
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateEmployeeWeekend(vmEmployeeWeekend employeeWeekend)
        {
            if (ModelState.IsValid)
            {
                EmployeeWeekend employee = new EmployeeWeekend()
                {
                    EmployeeId = employeeWeekend.EmployeeId,
                    Dayname = employeeWeekend.Dayname
                };
                db.EmployeeWeekend.Add(employee);
                bool isSave=db.Save()>0;

                if (isSave)
                {
                    return Json(true);
                }
            }
            return Json(false);
        }

        [HttpPost]
        public IActionResult UpdateEmployeeWeekend(vmEmployeeWeekend employeeWeekend)
        {
            var weekendObj = db.EmployeeWeekend.Get(employeeWeekend.Id);
            if (weekendObj !=null)
            {
                weekendObj.EmployeeId = employeeWeekend.EmployeeId;
                weekendObj.Dayname = employeeWeekend.Dayname;
            }
            db.EmployeeWeekend.Update(weekendObj);
            bool weekendUpdate = db.Save() > 0;

            if (weekendUpdate)
            {
                return Json(true);
            }
            return Json(false);
        }


        public IActionResult Delete(long id)
        {

            var weekend = db.EmployeeWeekend.Get(id);
            if (weekend !=null)
            {
                db.EmployeeWeekend.Remove(weekend);
                db.Save();
                return Json(true);
            }
            return Json(false);
        }
        public IActionResult LoadEmployeeWeekend()
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

            List<EmployeeWeekend> employeeList = db.EmployeeWeekend.GetAllEmployeeAndWeekend().Where(x=>x.IsActive==true && x.IsDeleted==false).ToList();
            List<vmEmployeeWeekend> weekendList = new List<vmEmployeeWeekend>();
            foreach (var item in employeeList)
            {
                weekendList.Add(new vmEmployeeWeekend
                {
                    Id=item.Id,
                    EmployeeId=item.EmployeeId,
                    Dayname=item.Dayname,
                    Name=item.Employee.FullName,
                });
            }

            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                // AllLoans = AllLoans.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                weekendList = weekendList.OrderByDescending(model => model.Id).ToList();
            }

            //Search
            if (!string.IsNullOrEmpty(searchValue))
            {
                weekendList = weekendList.Where(model => model.Dayname.Contains(searchValue)).ToList();

            }


            //total number of rows count     
            recordsTotal = weekendList.Count();

            //Paging     
            var data = weekendList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}