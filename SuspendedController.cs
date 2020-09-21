using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Enum;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.Suspended;
using Pronali.Web.Controllers;
using Pronali.Web.Extension;
using Pronali.Web.Helper;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
 
    public class SuspendedController : BaseController
    {

        private readonly IImagePath _imagePath;

        public SuspendedController(IUnitOfWork _unitOfWork, IImagePath imagePath) : base(_unitOfWork)
        {
            _imagePath = imagePath;
        }
        public IActionResult Index()
        {
            ViewBag.itemsTime = DateTime.Now.ToShortDateString();
            return View();
        }

       public IActionResult GetEmployeeList()
       {
            var result = db.Employee.GetAll().ToList();

            return Json(result);
        }

        public IActionResult SuspendedCreate(vmSuspended VmSuspended)
        {
            if (ModelState.IsValid)
            {
                Suspended suspended = new Suspended()
                {
                    EmployeeId = VmSuspended.EmployeeId,
                    FromDate = VmSuspended.FromDate,
                    ToDate = VmSuspended.ToDate != null ? VmSuspended.ToDate : DateTime.Now,
                   
                    Reason = VmSuspended.Reason,
                    Status=ApplicationStatus.Pending

                };

                db.Suspended.Add(suspended);
                db.Save();

                return Json(true);
            }

            return Json(false);
        }

        public IActionResult UpdateSuspended(vmSuspended VmSuspended)
        {
            var updaetSuspendedObj = db.Suspended.Get(VmSuspended.Id);

            if (updaetSuspendedObj !=null)
            {
                updaetSuspendedObj.EmployeeId = VmSuspended.EmployeeId;
                updaetSuspendedObj.FromDate = VmSuspended.FromDate;
                updaetSuspendedObj.ToDate = VmSuspended.ToDate != null ? VmSuspended.ToDate : DateTime.Now;
                updaetSuspendedObj.Reason = VmSuspended.Reason;
                updaetSuspendedObj.Status = ApplicationStatus.Pending;
               // db.Suspended.Update(updaetSuspendedObj);
                db.Save();
                return Json(true);
            }

            return Json(false);
          
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var suspended = db.Suspended.Get(id);


            if (suspended !=null)
            {
                suspended.IsDeleted = true;
                suspended.IsActive = false;
                db.Suspended.Remove(suspended);
                db.Save();
                return Json(true);
            }

       
            return Json(false);
        }


        public IActionResult LoadSuspended()
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

            List<Suspended> suspendedList = db.Suspended.GetAllWithData().Where(Model=>Model.IsActive==true && Model.IsDeleted==false).ToList();
            List<vmSuspended> suspendedItem = new List<vmSuspended>();
            foreach (var item in suspendedList)
            {
                suspendedItem.Add(new vmSuspended
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
                    FromDateChange = item.FromDate.ToString("dd MMMM, yyyy"),
                    ToDateChange =item.ToDate != null ? item.ToDate.Value.ToString("dd MMMM, yyyy") : null,
                   
                }) ;
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