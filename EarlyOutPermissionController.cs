﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pronali.Data;
using Pronali.Data.Enum;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.BusinessTravel;
using Pronali.Web.Areas.HR.Models.EarlyOutPermission;
using Pronali.Web.Controllers;
using Pronali.Web.Extension;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Hosting;
using Pronali.Web.Areas.HR.Models.Employee;
using Pronali.Web.Helper;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class EarlyOutPermissionController : BaseController
    {
        private readonly IHostingEnvironment _env;
        private readonly IImagePath _imagePath;

        public EarlyOutPermissionController(IUnitOfWork _unitOfWork, IHostingEnvironment env, IImagePath imagePath) : base(_unitOfWork)
        {
            _env = env;
            _imagePath = imagePath;
        }

        #region CustomMethids
        private vmEarlyOutPermissionCreate GetAdditionalShowingInfo(vmEarlyOutPermissionCreate vmEarlyOutPermissionCreate)
        {
            var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            Employee employee = db.Employee.GetFirstOrDefaultWithRelatedData(c => c.Id == loggedInEmployeeId && c.IsActive == true && c.IsDeleted == false);
            if (employee.Superiror != null)
            {
                var applicationTo = new List<SelectListItem>();
                applicationTo.Add(new SelectListItem { Text = employee.Superiror.MaskingId + " || " + employee.Superiror.FullName, Value = employee.Superiror.Id.ToString() });
                vmEarlyOutPermissionCreate.ApplicationToList = applicationTo;
            }
            vmEarlyOutPermissionCreate.EmployeeId = employee.MaskingId;
            vmEarlyOutPermissionCreate.Name = employee.FullName;
            vmEarlyOutPermissionCreate.Department = employee.Department.Name;
            vmEarlyOutPermissionCreate.Designation = employee.Designation.Name;
            vmEarlyOutPermissionCreate.Department = employee.Department.Name;
            vmEarlyOutPermissionCreate.Username = User.Identity.Name;

            vmEarlyOutPermissionCreate.ApplicationStatusList = GetApplicationStatusList(employee.Id);

            return vmEarlyOutPermissionCreate;
        }

        private List<SelectListItem> GetApplicationStatusList(long employeeId)
        {
            List<SelectListItem> applicationStatusList = new List<SelectListItem>();

            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Approved.ToString(), Value = db.EarlyOutPermission.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Approved).ToString() });
            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Pending.ToString(), Value = db.EarlyOutPermission.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Pending).ToString() });
            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Rejected.ToString(), Value = db.EarlyOutPermission.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Rejected).ToString() });

            return applicationStatusList;
        }


        private bool AddEarlyOutPermission(vmEarlyOutPermissionCreate vmEarlyOutPermissionCreate)
        {
            bool isEarlyOutPermissionAdded = false;

            EarlyOutPermission EarlyOutPermission = new EarlyOutPermission();
            var loggedIdEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            Employee employee = db.Employee.GetFirstOrDefaultWithRelatedData(c => c.Id == loggedIdEmployeeId && c.IsActive == true && c.IsDeleted == false);
            EarlyOutPermission.EmployeeId = employee.Id;
            EarlyOutPermission.Purpose = vmEarlyOutPermissionCreate.Purpose;
            EarlyOutPermission.WhenTime = DateTime.Parse(vmEarlyOutPermissionCreate.When);
            EarlyOutPermission.Status = ApplicationStatus.Pending;
            EarlyOutPermission.ApplicationToId = vmEarlyOutPermissionCreate.ApplicationTo;
            db.EarlyOutPermission.Add(EarlyOutPermission);

            isEarlyOutPermissionAdded = db.Save() > 0;

            if (isEarlyOutPermissionAdded)
            {
                db.ApplicationApproval.SetApprover(ApplicationType.Early, loggedIdEmployeeId, EarlyOutPermission.Id.ToString(), EarlyOutPermission.CreatedDate, EarlyOutPermission.CreatedBy);
            }

            return isEarlyOutPermissionAdded;
        }

        private TimeSpan TwelveHourFormatStringToTimeSpan(string TwelveHourFormatString)
        {
            return DateTime.ParseExact(TwelveHourFormatString, "h:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
        }
        #endregion CustomMethids

        #region Action
        public IActionResult Manage()
        {

            return View();
        }

        public IActionResult Application()
        {
            vmEarlyOutPermissionCreate vmEarlyOutPermissionCreate = new vmEarlyOutPermissionCreate();
            vmEarlyOutPermissionCreate = GetAdditionalShowingInfo(vmEarlyOutPermissionCreate);
            return View(vmEarlyOutPermissionCreate);
        }

        [HttpPost]
        public IActionResult Application(vmEarlyOutPermissionCreate vmEarlyOutPermissionCreate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isSaved = AddEarlyOutPermission(vmEarlyOutPermissionCreate);

                    if (isSaved)
                    {
                        vmEarlyOutPermissionCreate model = new vmEarlyOutPermissionCreate();
                        model = GetAdditionalShowingInfo(model);

                        model.IsModelValid = true;
                        model.ErrorMessage = "Early out permission successfully added. Please wait for the confirmation.";

                        return Json(model);
                    }

                    vmEarlyOutPermissionCreate.IsModelValid = false;
                    vmEarlyOutPermissionCreate.ErrorMessage = "Early out permission can not be added. Something went wrong. Please try again.";

                    return Json(vmEarlyOutPermissionCreate);
                }

                vmEarlyOutPermissionCreate.IsModelValid = false;
                vmEarlyOutPermissionCreate.ErrorMessage = "Validation Failed!. Please try Again with valid data.";

                return Json(vmEarlyOutPermissionCreate);
            }
            catch
            {
                vmEarlyOutPermissionCreate.IsModelValid = false;
                vmEarlyOutPermissionCreate.ErrorMessage = "Early out permission can not be added. Something went wrong. Please try Again.";

                return Json(vmEarlyOutPermissionCreate);
            }
        }

        public IActionResult MyApplications()
        {
            vmMyApplications vmMyApplications = new vmMyApplications();
            vmMyApplications = GetAdditionalShowingInfo(vmMyApplications);

            return View(vmMyApplications);
        }

        private vmMyApplications GetAdditionalShowingInfo(vmMyApplications vmMyApplications)
        {
            var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            vmMyApplications.BusinessTravelApplicationStatusList = GetApplicationStatusList(loggedInEmployeeId);

            return vmMyApplications;
        }

        public IActionResult LoadMyApplications()
        {
            try
            {
                var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
                var latePermissionApplicationList = db.EarlyOutPermission.FindWithRelatedData(c => c.EmployeeId == loggedInEmployeeId && c.IsActive == true && c.IsDeleted == false).OrderByDescending(c => c.Id).ToList();

                List<vmEarlyOutApplicationGrid> vmEarlyOutApplicationGrid = latePermissionApplicationList
                    .Select(x => new vmEarlyOutApplicationGrid()
                    {
                        Id = x.Id,
                        From = x.WhenTime.ToString("dddd, dd MMMM yyyy hh:mm tt"),
                        ApplyDate = x.CreatedDate.ToString("dddd, dd MMMM yyyy"),
                        Purpose = x.Purpose,
                        Status = x.Status,
                    }
                    ).ToList();


                foreach (var item in vmEarlyOutApplicationGrid)
                {
                    var leaveApprovalList = db.ApplicationApproval.GetAll().Where(x => x.ApplicationId == item.Id.ToString() && x.ApplicationType == ApplicationType.Early && x.Lock == false).ToList();
                    foreach (var status in leaveApprovalList.Select(s => s.Status))
                    {
                        item.StatusList.Add(status);
                    }
                }


                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                //Sorting    
                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
                {
                    vmEarlyOutApplicationGrid = vmEarlyOutApplicationGrid.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
                }
                else
                {
                    vmEarlyOutApplicationGrid = vmEarlyOutApplicationGrid.OrderByDescending(x => x.Id).ToList();
                }

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    vmEarlyOutApplicationGrid = vmEarlyOutApplicationGrid
                        .Where(x =>
                        x.From.ToLower().Contains(searchValue.ToLower()) ||
                        x.Purpose.ToLower().Contains(searchValue.ToLower()) ||
                        x.Status.ToString().ToLower().Contains(searchValue.ToLower())
                    ).ToList();
                }

                //total number of rows count     
                recordsTotal = vmEarlyOutApplicationGrid.Count();

                //Paging     
                var data = vmEarlyOutApplicationGrid.Skip(skip).Take(pageSize).ToList();

                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { draw = "", recordsFiltered = 0, recordsTotal = 0, data = new vmBusinessTravelApplicationGrid() });
            }
        }

        public JsonResult LoadAllApplication()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            var dateRange = Request.Form["dateRange"].FirstOrDefault();



            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;






            var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            var leaveList = db.ApplicationApproval.GetAll().Where(x => x.ApplicationType == ApplicationType.Early && x.ApproverId == loggedInEmployeeId && x.Lock == false).ToList();
            var todaysLeaveList = new List<TodaysLeave>();
            foreach (var item in leaveList)
            {
                var leaveData = db.EarlyOutPermission.GetFirstOrDefaultwithRelatedData(x => x.IsDeleted == false && x.Id == Convert.ToInt64(item.ApplicationId));
                if (leaveData != null)
                {
                    var leave = new TodaysLeave
                    {
                        ApproverId = loggedInEmployeeId,
                        Name = leaveData.Employee.FullName,
                        EmployeeId = leaveData.Employee.MaskingId,
                        Remarks = item.Comments ?? "",
                        Days = "",
                        From = leaveData.WhenTime.ToString(),
                        Status = item.Status.ToString(),
                        Purpose = leaveData.Purpose,
                        AppliedDate = leaveData.CreatedDate.ToString(),
                        Id = item.Id,
                        FromDate = leaveData.WhenTime,
                        LeaveType = "Early Out",
                        CommentsCount = db.ApplicationApproval.GetAll().Count(x => x.ApplicationType == ApplicationType.Early && x.Lock == false && x.ApplicationId == leaveData.Id.ToString() && x.Level > item.Level)

                    };

                    if (!string.IsNullOrEmpty(leaveData.Employee.PhotoUrl))
                    {
                        leave.PhotoUrl = _imagePath.GetFilePathAsSourceUrl(leaveData.Employee.PhotoUrl);
                    }
                    else
                    {
                        leave.PhotoUrl = _imagePath.GetFilePathAsSourceUrl("/images/Uploads/Employee/AlterImage.png");
                    }


                    if (item.ApproveTime != null)
                    {
                        leave.ApprovedBy = "(" + item.Approver.MaskingId + ") " + item.Approver.FullName;
                        leave.ApprovedTime = leaveData.ApprovedTime.Value.ToString();
                    }
                    else
                    {
                        leave.ApprovedBy = "";
                        leave.ApprovedTime = "";
                    }

                    if (!string.IsNullOrEmpty(leaveData.Employee.PhotoUrl))
                    {
                        leave.PhotoUrl = _imagePath.GetFilePathAsSourceUrl(leaveData.Employee.PhotoUrl);
                    }

                    todaysLeaveList.Add(leave);
                }

            }


            //check job status
            if (searchValue == "all")
            {
                todaysLeaveList = todaysLeaveList.ToList();
                searchValue = "";
            }
            else if (searchValue == "pending")
            {
                todaysLeaveList = todaysLeaveList.Where(x => x.Status == "Pending").ToList();
                searchValue = "";
            }
            else if (searchValue == "approved")
            {
                todaysLeaveList = todaysLeaveList.Where(x => x.Status == "Approved").ToList();
                searchValue = "";
            }
            else if (searchValue == "rejected")
            {
                todaysLeaveList = todaysLeaveList.Where(x => x.Status == "Rejected").ToList();
                searchValue = "";
            }
            else
            {
                todaysLeaveList = todaysLeaveList.Where(x => x.Status == "Pending").ToList();
            }

            if (!string.IsNullOrEmpty(dateRange))
            {
                var startDate = DateTime.Parse(dateRange.Substring(0, 10));
                var endDate = DateTime.Parse(dateRange.Substring(13, 10));
                todaysLeaveList = todaysLeaveList.Where(x => x.FromDate >= startDate && x.ToDate <= endDate).ToList();
            }




            //total number of rows count     
            recordsTotal = todaysLeaveList.Count();

            //Paging     
            var data = todaysLeaveList.Skip(skip).Take(pageSize).OrderBy(o => o.Status).ThenByDescending(t => t.FromDate).ToList();
            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }




        #endregion Action
    }
}