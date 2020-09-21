using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pronali.Data;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models;
using Pronali.Web.Areas.HR.Models.Weekend;
using Pronali.Web.Controllers;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class WeekendController : BaseController
    {
        public WeekendController(IUnitOfWork _context) : base(_context)
        {
        }
        public IActionResult Index()
        {
            ViewBag.Employee = new SelectList( db.Employee.GetAll()
                .Where(e => e.IsActive == true && e.IsDeleted == false),"Id","FullName");
            return View("Index");
        }

        [HttpPost]
        public IActionResult Create(vmWeekend vmWeekend)
        {
            if (ModelState.IsValid)
            {
                EmployeeWeekend employeeWeekend = new EmployeeWeekend()
                {
                    EmployeeId = vmWeekend.EmployeeId,
                    Dayname = vmWeekend.Dayname
                };
                db.EmployeeWeekend.Add(employeeWeekend);
                db.Save();
            }
            return View("Index");
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Employee = new SelectList(db.Employee.GetAll()
                .Where(e => e.IsActive == true && e.IsDeleted == false), "Id", "FullName");
            EmployeeWeekend employeeWeekend = db.EmployeeWeekend.GetFirstOrDefault(w => w.Id == id);
            vmWeekend vmWeekend = new vmWeekend
            {
                Id = employeeWeekend.Id,
                Employee = employeeWeekend.Employee,
                EmployeeId = employeeWeekend.EmployeeId,
                Dayname = employeeWeekend.Dayname
            };
            return View("Edit", vmWeekend);
        }

        [HttpPost]
        public IActionResult Edit(vmWeekend vmWeekend)
        {
            if (ModelState.IsValid)
            {
                EmployeeWeekend employeeWeekend = db.EmployeeWeekend.GetFirstOrDefault(w => w.Id == vmWeekend.Id);
                employeeWeekend.EmployeeId = vmWeekend.EmployeeId;
                employeeWeekend.Dayname = vmWeekend.Dayname;

                db.EmployeeWeekend.Update(employeeWeekend);
                db.Save();
            }
            return View("Edit");
        }

        public IActionResult Delete(long id)
        {
            EmployeeWeekend employeeWeekend = db.EmployeeWeekend.GetFirstOrDefault(w => w.Id == id);
            employeeWeekend.IsActive = false;
            employeeWeekend.IsDeleted = true;
            db.EmployeeWeekend.Add(employeeWeekend);
            db.Save();
            return View("Index");
        }


        public IActionResult Loadweekend()
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

            var weekends = db.EmployeeWeekend.GetAllWithRelatedData(d => d.IsActive == true && d.IsDeleted == false);

            var weekendList = new List<vmWeekend>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                weekends = weekends.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                weekends = weekends.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            //if (!string.IsNullOrEmpty(searchValue))
            //{
            //    weekends = weekends.Where(x => x.Name.Contains(searchValue)).ToList();
            //}

            foreach (var item in weekends)
            {
                weekendList.Add(new vmWeekend
                {
                    Id = item.Id,
                    Employee = item.Employee,
                    EmployeeId = item.EmployeeId,
                    Dayname = item.Dayname
                });
            }

            //total number of rows count     
            recordsTotal = weekendList.Count();

            //Paging     
            var data = weekendList.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

    }
}