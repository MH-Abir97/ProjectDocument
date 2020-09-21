using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Enum;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models;
using Pronali.Web.Controllers;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    public class SalaryHeldupController : BaseController
    {

        public SalaryHeldupController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
           
        }
        public IActionResult Index()
        {
            ViewBag.itemsTime = DateTime.Now.ToShortDateString();
            return View();
        }

       public IActionResult GetEmployee()
        {
            var result = db.Employee.GetAll().ToList();

            return Json(result);
        }

        public IActionResult CreateSalleryHeldUp(vmSallaryHeldup VmSalleryheldup)
        {
            if (ModelState.IsValid)
            {
                SalaryHeldup sallary = new SalaryHeldup()
                {
                    EmployeeId = VmSalleryheldup.EmployeeId,
                    FromDate = VmSalleryheldup.FromDate,
                    ToDate = VmSalleryheldup.ToDate !=null ? VmSalleryheldup.ToDate:DateTime.Now,
                    Reason = VmSalleryheldup.Reason,
                    Status = ApplicationStatus.Pending
                };
                db.SalaryHeldup.Add(sallary);
                bool isSave = db.Save() > 0;
                if (isSave)
                {
                    return Json(true);
                }
            }
            return Json(false);
        }

        public IActionResult UpdateSalleryHeldup(vmSallaryHeldup VmSalleryheldup)
        {
            var salleryheldup = db.SalaryHeldup.Get(VmSalleryheldup.Id);
            if (salleryheldup !=null)
            {
                salleryheldup.EmployeeId = VmSalleryheldup.EmployeeId;
                salleryheldup.FromDate = VmSalleryheldup.FromDate;
                salleryheldup.ToDate = VmSalleryheldup.ToDate != null ? VmSalleryheldup.ToDate : DateTime.Now;
                salleryheldup.Reason = VmSalleryheldup.Reason;
                salleryheldup.Status= ApplicationStatus.Pending;
                db.SalaryHeldup.Update(salleryheldup);
                bool isUpdate = db.Save() > 0;
                if (isUpdate)
                {
                    return Json(true);
                }

            }
            return Json(false);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var sallery = db.SalaryHeldup.Get(id);

            if (sallery !=null)
            {
                db.SalaryHeldup.Remove(sallery);
                db.Save();
                return Json(true);
            }

            return Json(true);
        }

        public IActionResult LoadHeldup()
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

            List<SalaryHeldup> suspendedList = db.SalaryHeldup.GetAllWithSallaryHeldUp().Where(Model => Model.IsActive == true && Model.IsDeleted == false).ToList();
            List<vmSallaryHeldup> suspendedItem = new List<vmSallaryHeldup>();
            foreach (var item in suspendedList)
            {
                suspendedItem.Add(new vmSallaryHeldup
                {
                    Id = item.Id,
                    EmployeeId = item.EmployeeId,
                    FromDate = item.FromDate,
                    ToDate = item.ToDate,
                    Reason = item.Reason,
                    Status = item.Status,
                    EmployeeName = item.Employee.FullName,
                    Approveby = item.Approveby,
                    ApproveDate = item.ApproveDate,
                   ToDateChange=item.ToDate !=null ? item.ToDate.Value.ToString("dd MMMM, yyyy") :null,
                    FromDateChange = item.FromDate.ToString("dd MMMM, yyyy"),
                   
                });
            }

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                // AllLoans = AllLoans.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                suspendedItem = suspendedItem.OrderByDescending(model => model.Id).ToList();
            }

            //Search
            if (!string.IsNullOrEmpty(searchValue))
            {
                suspendedItem = suspendedItem.Where(model => model.FromDate.ToShortDateString().Contains(searchValue)).ToList();

            }


            //total number of rows count     
            recordsTotal = suspendedItem.Count();

            //Paging     
            var data = suspendedItem.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

    }
}