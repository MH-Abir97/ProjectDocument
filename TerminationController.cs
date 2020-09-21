using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Enum;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models;
using Pronali.Web.Controllers;
using Pronali.Web.Helper;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
   // [Authorize]
    public class TerminationController : BaseController
    {
        private readonly IImagePath _imagePath;

        public TerminationController(IUnitOfWork _unitOfWork, IImagePath imagePath) : base(_unitOfWork)
        {
            _imagePath = imagePath;
        }
        public IActionResult GetEmployeeList()
        {
            var result = db.Employee.GetAll().ToList();

            return Json(result);
        }
        public IActionResult Index()
        {
            ViewBag.itemsTime = DateTime.Now.ToShortDateString();
            return View();
        }
        public IActionResult TerminationCreate(vmTerminate VmTerminate)
        {
            if (ModelState.IsValid)
            {
                Terminated termination = new Terminated()
                {
                    EmployeeId = VmTerminate.EmployeeId,
                    TerminatedDate = VmTerminate.TerminatedDate,
                    Reason = VmTerminate.Reason,
                    Status = ApplicationStatus.Pending

                };

                db.Terminate.Add(termination);
                db.Save();

                return Json(true);
            }

            return Json(false);
        }
        [HttpPost]
        public IActionResult UpdateTermination(vmTerminate VmTerminate)
        {
            var updaetSuspendedObj = db.Terminate.Get(VmTerminate.Id);

            if (updaetSuspendedObj != null)
            {
                updaetSuspendedObj.EmployeeId = VmTerminate.EmployeeId;
                updaetSuspendedObj.TerminatedDate = VmTerminate.TerminatedDate;
              
                updaetSuspendedObj.Reason = VmTerminate.Reason;
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
            var terminate = db.Terminate.Get(id);


            if (terminate != null)
            {
                terminate.IsDeleted = true;
                terminate.IsActive = false;
                db.Terminate.Remove(terminate);
                db.Save();
                return Json(true);
            }


            return Json(false);
        }


        public IActionResult LoadTermination()
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

            List<Terminated> terminateList = db.Terminate.GetAllWithDataTeminate().Where(Model => Model.IsActive == true && Model.IsDeleted == false).ToList();
            List<vmTerminate> terminateItem = new List<vmTerminate>();
            foreach (var item in terminateList)
            {
                terminateItem.Add(new vmTerminate
                {
                    Id = item.Id,
                    EmployeeId = item.EmployeeId,
                    TerminatedDate=item.TerminatedDate,
                    Reason = item.Reason,
                    Status = item.Status,
                    EmployeeName = item.Employee.FullName,
                    Approveby = item.Approveby,
                    ApproveDate = item.ApproveDate,
                    FromDateChange = item.TerminatedDate.ToString("dd MMMM, yyyy"),
                });
            }

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                // AllLoans = AllLoans.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                terminateItem = terminateItem.OrderByDescending(model => model.Id).ToList();
            }

            //Search
            if (!string.IsNullOrEmpty(searchValue))
            {
                terminateItem = terminateItem.Where(model => model.TerminatedDate.ToShortDateString().Contains(searchValue)).ToList();

            }


            //total number of rows count     
            recordsTotal = terminateItem.Count();

            //Paging     
            var data = terminateItem.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}