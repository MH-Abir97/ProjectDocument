using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Enum;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.ManualAbsent;
using Pronali.Web.Controllers;
using Pronali.Web.Helper;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    public class ManualAbsentController : BaseController
    {
        private readonly IImagePath _imagePath;
        public ManualAbsentController(IUnitOfWork _unitOfWork, IImagePath imagePath) : base(_unitOfWork)
        {
            _imagePath = imagePath;
        }
        public IActionResult GetEmployeeList()
        {
            var result = db.Employee.GetAll().ToList();

            return Json(result);
        }

        [HttpGet]
        public IActionResult CreateView()
        {
            vmManualAbsent Absent = new vmManualAbsent();
            ViewBag.itemsTime = DateTime.Now.ToShortDateString();
            return PartialView("MAbsentCreate", Absent);
        }

        [HttpPost]
        public IActionResult Create(vmManualAbsent absent)
        {

            if (ModelState.IsValid)
            {
                ManualAbsent manualAbsent = new ManualAbsent()
                {
                    EmployeeId = absent.EmployeeId,
                    TransactionTime = absent.TransactionTime,
                    Reason = absent.Reason,
                    ApproverId = absent.ApproverId,
                    ApprovedTime = absent.ApprovedTime
                };
                db.ManualAbsent.Add(manualAbsent);
                var isSave = db.Save() > 0;
                //if (isSave)
                //{
                //    db.ApplicationApproval.SetApprover(ApplicationType.ManualAbsent, loggedInEmployeeId, latePermission.Id.ToString(), latePermission.CreatedDate, latePermission.CreatedBy);
                //}
                ModelState.Clear();

                if (manualAbsent.Id > 0)
                {
                    return Json(true);
                }

            }
            return Json(false);
        }
        [HttpPost]
        public IActionResult Edit(vmManualAbsent modelData)
        {
           
            //var headObj = db.BranchHead.Get(modelData.Id);
            if (ModelState.IsValid)
            {
                ManualAbsent head = db.ManualAbsent.GetFirstOrDefault(c => c.Id == modelData.Id);
                head.EmployeeId = modelData.EmployeeId;
                head.TransactionTime = modelData.TransactionTime;
                head.Reason = modelData.Reason;
                head.ApproverId = modelData.ApproverId;
                head.ApprovedTime = modelData.ApprovedTime;
                db.ManualAbsent.Update(head);
                db.Save();
                if (head.Id > 0)
                {
                    return Json(true);
                }

            }
            return Json(false);
        }
        public IActionResult Manage()
        {

            return View();
        }
        public IActionResult Delete(long id)
        {
            var head = db.ManualAbsent.Get(id);
            db.ManualAbsent.Remove(head);
            db.Save();

            return Json(true);
        }
        public IActionResult LoadManualAbsents()
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

            List<ManualAbsent> absents = db.ManualAbsent.GetAllWithRelatedData();

            var ManualAbsentList = new List<vmManualAbsent>();

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

            foreach (var item in absents)
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
                ManualAbsentList.Add(new vmManualAbsent
                {
                    Id = item.Id,                  
                    EmployeeName = item.Employee == null ? string.Empty : item.Employee.FullName,
                    TransactionTime = item.TransactionTime,
                    Reason = item.Reason,
                    Approver = item.Approver == null ? string.Empty : item.Employee.FullName,
                    ApprovedTime = item.ApprovedTime,
                    PhotoUrl = photoURL,
                    EmployeeId =item.EmployeeId,
                    DateTransactionTime = item.TransactionTime.ToShortDateString(),
                });
            }
            //total number of rows count     
            recordsTotal = ManualAbsentList.Count();

            //Paging     
            var data = ManualAbsentList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}
