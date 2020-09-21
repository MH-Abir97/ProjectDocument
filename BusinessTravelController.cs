using System;
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
using Pronali.Web.Controllers;
using Pronali.Web.Extension;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Hosting;
using Pronali.Web.Helper;
using Pronali.Web.Areas.HR.Models.Employee;
using Pronali.Web.Areas.HR.Models.Leave;
using vmMyApplications = Pronali.Web.Areas.HR.Models.BusinessTravel.vmMyApplications;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class BusinessTravelController : BaseController
    {
        private readonly IHostingEnvironment _env;
        private readonly IImagePath _imagePath;

        public BusinessTravelController(IUnitOfWork _unitOfWork, IHostingEnvironment env, IImagePath imagePath) : base(_unitOfWork)
        {
            _env = env;
            _imagePath = imagePath;
        }

        #region CustomMethids
        private vmBusinessTravelApplicationCreate GetAdditionalShowingInfo(vmBusinessTravelApplicationCreate vmBusinessTravelApplicationCreate)
        {
            var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            Employee employee = db.Employee.GetFirstOrDefaultWithRelatedData(c => c.Id == loggedInEmployeeId && c.IsActive == true && c.IsDeleted == false);
            if (employee.Superiror != null)
            {
                var applicationTo = new List<SelectListItem>();
                applicationTo.Add(new SelectListItem { Text = employee.Superiror.MaskingId + " || " + employee.Superiror.FullName, Value = employee.Superiror.Id.ToString() });
                vmBusinessTravelApplicationCreate.ApplicationToList = applicationTo;
            }
            
            vmBusinessTravelApplicationCreate.EmployeeId = employee.MaskingId;
            vmBusinessTravelApplicationCreate.Name = employee.FullName;
            vmBusinessTravelApplicationCreate.Department = employee.Department.Name;
            vmBusinessTravelApplicationCreate.Designation = employee.Designation.Name;
            vmBusinessTravelApplicationCreate.Department = employee.Department.Name;
            vmBusinessTravelApplicationCreate.Username = User.Identity.Name;

            vmBusinessTravelApplicationCreate.ApplicationStatusList = GetApplicationStatusList(employee.Id);

            return vmBusinessTravelApplicationCreate;
        }

        private List<SelectListItem> GetApplicationStatusList(long employeeId)
        {
            List<SelectListItem> applicationStatusList = new List<SelectListItem>();

            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Approved.ToString(), Value = db.BusinessApplication.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Approved).ToString() });
            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Pending.ToString(), Value = db.BusinessApplication.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Pending).ToString() });
            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Rejected.ToString(), Value = db.BusinessApplication.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Rejected).ToString() });

            return applicationStatusList;
        }

        private List<SelectListItem> GetShortApplicationStatusList(long employeeId)
        {
            List<SelectListItem> applicationStatusList = new List<SelectListItem>();

            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Approved.ToString(), Value = db.ShortBusinessApplication.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Approved).ToString() });
            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Pending.ToString(), Value = db.ShortBusinessApplication.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Pending).ToString() });
            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Rejected.ToString(), Value = db.ShortBusinessApplication.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Rejected).ToString() });

            return applicationStatusList;
        }

        private bool AddBusinessTravelApplication(vmBusinessTravelApplicationCreate vmBusinessTravelApplicationCreate)
        {
            bool isBusinessTravelApplicationAdded = false;

            BusinessApplication businessApplication = new BusinessApplication();
            var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            Employee employee = db.Employee.GetFirstOrDefaultWithRelatedData(c => c.Id == loggedInEmployeeId && c.IsActive == true && c.IsDeleted == false);
            businessApplication.EmployeeId = employee.Id;
            businessApplication.Purpose = vmBusinessTravelApplicationCreate.Purpose;
            businessApplication.Where = vmBusinessTravelApplicationCreate.Where;
            businessApplication.FromDate = DateTime.Parse(vmBusinessTravelApplicationCreate.From);
            businessApplication.ToDate = DateTime.Parse(vmBusinessTravelApplicationCreate.To);
            businessApplication.Status = ApplicationStatus.Pending;
            businessApplication.ApplicationToId = vmBusinessTravelApplicationCreate.ApplicationTo;
            db.BusinessApplication.Add(businessApplication);

            isBusinessTravelApplicationAdded = db.Save() > 0;
            if (isBusinessTravelApplicationAdded)
            {
                db.ApplicationApproval.SetApprover(ApplicationType.BusinessTravel, loggedInEmployeeId, businessApplication.Id.ToString(), businessApplication.CreatedDate, businessApplication.CreatedBy);
            }

            return isBusinessTravelApplicationAdded;
        }

        private vmMyApplications GetAdditionalShowingInfo(vmMyApplications vmMyApplications)
        {
            var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            vmMyApplications.BusinessTravelApplicationStatusList = GetApplicationStatusList(loggedInEmployeeId);
            vmMyApplications.BusinessTravelShortApplicationStatusList = GetShortApplicationStatusList(loggedInEmployeeId);

            return vmMyApplications;
        }

        private vmShortBusinessApplicationCreate GetAdditionalShowingInfo(vmShortBusinessApplicationCreate vmShortBusinessApplicationCreate)
        {
            var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            Employee employee = db.Employee.GetFirstOrDefaultWithRelatedData(c => c.Id == loggedInEmployeeId && c.IsActive == true && c.IsDeleted == false);
            if (employee.Superiror != null)
            {
                var applicationTo = new List<SelectListItem>();
                applicationTo.Add(new SelectListItem { Text = employee.Superiror.MaskingId + " || " + employee.Superiror.FullName, Value = employee.Superiror.Id.ToString() });
                vmShortBusinessApplicationCreate.ApplicationToList = applicationTo;
            }
            vmShortBusinessApplicationCreate.EmployeeId = employee.MaskingId;
            vmShortBusinessApplicationCreate.Name = employee.FullName;
            vmShortBusinessApplicationCreate.Department = employee.Department.Name;
            vmShortBusinessApplicationCreate.Designation = employee.Designation.Name;
            vmShortBusinessApplicationCreate.Department = employee.Department.Name;
            vmShortBusinessApplicationCreate.Username = User.Identity.Name;

            vmShortBusinessApplicationCreate.ApplicationStatusList = GetShortApplicationStatusList(employee.Id);

            return vmShortBusinessApplicationCreate;
        }

        private bool AddShortBusinessTravelApplication(vmShortBusinessApplicationCreate vmShortBusinessApplicationCreate)
        {
            bool isShortBusinessApplicationAdded = false;

            ShortBusinessApplication shortBusinessApplication = new ShortBusinessApplication();
            var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            Employee employee = db.Employee.GetFirstOrDefaultWithRelatedData(c => c.Id == loggedInEmployeeId && c.IsActive == true && c.IsDeleted == false);
            shortBusinessApplication.EmployeeId = employee.Id;
            shortBusinessApplication.Purpose = vmShortBusinessApplicationCreate.Purpose;
            var fromTimeSpan = TwelveHourFormatStringToTimeSpan(vmShortBusinessApplicationCreate.From);
            var fromDate = DateTime.Parse(vmShortBusinessApplicationCreate.Date);
            shortBusinessApplication.FromDate = fromDate.Add(fromTimeSpan);
            var toTimeSpan = TwelveHourFormatStringToTimeSpan(vmShortBusinessApplicationCreate.To);
            var toDate = DateTime.Parse(vmShortBusinessApplicationCreate.Date);
            shortBusinessApplication.ToDate = toDate.Add(toTimeSpan);
            shortBusinessApplication.Status = ApplicationStatus.Pending;
            shortBusinessApplication.ApplicationToId = vmShortBusinessApplicationCreate.ApplicationTo;
            db.ShortBusinessApplication.Add(shortBusinessApplication);

            isShortBusinessApplicationAdded = db.Save() > 0;
            if (isShortBusinessApplicationAdded)
            {
                db.ApplicationApproval.SetApprover(ApplicationType.ShortBusinessTravel, loggedInEmployeeId, shortBusinessApplication.Id.ToString(), shortBusinessApplication.CreatedDate, shortBusinessApplication.CreatedBy);
            }
            return isShortBusinessApplicationAdded;
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
            //var list = db.LeaveApplication.FindWithRelatedData(x=>x.IsDeleted==false &&x.Status==ApplicationStatus.Pending);
            var leaveList = db.ApplicationApproval.GetAll().Where(x => x.ApplicationType == ApplicationType.BusinessTravel && x.ApproverId == loggedInEmployeeId && x.Lock == false).ToList();
            var shortLeaveList = db.ApplicationApproval.GetAll().Where(x => x.ApplicationType == ApplicationType.ShortBusinessTravel && x.ApproverId == loggedInEmployeeId && x.Lock == false).ToList();

            var todaysLeaveList = new List<TodaysLeave>();
            foreach (var item in leaveList)
            {
                var leaveData = db.BusinessApplication.GetFirstOrDefaultwithRelatedData(x => x.IsDeleted == false && x.Id == Convert.ToInt64(item.ApplicationId));
                if (leaveData != null)
                {
                    var days = leaveData.ToDate.AddDays(1) - leaveData.FromDate;
                    var leave = new TodaysLeave
                    {
                        ApproverId = loggedInEmployeeId,
                        Name = leaveData.Employee.FullName,
                        EmployeeId = leaveData.Employee.MaskingId,
                        Remarks = item.Comments ?? "",
                        Days = days.Days.ToString() + " days",
                        From = leaveData.FromDate.ToShortDateString(),
                        To = leaveData.ToDate.ToShortDateString(),
                        Status = item.Status.ToString(),
                        Purpose = leaveData.Purpose,
                        AppliedDate = leaveData.CreatedDate.ToString(),
                        Id = item.Id,
                        FromDate = leaveData.FromDate,
                        ToDate = leaveData.ToDate,
                        LeaveType = "Full",
                        CommentsCount = db.ApplicationApproval.GetAll().Count(x => x.ApplicationType == ApplicationType.BusinessTravel && x.Comments!=null && x.ApplicationId == leaveData.Id.ToString() && x.Level > item.Level)

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

            foreach (var item in shortLeaveList)
            {
                var leaveData = db.ShortBusinessApplication.GetFirstOrDefaultwithRelatedData(x => x.IsDeleted == false && x.Id == Convert.ToInt64(item.ApplicationId));
                if (leaveData != null)
                {
                    var days = leaveData.ToDate - leaveData.FromDate;
                    var leave = new TodaysLeave
                    {
                        ApproverId = loggedInEmployeeId,
                        Name = leaveData.Employee.FullName,
                        EmployeeId = leaveData.Employee.MaskingId,
                        Remarks = leaveData.Comments ?? "",
                        Days = days.TotalHours + " hrs",
                        From = leaveData.FromDate.ToShortDateString() + " " + leaveData.FromDate.ToShortTimeString(),
                        To = leaveData.ToDate.ToShortTimeString(),
                        Status = item.Status.ToString(),
                        Purpose = leaveData.Purpose,
                        AppliedDate = leaveData.CreatedDate.ToString(),
                        FromDate = leaveData.FromDate,
                        ToDate = leaveData.ToDate,
                        Id = item.Id,
                        LeaveType = "Partial",
                        CommentsCount = db.ApplicationApproval.GetAll().Count(x => x.ApplicationType == ApplicationType.ShortBusinessTravel && x.Comments != null && x.ApplicationId == leaveData.Id.ToString() && x.Level > item.Level)

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



        public IActionResult Application()
        {
            vmBusinessTravelApplicationCreate vmBusinessTravelApplicationCreate = new vmBusinessTravelApplicationCreate();
            vmBusinessTravelApplicationCreate = GetAdditionalShowingInfo(vmBusinessTravelApplicationCreate);
            return View(vmBusinessTravelApplicationCreate);
        }

        [HttpPost]
        public IActionResult Application(vmBusinessTravelApplicationCreate vmBusinessTravelApplicationCreate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isOverlap = CheckLeaveOverlap(vmBusinessTravelApplicationCreate);
                    if (isOverlap == false)
                    {
                        bool isSaved = AddBusinessTravelApplication(vmBusinessTravelApplicationCreate);

                        if (isSaved)
                        {

                            vmBusinessTravelApplicationCreate model = new vmBusinessTravelApplicationCreate();
                            model = GetAdditionalShowingInfo(model);

                            model.IsModelValid = true;
                            model.ErrorMessage = "Business travel application successfully added. Please wait for the confirmation.";

                            return Json(model);
                        }
                    }

                    

                    vmBusinessTravelApplicationCreate.IsModelValid = false;
                    vmBusinessTravelApplicationCreate.ErrorMessage = "Business travel application can not be added. Something went wrong. Please try again.";

                    return Json(vmBusinessTravelApplicationCreate);
                }

                vmBusinessTravelApplicationCreate.IsModelValid = false;
                vmBusinessTravelApplicationCreate.ErrorMessage = "Validation Failed!. Please try Again with valid data.";

                return Json(vmBusinessTravelApplicationCreate);
            }
            catch
            {
                vmBusinessTravelApplicationCreate.IsModelValid = false;
                vmBusinessTravelApplicationCreate.ErrorMessage = "Business travel application can not be added. Something went wrong. Please try Again.";

                return Json(vmBusinessTravelApplicationCreate);
            }
        }
        private bool CheckLeaveOverlap(vmBusinessTravelApplicationCreate vm)
        {
            var loggedEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            var leaveApp = db.BusinessApplication.GetAll().Where(x => x.EmployeeId == loggedEmployeeId);
            var selectedDate = new List<DateTime>();
            DateTime start = DateTime.Parse(vm.From);
            DateTime end = DateTime.Parse(vm.To);
            for (DateTime counter = start; counter <= end; counter = counter.AddDays(1))
            {
                selectedDate.Add(counter);
            }

            bool flag = false;
            foreach (var item in leaveApp)
            {
                for (DateTime counter = item.FromDate; counter <= item.ToDate; counter = counter.AddDays(1))
                {
                    foreach (var selectedItem in selectedDate)
                    {
                        if (counter.Date == selectedItem.Date)
                        {
                            flag = true;
                            break;
                        }

                    }
                    if (flag == true)
                    {
                        break;
                    }
                }
                if (flag == true)
                {
                    break;
                }
            }

            return flag;
        }
        public IActionResult ShortApplication()
        {
            vmShortBusinessApplicationCreate vmShortBusinessApplicationCreate = new vmShortBusinessApplicationCreate();
            vmShortBusinessApplicationCreate = GetAdditionalShowingInfo(vmShortBusinessApplicationCreate);
            return View(vmShortBusinessApplicationCreate);
        }

        [HttpPost]
        public IActionResult ShortApplication(vmShortBusinessApplicationCreate vmShortBusinessApplicationCreate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isSaved = AddShortBusinessTravelApplication(vmShortBusinessApplicationCreate);

                    if (isSaved)
                    {
                        vmShortBusinessApplicationCreate model = new vmShortBusinessApplicationCreate();
                        model = GetAdditionalShowingInfo(model);

                        model.IsModelValid = true;
                        model.ErrorMessage = "Short business travel application successfully added. Please wait for the confirmation.";

                        return Json(model);
                    }

                    vmShortBusinessApplicationCreate.IsModelValid = false;
                    vmShortBusinessApplicationCreate.ErrorMessage = "Short business travel application can not be added. Something went wrong. Please try Again.";

                    return Json(vmShortBusinessApplicationCreate);
                }

                vmShortBusinessApplicationCreate.IsModelValid = false;
                vmShortBusinessApplicationCreate.ErrorMessage = "Validation Failed!. Please try Again with valid data.";

                return Json(vmShortBusinessApplicationCreate);
            }
            catch
            {
                vmShortBusinessApplicationCreate.IsModelValid = false;
                vmShortBusinessApplicationCreate.ErrorMessage = "Short business travel application can not be added. Something went wrong. Please try Again.";

                return Json(vmShortBusinessApplicationCreate);
            }

        }

        public IActionResult MyApplications()
        {
            vmMyApplications vmMyApplications = new vmMyApplications();
            vmMyApplications = GetAdditionalShowingInfo(vmMyApplications);

            return View(vmMyApplications);
        }

        public IActionResult LoadMyApplications()
        {
            try
            {
                var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
                var leaveApplicationList = db.BusinessApplication.FindWithRelatedData(c => c.EmployeeId == loggedInEmployeeId && c.IsActive == true && c.IsDeleted == false).OrderByDescending(c => c.Id).ToList();

                List<vmLeaveApplicationGrid> vmLeaveApplicationGrid = leaveApplicationList
                    .Select(x => new vmLeaveApplicationGrid()
                    {
                        Id = x.Id,
                        LeaveName = "Business Travel",
                        From = x.FromDate.ToString("dddd, dd MMMM yyyy"),
                        To = x.ToDate.ToString("dddd, dd MMMM yyyy"),
                        ApplyDate = x.CreatedDate.ToString("dddd, dd MMMM yyyy"),
                        Purpose = x.Purpose,
                        Days = Convert.ToInt32((x.ToDate - x.FromDate).TotalDays + 1),
                        WithoutPay = false,
                        Status = x.Status
                    }
                    ).ToList();

                foreach (var item in vmLeaveApplicationGrid)
                {
                    var leaveApprovalList = db.ApplicationApproval.GetAll().Where(x => x.ApplicationId == item.Id.ToString() && x.ApplicationType == ApplicationType.BusinessTravel && x.Lock == false).ToList();
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
                    vmLeaveApplicationGrid = vmLeaveApplicationGrid.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
                }
                else
                {
                    vmLeaveApplicationGrid = vmLeaveApplicationGrid.OrderByDescending(x => x.Id).ToList();
                }

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    vmLeaveApplicationGrid = vmLeaveApplicationGrid
                        .Where(x =>
                        x.LeaveName.ToLower().Contains(searchValue.ToLower()) ||
                        x.From.ToLower().Contains(searchValue.ToLower()) ||
                        x.To.ToLower().Contains(searchValue.ToLower()) ||
                        x.Purpose.ToLower().Contains(searchValue.ToLower()) ||
                        x.Status.ToString().ToLower().Contains(searchValue.ToLower())
                    ).ToList();
                }

                //total number of rows count     
                recordsTotal = vmLeaveApplicationGrid.Count();

                //Paging     
                var data = vmLeaveApplicationGrid.Skip(skip).Take(pageSize).ToList();

                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { draw = "", recordsFiltered = 0, recordsTotal = 0, data = new vmLeaveApplicationGrid() });
            }
        }

        public IActionResult LoadMyShortApplications()
        {
            try
            {
                var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
                var leaveApplicationList = db.ShortBusinessApplication.FindWithRelatedData(c => c.EmployeeId == loggedInEmployeeId && c.IsActive == true && c.IsDeleted == false).OrderByDescending(c => c.Id).ToList();

                List<vmLeaveApplicationGrid> vmLeaveApplicationGrid = leaveApplicationList
                    .Select(x => new vmLeaveApplicationGrid()
                    {
                        Id = x.Id,
                        LeaveName = "Business Travel",
                        From = x.FromDate.ToString("hh:mm tt"),
                        To = x.ToDate.ToString("hh:mm tt"),
                        ApplyDate = x.CreatedDate.ToString("dddd, dd MMMM yyyy"),
                        Purpose = x.Purpose,
                        Days = Convert.ToInt32((x.ToDate - x.FromDate).TotalDays + 1),
                        WithoutPay = false,
                        Status = x.Status
                    }
                    ).ToList();

                foreach (var item in vmLeaveApplicationGrid)
                {
                    var leaveApprovalList = db.ApplicationApproval.GetAll().Where(x => x.ApplicationId == item.Id.ToString() && x.ApplicationType == ApplicationType.ShortBusinessTravel && x.Lock == false).ToList();
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
                    vmLeaveApplicationGrid = vmLeaveApplicationGrid.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
                }
                else
                {
                    vmLeaveApplicationGrid = vmLeaveApplicationGrid.OrderByDescending(x => x.Id).ToList();
                }

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    vmLeaveApplicationGrid = vmLeaveApplicationGrid
                        .Where(x =>
                        x.LeaveName.ToLower().Contains(searchValue.ToLower()) ||
                        x.From.ToLower().Contains(searchValue.ToLower()) ||
                        x.To.ToLower().Contains(searchValue.ToLower()) ||
                        x.Purpose.ToLower().Contains(searchValue.ToLower()) ||
                        x.Status.ToString().ToLower().Contains(searchValue.ToLower())
                    ).ToList();
                }

                //total number of rows count     
                recordsTotal = vmLeaveApplicationGrid.Count();

                //Paging     
                var data = vmLeaveApplicationGrid.Skip(skip).Take(pageSize).ToList();

                //Returning Json Data    
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { draw = "", recordsFiltered = 0, recordsTotal = 0, data = new vmLeaveApplicationGrid() });
            }

        }
        #endregion Action
    }
}