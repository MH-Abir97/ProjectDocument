using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.Holiday;
using Pronali.Web.Controllers;
using Pronali.Data.Enum;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class HolidayController : BaseController
    {
        public HolidayController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
        }
        public IActionResult LoadHoliday()
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

            var holiday = db.Holiday.GetAll().Where(d => d.IsActive == true && d.IsDeleted == false);

            var holidayList = new List<vmHoliday>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                holiday = holiday.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                holiday = holiday.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                holiday = holiday.Where(x => x.Name.Contains(searchValue)).ToList();
            }

            foreach (var item in holiday)
            {
                holidayList.Add(new vmHoliday
                {
                    Id = item.Id,
                    Name = item.Name,
                    To = item.To.ToString("dd MMMM, yyyy"),
                    From = item.From.ToString("dd MMMM, yyyy"),
                    sFlag = ((DayFlag)item.Flag).ToString()
                });
            }

            //total number of rows count     
            recordsTotal = holidayList.Count();

            //Paging     
            var data = holidayList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


        public IActionResult CreateHoliday()
        {
            vmHoliday vmholiday = new vmHoliday();
            return PartialView("CreateHoliday", vmholiday);
        }

        [HttpPost]
        public IActionResult CreateHoliday(vmHoliday vmHoliday)
        {
            if (ModelState.IsValid)
            {
                //DayFlag flag = (DayFlag)Enum.Parse(typeof(DayFlag), vmHoliday.Flag);
                
                Holiday holiday = new Holiday
                {
                    Name = vmHoliday.Name,
                    From = Convert.ToDateTime(vmHoliday.From),
                    To = Convert.ToDateTime(vmHoliday.To),
                    Flag = (DayFlag)vmHoliday.Flag
                };
                db.Holiday.Add(holiday);
                db.Save();
                return PartialView("CreateHoliday");
            }
            return PartialView("CreateHoliday");
        }

        //Edit
        public IActionResult Edit(int id)
        {
            Holiday holiday = db.Holiday.GetFirstOrDefault(h => h.Id == id);
            vmHoliday vmholiday = new vmHoliday()
            {
                Id = holiday.Id,
                Name = holiday.Name,
                From = holiday.From.ToString("dd MMMM, yyyy"),
                To = holiday.To.ToString("dd MMMM, yyyy"),
                Flag = Convert.ToInt32(holiday.Flag)
            };
            return PartialView("_Edit", vmholiday);
        }

        [HttpPost]
        public IActionResult Edit(vmHoliday vmHoliday)
        {
            if (ModelState.IsValid)
            {
                Holiday holiday = db.Holiday.GetFirstOrDefault(h => h.Id == vmHoliday.Id);

                string flag = ((DayFlag)vmHoliday.Flag).ToString();
                
                holiday.Name = vmHoliday.Name;
                holiday.From = Convert.ToDateTime(vmHoliday.From);
                holiday.To = Convert.ToDateTime(vmHoliday.To);
                holiday.Flag = (DayFlag)Enum.Parse(typeof(DayFlag), flag);

                db.Holiday.Update(holiday);
                db.Save();
                return PartialView("_Edit");
            }
            return PartialView("_Edit");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Holiday holiday = db.Holiday.GetFirstOrDefault(c => c.Id == id);

            holiday.IsActive = false;
            holiday.IsDeleted = true;
            db.Holiday.Update(holiday);
            db.Save();
            //_db.Designation.Remove(designation);
            return Json(holiday);
        }
    }
}