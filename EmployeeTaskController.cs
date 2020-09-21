using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pronali.Data;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.EmployeeTask;
using Pronali.Web.Controllers;
using System.Linq.Dynamic.Core;
using Pronali.Web.Extension;
using Pronali.Web.Areas.HR.Models.Employee;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    public class EmployeeTaskController :BaseController
    {
    
        public EmployeeTaskController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
        }
        public IActionResult Index()
        {

            ViewBag.itemsTime = DateTime.Now.ToString("dddd dd MMMMM yyyy");
            return View();
        }

        public IActionResult GetDropdownList()
        {
            var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            var result = db.Employee.GetAll().Where(x=>x.Id != loggedInEmployeeId).ToList();                  
            return Json(result);
        }


        public IActionResult ReplayList()
        {
          
           vmEmployeeTask vmEmployeeTask=new vmEmployeeTask();

            return PartialView("_Reply", vmEmployeeTask);
        }

        public IActionResult ReplayUpdate(vmEmployeeTask modelData)
        {

            if (ModelState.IsValid)
            {
                var employeeObj = db.EmployeeTask.Get(modelData.Id);
              //  var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);

                if (employeeObj != null)
                {
                   // employeeObj.AssigneeId = loggedInEmployeeId;
                    //employeeObj.AssignToId = modelData.AssignToId;

                    employeeObj.Replay = modelData.Reply;
                    //employeeObj.Replay = modelData.Replay != null ? modelData.Replay : employeeObj.Replay;

                    db.Save();
                    if (employeeObj.Id > 0)
                    {
                        return Json(true);
                    }

                }

            }
            return Json(false);
        }


        public IActionResult EmployeeTaskList()
        {


            return PartialView("_Marking");
        }

        [HttpPost]
        public IActionResult EmployeeComentUpdate(vmEmployeeTask modelData)
        {

            if (ModelState.IsValid)
            {

                var employeeObj = db.EmployeeTask.Get(modelData.Id);
               // var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);

                if (employeeObj != null)
                {
                    // employeeObj.AssigneeId = loggedInEmployeeId;
                    // employeeObj.AssignToId = modelData.AssignToId;

                    employeeObj.Marks = modelData.Marks == modelData.Marks ? modelData.Marks : employeeObj.Marks;
                    employeeObj.Comments = modelData.Comments != null ? modelData.Comments: employeeObj.Comments;

                    db.Save();
                    if (employeeObj.Id > 0)
                    {
                        return Json(true);
                    }

                }
              

            }
            return Json(false);
        }


        [HttpPost]
        public IActionResult Create(vmEmployeeTask vmEmployeeTask)
        {

            var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            if (loggedInEmployeeId== vmEmployeeTask.AssignToId)
            {
                return Json(false);
            }
            EmployeeTask task = new EmployeeTask() 
            {
                AssigneeId=loggedInEmployeeId,
                
                AssignToId=vmEmployeeTask.AssignToId,
                    
                Responsibility=vmEmployeeTask.Responsibility,
                DueDate=vmEmployeeTask.ChangeDueDate != null ? vmEmployeeTask.DueDate: DateTime.Now,
                Marks =vmEmployeeTask.Marks,
                Comments=vmEmployeeTask.Comments,
            };
            db.EmployeeTask.Add(task);

            db.Save();

            if (task.Id > 0 )
            {
                return Json(true);
            }
                return Json(false);
                 
        }

       

        [HttpPost]
        public IActionResult Edit(vmEmployeeTask modelData)
        {
            var employeeObj = db.EmployeeTask.Get(modelData.Id);
            var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            //if (loggedInEmployeeId == modelData.AssignToId)
            //{
            //    return Json(false);
            //}
            if (employeeObj !=null)
            {
                employeeObj.AssigneeId = loggedInEmployeeId;
                employeeObj.AssignToId = modelData.AssignToId;
                employeeObj.Responsibility = modelData.Responsibility;
               // employeeObj.DueDate = modelData.DueDate;
                employeeObj.DueDate = modelData.ChangeDueDate != null ? modelData.DueDate : DateTime.Now;
                
                db.Save();
                if (employeeObj.Id >0)
                {
                    return Json(true);
                }
                
            }
            return Json(false);
        }

        public IActionResult Delete(long id)
        {
            var employeeTask = db.EmployeeTask.Get(id);
            db.EmployeeTask.Remove(employeeTask);
            db.Save();
            
            return Json(true);
        }

        public IActionResult LoadEmployeeTask()
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

            var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);

            var employeeTask = (from employeeTaskName in db.EmployeeTask.GetAll()
                                join employeeName in db.Employee.GetAll() on employeeTaskName.AssignToId equals employeeName.Id into Mixed
                                from finalEmployeeTask in Mixed.DefaultIfEmpty()
                                where employeeTaskName.AssigneeId== loggedInEmployeeId
                                select new
                                {
                                    employeeTaskName.Id,
                                    finalEmployeeTask.FullName,
                                    employeeTaskName.Responsibility,
                                    employeeTaskName.DueDate,
                                    employeeTaskName.AssignToId,
                                    employeeTaskName.AssigneeId,
                                    employeeTaskName.Replay,
                                    employeeTaskName.Marks,
                                    employeeTaskName.Comments,
                                  
                                }).ToList();

            List<vmEmployeeTask> employeeTaskList = new List<vmEmployeeTask>();
            foreach ( var items in employeeTask )
            {
                vmEmployeeTask get = new vmEmployeeTask
                {
                    Id = items.Id,
                    FullName = items.FullName,
                    Responsibility = items.Responsibility,
                    DueDate = items.DueDate.Date,
                    AssigneeId = items.AssigneeId,
                    AssignToId = items.AssignToId,
                    Reply = items.Replay,
                    Comments=items.Comments,
                    Marks=items.Marks,
			
                    ChangeDueDate = items.DueDate.ToString("dddd dd MMMMM yyyy")

                };
                employeeTaskList.Add(get);
            }
            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                // AllLoans = AllLoans.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                employeeTask = employeeTask.OrderByDescending(model => model.FullName).ToList();
            }

            //Search
            if (!string.IsNullOrEmpty(searchValue))
            {
                employeeTask = employeeTask.Where(model => model.FullName.Contains(searchValue)).ToList();

            }


            //total number of rows count     
            recordsTotal = employeeTask.Count();

            //Paging     
            var data = employeeTask.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = employeeTaskList });
        }

        //==========================Start LoadAssignTask  Operations=========================================//

        public IActionResult LoadAssignTask()
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

            var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);

            List<EmployeeTask> employeeList = db.EmployeeTask.GetWithAllData().Where(c=>c.AssignToId== loggedInEmployeeId).ToList();

            List<vmEmployeeTask> employeeTask = new List<vmEmployeeTask>();

            foreach (var item in employeeList)
            {
                employeeTask.Add(new vmEmployeeTask
                {
                    Id = item.Id,
                    AssigneeId = item.AssigneeId,
                    AssignToId = item.AssignToId,
                    AssigneeName = item.Assignee.FullName,
                    AssigneeToName = item.AssignTo.FullName,
                    Responsibility=item.Responsibility,
                    DueDate=item.DueDate,
                    Marks=item.Marks,
                    Comments=item.Comments,
                    Reply= item.Replay,
                    ChangeDueDate=item.DueDate.ToString("dddd dd MMMMM yyyy")

                });
            }
           
            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                // AllLoans = AllLoans.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                employeeTask = employeeTask.OrderByDescending(model => model.Id).ToList();
            }

            //Search
            if (!string.IsNullOrEmpty(searchValue))
            {
                employeeTask = employeeTask.Where(model => model.Responsibility.Contains(searchValue)).ToList();

            }


            //total number of rows count     
            recordsTotal = employeeTask.Count();

            //Paging     
            var data = employeeTask.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
        //end Loan Disbursement Operations

    }
}