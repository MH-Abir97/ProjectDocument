using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.Shift;
using Pronali.Web.Controllers;
using Pronali.Web.Helper;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class ShiftingController : BaseController
    {
        public ShiftingController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
        }
        public IActionResult Index()
        {
            return View("Index");
        }

        public IActionResult Create()
        {
            vmShift vmShift = new vmShift();
            return PartialView("Create", vmShift);
        }

        [HttpPost]
        public IActionResult Create(vmShift vmShift)
        {
            if (ModelState.IsValid)
            {
                Shift shift = new Shift
                {
                    Name = vmShift.Name,
                    LunchHr = vmShift.LunchHr,
                    IsActive = true,
                    IsDeleted = false
                };
                db.Shift.Add(shift);
                db.Save();

                foreach (var sDetails in vmShift.vmShiftDetails)
                {
                    ShiftDetails shiftDetails = new ShiftDetails {
                        DayName = sDetails.DayName,
                        Flag = sDetails.Flag,
                        CheckInTime = sDetails.CheckInTime,
                        CheckOutTime = sDetails.CheckOutTime,
                        OfficeStartTime = sDetails.OfficeStartTime,
                        OfficeEndTime = sDetails.OfficeEndTime,
                        ShiftId = shift.Id
                    };
                    db.ShiftDetails.Add(shiftDetails);
                    db.Save();
                }
                return PartialView("Create");
            }
            return PartialView("Create");
        }

        public IActionResult LoadShift()
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

            var shift = db.Shift.GetAll().Where(d => d.IsActive == true && d.IsDeleted == false);

            var shiftList = new List<vmShift>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                shift = shift.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                shift = shift.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                shift = shift.Where(x => x.Name.Contains(searchValue)).ToList();
            }

            foreach (var item in shift)
            {
                shiftList.Add(new vmShift
                {
                    Id = item.Id,
                    Name = item.Name,
                    CreatedDate = item.CreatedDate
                });
            }

            shiftList = shiftList.OrderByDescending(i => i.CreatedDate.Date)
                .ThenByDescending(i => i.CreatedDate.TimeOfDay).ToList();
            //total number of rows count     
            recordsTotal = shiftList.Count();

            //Paging     
            var data = shiftList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var shift = db.Shift.GetFirstOrDefaultWithRelatedData(s => s.Id == id);
            //var shiftDetails = db.ShiftDetails.GetAll();
            vmShift vmShift = new vmShift();
            vmShift.Id = shift.Id;
            vmShift.Name = shift.Name;
            vmShift.LunchHr = shift.LunchHr;
            if(shift.ShiftDetailsList.Count() != 0)
            {
                List<vmShiftDetails> sDetails = new List<vmShiftDetails>();
                foreach (var item in shift.ShiftDetailsList)
                {
                    vmShiftDetails vmdetails = new vmShiftDetails()
                    {
                        Id = item.Id,
                        DayName = item.DayName,
                        CheckInTime = item.CheckInTime,
                        CheckOutTime = item.CheckOutTime,
                        OfficeStartTime = item.OfficeStartTime,
                        OfficeEndTime = item.OfficeEndTime,
                        Flag = item.Flag,
                        ShiftId = item.ShiftId
                    };
                    sDetails.Add(vmdetails);
                    //vmShift.vmShiftDetails.Add(vmdetails);
                }
                ViewBag.ShiftDetails = sDetails;
                vmShift.vmShiftDetails = sDetails;
            }
            return PartialView("_Edit", vmShift);
        }

        [HttpPost]
        public IActionResult Edit(vmShift vmShift)
        {
            if (ModelState.IsValid)
            {
                Shift shift = db.Shift.GetFirstOrDefault(s => s.Id == vmShift.Id);
                shift.Name = vmShift.Name;
                shift.LunchHr = vmShift.LunchHr;
                db.Shift.Update(shift);
                db.Save();

                foreach (var items in vmShift.vmShiftDetails)
                {
                    ShiftDetails shiftDetails = db.ShiftDetails.GetFirstOrDefault(s => s.Id == items.Id);
                    shiftDetails.DayName = items.DayName;
                    shiftDetails.Flag = items.Flag;
                    shiftDetails.OfficeStartTime = items.OfficeStartTime;
                    shiftDetails.OfficeEndTime = items.OfficeEndTime;
                    shiftDetails.CheckInTime = items.CheckInTime;
                    shiftDetails.CheckOutTime = items.CheckOutTime;
                    db.ShiftDetails.Update(shiftDetails);
                    db.Save();
                }
                return Json("Success");
                //return PartialView("_Edit", vmShift);
            }
            return Json("Success");
        }

        public IActionResult Delete(long id)
        {
            Shift shift = db.Shift.GetFirstOrDefaultWithRelatedData(s => s.Id == id);
            shift.IsActive = false;
            shift.IsDeleted = true;
            db.Shift.Update(shift);
            db.Save();
            foreach (var item in shift.ShiftDetailsList)
            {
                item.IsActive = false;
                item.IsDeleted = true;
                db.ShiftDetails.Update(item);
                db.Save();
            }
            return Json("Success");
        }

        public JsonResult GetShiftDetails(int id)
        {
            //IEnumerable <ShiftDetails> shiftD = db.ShiftDetails.GetAll().ToList();
            var shiftDetails = db.Shift.GetFirstOrDefaultWithRelatedData(s => s.Id == id);

            return Json(shiftDetails);
        }
    }
}