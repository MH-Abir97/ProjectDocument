using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.ManualAttendance;
using Pronali.Web.Controllers;
using Pronali.Web.Helper;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    public class ManualAttendanceController : BaseController
    {
        private readonly IImagePath _imagePath;
        public ManualAttendanceController(IUnitOfWork _unitOfWork, IImagePath imagePath) : base(_unitOfWork)
        {
            _imagePath = imagePath;
        }
        public IActionResult Manage()
        {

            return View();
        }
        public IActionResult GetEmployeeList()
        {
            var result = db.Employee.GetAll().ToList();

            return Json(result);
        }
        [HttpGet]
        public IActionResult CreateView()
        {
            //ViewBag.itemsTime = DateTime.Now.ToShortDateString();
            vmManualAttendance manualAttendance = new vmManualAttendance();
            return PartialView("MAttendanceCreate", manualAttendance);
        }
        [HttpPost]
        public IActionResult Create(vmManualAttendance manualAttendance)
        {

            if (ModelState.IsValid)
            {
                ManualAttendance attendance = new ManualAttendance()
                {
                    EmployeeId = manualAttendance.EmployeeId,
                    InTime = manualAttendance.InTime,
                    OutTime = manualAttendance.OutTime,
                    Remarks = manualAttendance.Remarks,
                    ApproverId = manualAttendance.ApproverId
                };
                db.ManualAttendance.Add(attendance);
                db.Save();
                ModelState.Clear();

                if (attendance.Id > 0)
                {
                    return Json(true);
                }

            }
            return Json(false);
        }
        [HttpPost]
        public IActionResult Edit(vmManualAttendance modelData)
        {
            if (ModelState.IsValid)
            {
                ManualAttendance head = db.ManualAttendance.GetFirstOrDefault(c => c.Id == modelData.Id);
                head.EmployeeId = modelData.EmployeeId;
                head.InTime = modelData.InTime;
                head.OutTime = modelData.OutTime;
                head.Remarks = modelData.Remarks;
                head.ApproverId = modelData.ApproverId;
                db.ManualAttendance.Update(head);
                db.Save();
                if (head.Id > 0)
                {
                    return Json(true);
                }

            }
            return Json(false);
        }

        public IActionResult Delete(long id)
        {
            var head = db.ManualAttendance.Get(id);
            db.ManualAttendance.Remove(head);
            db.Save();

            return Json(true);
        }
        public IActionResult LoadManualAttendances()
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

            List<ManualAttendance> attendances = db.ManualAttendance.GetAllWithRelatedData();

            var ManualAttendanceList = new List<vmManualAttendance>();

            //Sorting    
            //if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            //{
            //    branches = branches.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            //}
            //else
            //{
            //    branches = branches.OrderByDescending(x => x.Id).ToList();
            //}

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                //branches = branches.Where(x => x.Id.Contains(searchValue)).ToList();
            }

            foreach (var item in attendances)
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
                ManualAttendanceList.Add(new vmManualAttendance
                {
                    Id = item.Id,
                    EmployeeName = item.Employee == null ? string.Empty : item.Employee.FullName,
                    InTime = item.InTime,
                    OutTime = item.OutTime,
                    Remarks = item.Remarks,
                    Approver = item.Approver == null ? string.Empty : item.Employee.FullName,
                    ApprovedTime = item.ApprovedTime,
                    PhotoUrl = photoURL,
                    EmployeeId = item.EmployeeId,
                    //DateInTime = item.InTime.ToShortDateString(),
                    //DateOutTime = item.OutTime.ToShortDateString(),
                });
            }
            //total number of rows count     
            recordsTotal = ManualAttendanceList.Count();

            //Paging     
            var data = ManualAttendanceList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}