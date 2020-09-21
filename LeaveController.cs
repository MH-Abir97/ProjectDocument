using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Pronali.Web.Controllers;
using Pronali.Web.Areas.HR.Models.Leave;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using Pronali.Data;
using Pronali.Data.Enum;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Extension;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Pronali.Web.Areas.HR.Models.Employee;
using Pronali.Web.Helper;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class LeaveController : BaseController
    {
        private readonly IUnitOfWork db;
        private readonly IHostingEnvironment _env;
        private readonly DropdownHelper _dropdownHelper;
        private readonly IImagePath _imagePath;
        public LeaveController(IUnitOfWork _unitOfdb, IHostingEnvironment env, IImagePath imagePath) : base(_unitOfdb)
        {
            db = _unitOfdb;
            _env = env;
            _dropdownHelper = new DropdownHelper();
            _imagePath = imagePath;
        }

        #region CustomMethods
        private vmLeaveApplicationCreate GetAdditionalShowingInfo(vmLeaveApplicationCreate vmLeaveApplicationCreate)
        {

            var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            Employee employee = db.Employee.GetFirstOrDefaultWithRelatedData(c => c.Id == loggedInEmployeeId && c.IsActive == true && c.IsDeleted == false);
            if (employee.Superiror != null)
            {
                var applicationTo = new List<SelectListItem>();
                applicationTo.Add(new SelectListItem { Text = employee.Superiror.MaskingId + " || " + employee.Superiror.FullName, Value = employee.Superiror.Id.ToString() });
                vmLeaveApplicationCreate.ApplicationToList = applicationTo;
            }
            vmLeaveApplicationCreate.EmployeeId = employee.MaskingId;
            vmLeaveApplicationCreate.Name = employee.FullName;
            vmLeaveApplicationCreate.Department = employee.Department.Name;
            vmLeaveApplicationCreate.Designation = employee.Designation.Name;
            vmLeaveApplicationCreate.Department = employee.Department.Name;
            vmLeaveApplicationCreate.LeaveTypeList = employee.EmployeeLeaveList.Where(c => c.IsDeleted == false && c.IsActive == true && c.Leave.IsDeleted == false && c.Leave.IsActive == true).Select(x => new SelectListItem() { Text = x.Leave.Name, Value = x.Leave.Id.ToString() }).ToList();
            vmLeaveApplicationCreate.EmployeeLeaveList = employee.EmployeeLeaveList.Where(c => c.IsDeleted == false && c.IsActive == true ).ToList();
            vmLeaveApplicationCreate.Username = User.Identity.Name;

            vmLeaveApplicationCreate.ApplicationStatusList = GetApplicationStatusList(employee.Id);

            return vmLeaveApplicationCreate;
        }

        private vmShortLeaveApplicationCreate GetAdditionalShowingInfo(vmShortLeaveApplicationCreate vmShortLeaveApplicationCreate)
        {
            var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            Employee employee = db.Employee.GetFirstOrDefaultWithRelatedData(c => c.Id == loggedInEmployeeId && c.IsActive == true && c.IsDeleted == false);
            if (employee.Superiror != null)
            {
                var applicationTo = new List<SelectListItem>();
                applicationTo.Add(new SelectListItem { Text = employee.Superiror.MaskingId + " || " + employee.Superiror.FullName, Value = employee.Superiror.Id.ToString() });
                vmShortLeaveApplicationCreate.ApplicationToList = applicationTo;
            }
            vmShortLeaveApplicationCreate.EmployeeId = employee.MaskingId;
            vmShortLeaveApplicationCreate.Name = employee.FullName;
            vmShortLeaveApplicationCreate.Department = employee.Department.Name;
            vmShortLeaveApplicationCreate.Designation = employee.Designation.Name;
            vmShortLeaveApplicationCreate.Username = User.Identity.Name;

            vmShortLeaveApplicationCreate.ApplicationStatusList = GetShortApplicationStatusList(employee.Id);

            return vmShortLeaveApplicationCreate;
        }

        private vmMyApplications GetAdditionalShowingInfo(vmMyApplications vmMyApplications)
        {
            var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            vmMyApplications.LeaveApplicationStatusList = GetApplicationStatusList(loggedInEmployeeId);
            vmMyApplications.LeaveShortApplicationStatusList = GetShortApplicationStatusList(loggedInEmployeeId);

            return vmMyApplications;
        }

        private List<SelectListItem> GetShortApplicationStatusList(long employeeId)
        {
            List<SelectListItem> applicationStatusList = new List<SelectListItem>();

            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Approved.ToString(), Value = db.ShortLeaveApplication.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Approved).ToString() });
            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Pending.ToString(), Value = db.ShortLeaveApplication.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Pending).ToString() });
            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Rejected.ToString(), Value = db.ShortLeaveApplication.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Rejected).ToString() });

            return applicationStatusList;
        }

        private List<SelectListItem> GetApplicationStatusList(long employeeId)
        {
            List<SelectListItem> applicationStatusList = new List<SelectListItem>();

            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Approved.ToString(), Value = db.LeaveApplication.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Approved).ToString() });
            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Pending.ToString(), Value = db.LeaveApplication.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Pending).ToString() });
            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Rejected.ToString(), Value = db.LeaveApplication.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Rejected).ToString() });

            return applicationStatusList;
        }

        public bool IsBalanceAvailableForLeave(int LeaveType, DateTime From, DateTime To, bool WithoutPay)
        {
            bool result = false;

            if (WithoutPay == true)
            {
                result = true;
            }
            else
            {
                int leaveId = LeaveType;

                var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
                Employee employee = db.Employee.GetFirstOrDefaultWithRelatedData(c => c.Id == loggedInEmployeeId && c.IsActive == true && c.IsDeleted == false);

                EmployeeLeave employeeLeave = employee.EmployeeLeaveList.FirstOrDefault(c => c.IsActive == true && c.IsDeleted == false && c.LeaveId == leaveId);

                int totalDays = Convert.ToInt32((To - From).TotalDays + 1);

                if (totalDays <= employeeLeave.Balance)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        private bool AddLeaveApplication(vmLeaveApplicationCreate vmLeaveApplicationCreate)
        {
            bool isLeaveApplicationAdded = false;

            LeaveApplication leaveApplication = new LeaveApplication();
            var loggedEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            Employee employee = db.Employee.GetFirstOrDefaultWithRelatedData(c => c.Id == loggedEmployeeId && c.IsActive == true && c.IsDeleted == false);
            leaveApplication.EmployeeId = employee.Id;
            leaveApplication.LeaveId = vmLeaveApplicationCreate.LeaveType;
            leaveApplication.Purpose = vmLeaveApplicationCreate.Purpose;
            leaveApplication.FromDate = DateTime.Parse(vmLeaveApplicationCreate.From);
            leaveApplication.ToDate = DateTime.Parse(vmLeaveApplicationCreate.To);
            leaveApplication.WithoutPay = vmLeaveApplicationCreate.WithoutPay;
            leaveApplication.Status = ApplicationStatus.Pending;
            leaveApplication.ApplicationToId = vmLeaveApplicationCreate.ApplicationTo;
            bool isBalanceDeducted = false;

            if (leaveApplication.WithoutPay == true)
            {
                isBalanceDeducted = true;
            }
            else
            {
                db.LeaveApplication.Add(leaveApplication);

                isLeaveApplicationAdded = db.Save() > 0;


               
            }

            if (isLeaveApplicationAdded)
            {
                db.ApplicationApproval.SetApprover(ApplicationType.Leave,loggedEmployeeId,leaveApplication.Id.ToString(),leaveApplication.CreatedDate,leaveApplication.CreatedBy);
                isBalanceDeducted = DeductLeaveBalance(leaveApplication);
            }

            return isLeaveApplicationAdded;
        }

        private bool AddShortLeaveApplication(vmShortLeaveApplicationCreate vmShortLeaveApplicationCreate)
        {
            bool isShortLeaveApplicationAdded = false;

            ShortLeaveApplication shortLeaveApplication = new ShortLeaveApplication();
            var loggedEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            Employee employee = db.Employee.GetFirstOrDefaultWithRelatedData(c => c.Id == loggedEmployeeId && c.IsActive == true && c.IsDeleted == false);
            shortLeaveApplication.EmployeeId = employee.Id;
            shortLeaveApplication.Purpose = vmShortLeaveApplicationCreate.Purpose;
            var fromTimeSpan = TwelveHourFormatStringToTimeSpan(vmShortLeaveApplicationCreate.From);
            var fromDate = DateTime.Parse(vmShortLeaveApplicationCreate.Date);
            shortLeaveApplication.FromDate = fromDate.Add(fromTimeSpan);
            var toTimeSpan = TwelveHourFormatStringToTimeSpan(vmShortLeaveApplicationCreate.To);
            var toDate = DateTime.Parse(vmShortLeaveApplicationCreate.Date);
            shortLeaveApplication.ToDate = toDate.Add(toTimeSpan);
            shortLeaveApplication.WithoutPay = vmShortLeaveApplicationCreate.WithoutPay;
            shortLeaveApplication.Status = ApplicationStatus.Pending;
            shortLeaveApplication.ApplicationToId = vmShortLeaveApplicationCreate.ApplicationTo;
            db.ShortLeaveApplication.Add(shortLeaveApplication);

            isShortLeaveApplicationAdded = db.Save() > 0;
            if (isShortLeaveApplicationAdded)
            {
                db.ApplicationApproval.SetApprover(ApplicationType.ShortLeave, loggedEmployeeId, shortLeaveApplication.Id.ToString(), shortLeaveApplication.CreatedDate, shortLeaveApplication.CreatedBy);
            }

            return isShortLeaveApplicationAdded;
        }

        private bool DeductLeaveBalance(LeaveApplication leaveApplication)
        {
            EmployeeLeave employeeLeave = db.EmployeeLeave.GetFirstOrDefault(c => c.IsActive == true && c.IsDeleted == false && c.EmployeeId == leaveApplication.EmployeeId && c.LeaveId == leaveApplication.LeaveId);

            int numberOfLeaveDays = Convert.ToInt32((leaveApplication.ToDate - leaveApplication.FromDate).TotalDays + 1);

            // employeeLeave.Balance = employeeLeave.Balance - numberOfLeaveDays;

            employeeLeave.Enjoyed = employeeLeave.Enjoyed + numberOfLeaveDays;

            db.EmployeeLeave.Update(employeeLeave);

            bool isUpdated = db.Save() > 0;

            return isUpdated;
        }

        private TimeSpan TwelveHourFormatStringToTimeSpan(string TwelveHourFormatString)
        {
            return DateTime.ParseExact(TwelveHourFormatString, "h:mm tt", CultureInfo.InvariantCulture).TimeOfDay;
        }
        #endregion CustomMethods

        #region Actions
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Application()
        {
            vmLeaveApplicationCreate vmLeaveApplicationCreate = new vmLeaveApplicationCreate();
            vmLeaveApplicationCreate = GetAdditionalShowingInfo(vmLeaveApplicationCreate);
            return View(vmLeaveApplicationCreate);
        }

        [HttpPost]
        public JsonResult Application(vmLeaveApplicationCreate vmLeaveApplicationCreate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isLeaveBalanceAvailable = IsBalanceAvailableForLeave(vmLeaveApplicationCreate.LeaveType, DateTime.Parse(vmLeaveApplicationCreate.From), DateTime.Parse(vmLeaveApplicationCreate.To), vmLeaveApplicationCreate.WithoutPay);

                    if (isLeaveBalanceAvailable == true)
                    {
                        //check date overlap
                        bool isOverlap = CheckLeaveOverlap(vmLeaveApplicationCreate);
                        if (isOverlap==false)
                        {
                            bool isSaved = AddLeaveApplication(vmLeaveApplicationCreate);

                            if (isSaved)
                            {
                                
                                vmLeaveApplicationCreate model = new vmLeaveApplicationCreate();
                                model = GetAdditionalShowingInfo(model);

                                model.IsModelValid = true;
                                model.ErrorMessage = "Leave application successfully added. Please wait for the confirmation.";

                                return Json(model);
                            }
                            else
                            {
                                vmLeaveApplicationCreate.IsModelValid = false;
                                vmLeaveApplicationCreate.ErrorMessage = "Leave application can not be added. Something went wrong. Please try Again.";
                            }
                        }
                        else
                        {
                            vmLeaveApplicationCreate.IsModelValid = false;
                            vmLeaveApplicationCreate.ErrorMessage = "Leave application can not be added due to duplicate entry";
                        }


                      

                       
                    }
                    else
                    {
                        vmLeaveApplicationCreate.IsModelValid = false;
                        vmLeaveApplicationCreate.ErrorMessage = "Sorry, You do not have enough leave balance left.";
                    }

                    return Json(vmLeaveApplicationCreate);
                }

                vmLeaveApplicationCreate.IsModelValid = false;
                vmLeaveApplicationCreate.ErrorMessage = "Validation Failed!. Please try Again with valid data.";

                return Json(vmLeaveApplicationCreate);
            }
            catch(Exception ex)
            {
                vmLeaveApplicationCreate.IsModelValid = false;
                vmLeaveApplicationCreate.ErrorMessage = "Leave application can not be added. Something went wrong. Please try Again.";

                return Json(vmLeaveApplicationCreate);
            }

        }

        private bool CheckLeaveOverlap(vmLeaveApplicationCreate vmLeaveApplicationCreate)
        {
            var loggedEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            var leaveApp = db.LeaveApplication.GetAll().Where(x => x.EmployeeId == loggedEmployeeId);
            var selectedDate = new List<DateTime>();
            DateTime start = DateTime.Parse(vmLeaveApplicationCreate.From);
            DateTime end = DateTime.Parse(vmLeaveApplicationCreate.To);
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
            vmShortLeaveApplicationCreate vmShortLeaveApplicationCreate = new vmShortLeaveApplicationCreate();
            vmShortLeaveApplicationCreate = GetAdditionalShowingInfo(vmShortLeaveApplicationCreate);
            return View(vmShortLeaveApplicationCreate);
        }

        [HttpPost]
        public IActionResult ShortApplication(vmShortLeaveApplicationCreate vmShortLeaveApplicationCreate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isSaved = AddShortLeaveApplication(vmShortLeaveApplicationCreate);

                    if (isSaved)
                    {
                        vmShortLeaveApplicationCreate model = new vmShortLeaveApplicationCreate();
                        model = GetAdditionalShowingInfo(model);

                        model.IsModelValid = true;
                        model.ErrorMessage = "Short Leave application successfully added. Please wait for the confirmation.";

                        return Json(model);
                    }

                    vmShortLeaveApplicationCreate.IsModelValid = false;
                    vmShortLeaveApplicationCreate.ErrorMessage = "Short Leave application can not be added. Something went wrong. Please try Again.";

                    return Json(vmShortLeaveApplicationCreate);
                }

                vmShortLeaveApplicationCreate.IsModelValid = false;
                vmShortLeaveApplicationCreate.ErrorMessage = "Validation Failed!. Please try Again with valid data.";

                return Json(vmShortLeaveApplicationCreate);
            }
            catch
            {
                vmShortLeaveApplicationCreate.IsModelValid = false;
                vmShortLeaveApplicationCreate.ErrorMessage = "Short Leave application can not be added. Something went wrong. Please try Again.";

                return Json(vmShortLeaveApplicationCreate);
            }

        }


        public IActionResult MyApplications()
        {
            vmMyApplications vmMyApplications = new vmMyApplications();
            vmMyApplications =  GetAdditionalShowingInfo(vmMyApplications);

            return View(vmMyApplications);
        }

        public IActionResult LoadMyApplications()
        {
            try
            {
                var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
                var leaveApplicationList = db.LeaveApplication.FindWithRelatedData(c => c.EmployeeId == loggedInEmployeeId && c.IsActive == true && c.IsDeleted == false).OrderByDescending(c => c.Id).ToList();

                List<vmLeaveApplicationGrid> vmLeaveApplicationGrid = leaveApplicationList
                    .Select(x => new vmLeaveApplicationGrid()
                    {
                        Id = x.Id,
                        LeaveName = x.Leave.Name,
                        From = x.FromDate.ToString("dddd, dd MMMM yyyy"),
                        To = x.ToDate.ToString("dddd, dd MMMM yyyy"),
                        ApplyDate = x.CreatedDate.ToString("dddd, dd MMMM yyyy"),
                        Purpose = x.Purpose,
                        Days = Convert.ToInt32((x.ToDate - x.FromDate).TotalDays + 1),
                        WithoutPay = x.WithoutPay,
                        Status =x.Status
                    }
                    ).ToList();

                foreach (var item in vmLeaveApplicationGrid)
                {
                    var leaveApprovalList = db.ApplicationApproval.GetAll().Where(x =>x.ApplicationId == item.Id.ToString() && x.ApplicationType == ApplicationType.Leave && x.Lock==false).ToList();
                    foreach (var status in leaveApprovalList.Select(s=>s.Status))
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
                var leaveApplicationList = db.ShortLeaveApplication.FindWithRelatedData(c => c.EmployeeId == loggedInEmployeeId && c.IsActive == true && c.IsDeleted == false).OrderByDescending(c => c.Id).ToList();

                List<vmLeaveApplicationGrid> vmLeaveApplicationGrid = leaveApplicationList
                    .Select(x => new vmLeaveApplicationGrid()
                    {
                        Id = x.Id,
                        LeaveName = "Short Leave",
                        From = x.FromDate.ToString("hh:mm tt"),
                        To = x.ToDate.ToString("hh:mm tt"),
                        ApplyDate = x.CreatedDate.ToString("dddd, dd MMMM yyyy"),
                        Purpose = x.Purpose,
                        Days = Convert.ToInt32((x.ToDate - x.FromDate).TotalDays + 1),
                        WithoutPay = x.WithoutPay,
                        Status = x.Status
                    }
                    ).ToList();

                foreach (var item in vmLeaveApplicationGrid)
                {
                    var leaveApprovalList = db.ApplicationApproval.GetAll().Where(x => x.ApplicationId == item.Id.ToString() && x.ApplicationType == ApplicationType.ShortLeave && x.Lock == false).ToList();
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


        public IActionResult IsBalanceAvailable(int LeaveType, DateTime From, DateTime To, bool WithoutPay)
        {
            bool result = false;

            try
            {
                result = IsBalanceAvailableForLeave(LeaveType, From, To, WithoutPay);

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(result);
            }
        }

        #endregion Actions

        public IActionResult Create()
        {
            vmLeave vmLeave = new vmLeave();
            ViewBag.Branches = new SelectList(db.Branch.GetAll()
                .Where(b => b.IsActive == true && b.IsDeleted == false), "Id", "Name");
            return PartialView("Create", vmLeave);
        }

        [HttpPost]
        public IActionResult Create(vmLeave vmLeave)
        {
            if (ModelState.IsValid)
            {
                Leave leave = new Leave
                {
                    Name = vmLeave.Name,
                    BranchId = vmLeave.BranchId,
                    Flag = vmLeave.Flag,
                    Allocate = vmLeave.Allocate
                };
                db.Leave.Add(leave);
                db.Save();
                return PartialView("Create");
            }
            return PartialView("Create");
        }


        public IActionResult LoadLeave()
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

            var leave = db.Leave.GetAll().Where(d => d.IsActive == true && d.IsDeleted == false);

            var leaveList = new List<vmLeave>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                leave = leave.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                leave = leave.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                leave = leave.Where(x => x.Name.Contains(searchValue)).ToList();
            }

            foreach (var item in leave)
            {
                leaveList.Add(new vmLeave
                {
                    Id = item.Id,
                    Name = item.Name,
                    BranchId = item.BranchId,
                    Flag = item.Flag,
                    Allocate = item.Allocate,
                    CreatedDate = item.CreatedDate
                });
            }

            leaveList = leaveList.OrderByDescending(i => i.CreatedDate.Date)
                .ThenByDescending(i => i.CreatedDate.TimeOfDay).ToList();
            //total number of rows count     
            recordsTotal = leaveList.Count();

            //Paging     
            var data = leaveList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


        //Edit
        public IActionResult Edit(int id)
        {
            Leave leave = db.Leave.GetFirstOrDefault(h => h.Id == id);
            vmLeave vmLeave = new vmLeave()
            {
                Id = leave.Id,
                Name = leave.Name,
                Allocate = leave.Allocate,
                Flag = leave.Flag,
                BranchId = leave.BranchId
            };
            ViewBag.Branches = new SelectList(db.Branch.GetAll()
                .Where(b => b.IsActive == true && b.IsDeleted == false), "Id", "Name");
            return PartialView("_Edit", vmLeave);
        }

        [HttpPost]
        public IActionResult Edit(vmLeave vmLeave)
        {
            if (ModelState.IsValid)
            {
                Leave leave = db.Leave.GetFirstOrDefault(h => h.Id == vmLeave.Id);

                leave.Name = vmLeave.Name;
                leave.Flag = vmLeave.Flag;
                leave.Allocate = vmLeave.Allocate;
                leave.BranchId = vmLeave.BranchId;

                db.Leave.Update(leave);
                db.Save();
                return PartialView("_Edit");
            }
            return PartialView("_Edit");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Leave leave = db.Leave.GetFirstOrDefault(c => c.Id == id);

            leave.IsActive = false;
            leave.IsDeleted = true;
            db.Leave.Update(leave);
            db.Save();
            return Json(leave);
        }

        public IActionResult Manage()
        {
          
            return View();
        }

        [HttpPost]
        public JsonResult loadEmployees()
        {
            CultureInfo culture = new CultureInfo("en-US");
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();

            var keyword = Request.Form["keyword"].FirstOrDefault();
            var joinYear = Request.Form["joinYear"].FirstOrDefault();
            var joinMonth = Request.Form["joinMonth"].FirstOrDefault();
            var companyId = Request.Form["companyId"].FirstOrDefault();
            var departmentId = Request.Form["departmentId"].FirstOrDefault();
            var lineId = Request.Form["lineId"].FirstOrDefault();
            var gender = Request.Form["gender"].FirstOrDefault();
            var specialCase = Request.Form["specialCase"].FirstOrDefault();
            var sisterConcernId = Request.Form["sisterConcernId"].FirstOrDefault();
            var designationId = Request.Form["designationId"].FirstOrDefault();
            var floorId = Request.Form["floorId"].FirstOrDefault();
            var religion = Request.Form["religion"].FirstOrDefault();
            var incentive = Request.Form["incentive"].FirstOrDefault();
            var divisionId = Request.Form["divisionId"].FirstOrDefault();
            var sectionId = Request.Form["sectionId"].FirstOrDefault();
            var employeeGroupId = Request.Form["employeeGroupId"].FirstOrDefault();
            var machineId = Request.Form["machineId"].FirstOrDefault();
            var maritalStatus = Request.Form["maritalStatus"].FirstOrDefault();
            var branchId = Request.Form["branchId"].FirstOrDefault();
            var shiftId = Request.Form["shiftId"].FirstOrDefault();
            var jobLocationId = Request.Form["jobLocationId"].FirstOrDefault();
            var jobStatusId = Request.Form["jobStatusId"].FirstOrDefault();
            var bloodGroup = Request.Form["bloodGroup"].FirstOrDefault();


            var departmentFromChart = Request.Form["departmentFromChart"].FirstOrDefault();
            var branchIdFromChart = Request.Form["branchIdFromChart"].FirstOrDefault();
            var designationIdFromChart = Request.Form["designationIdFromChart"].FirstOrDefault();
            var genderIdFromChart = Request.Form["genderIdFromChart"].FirstOrDefault();
            var joinYearIdFromChart = Request.Form["joinYearIdFromChart"].FirstOrDefault();
            var shiftIdFromChart = Request.Form["shiftIdFromChart"].FirstOrDefault();



            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var list = db.Employee.GetAllWithRelatedData(c => c.IsActive == true && c.IsDeleted == false);
            List<Employee> employees = new List<Employee>();
            //replace photoURL path
            foreach (var item in list)
            {
                var emp = item;
                if (!string.IsNullOrEmpty(item.PhotoUrl))
                {
                    emp.PhotoUrl = _imagePath.GetFilePathAsSourceUrl(item.PhotoUrl);
                }
                else
                {
                    emp.PhotoUrl = _imagePath.GetFilePathAsSourceUrl("/images/Uploads/Employee/AlterImage.png");
                }
                employees.Add(emp);
            }



            if (!string.IsNullOrEmpty(departmentFromChart))
            {
                var dept = Convert.ToInt32(departmentFromChart);
                employees = employees.Where(x => x.DepartmentId == dept).ToList();
            }
            if (!string.IsNullOrEmpty(designationIdFromChart))
            {
                var desig = Convert.ToInt32(designationIdFromChart);
                employees = employees.Where(x => x.DesignationId == desig).ToList();
            }
            if (!string.IsNullOrEmpty(branchIdFromChart))
            {
                var bran = Convert.ToInt32(branchIdFromChart);
                employees = employees.Where(x => x.BranchId == bran).ToList();
            }
            if (!string.IsNullOrEmpty(genderIdFromChart))
            {
                employees = employees.Where(x => x.Gender == genderIdFromChart).ToList();
            }
            if (!string.IsNullOrEmpty(joinYearIdFromChart))
            {
                List<Employee> emp = new List<Employee>();
                foreach (var item in employees)
                {
                    try
                    {
                        string year = Convert.ToDateTime(item.JoinDate, culture).Year.ToString();
                        if (year.Length == 4)
                        {
                            if (year == joinYearIdFromChart)
                            {
                                emp.Add(item);
                            }
                        }
                    }
                    catch (Exception)
                    {

                        continue;
                    }
                }
                employees = emp;
            }

            if (!string.IsNullOrEmpty(shiftIdFromChart))
            {
                employees = employees
                                .Where(x => x.ShiftId == int.Parse(shiftIdFromChart))
                                .ToList();
            }



            if (!string.IsNullOrEmpty(joinYear))
            {
                employees = employees.Where(x => x.JoinDate.Year == int.Parse(joinYear)).ToList();
            }
            if (!string.IsNullOrEmpty(joinMonth))
            {
                employees = employees.Where(x => x.JoinDate.Month == int.Parse(joinMonth)).ToList();
            }
            if (!string.IsNullOrEmpty(companyId))
            {
                employees = employees.Where(x => x.CompanyId == int.Parse(companyId)).ToList();
            }
            if (!string.IsNullOrEmpty(departmentId))
            {
                employees = employees.Where(x => x.DepartmentId == int.Parse(departmentId)).ToList();
            }
            if (!string.IsNullOrEmpty(lineId))
            {
                employees = employees.Where(x => x.LineId == int.Parse(lineId)).ToList();
            }
            if (!string.IsNullOrEmpty(gender))
            {
                employees = employees.Where(x => x.Gender == gender).ToList();
            }
            if (!string.IsNullOrEmpty(specialCase))
            {
                employees = employees.Where(x => x.SpecialCase == Convert.ToBoolean(specialCase)).ToList();
            }
            if (!string.IsNullOrEmpty(sisterConcernId))
            {
                employees = employees.Where(x => x.SisterConcernId == int.Parse(sisterConcernId)).ToList();
            }
            if (!string.IsNullOrEmpty(designationId))
            {
                employees = employees.Where(x => x.DesignationId == int.Parse(designationId)).ToList();
            }
            if (!string.IsNullOrEmpty(floorId))
            {
                employees = employees.Where(x => x.FloorId == int.Parse(floorId)).ToList();
            }
            if (!string.IsNullOrEmpty(religion))
            {
                employees = employees.Where(x => x.Religion == religion).ToList();
            }
            if (!string.IsNullOrEmpty(incentive))
            {
                employees = employees.Where(x => x.Incentive == Convert.ToBoolean(incentive)).ToList();
            }
            if (!string.IsNullOrEmpty(divisionId))
            {
                employees = employees.Where(x => x.DivisionId == int.Parse(divisionId)).ToList();
            }
            if (!string.IsNullOrEmpty(sectionId))
            {
                employees = employees.Where(x => x.SectionId == int.Parse(sectionId)).ToList();
            }
            if (!string.IsNullOrEmpty(employeeGroupId))
            {
                employees = employees.Where(x => x.EmployeeGroupId == int.Parse(employeeGroupId)).ToList();
            }
            if (!string.IsNullOrEmpty(machineId))
            {
                employees = employees.Where(x => x.MachineId == int.Parse(machineId)).ToList();
            }
            if (!string.IsNullOrEmpty(maritalStatus))
            {
                employees = employees.Where(x => x.MaritalStatus == maritalStatus).ToList();
            }
            if (!string.IsNullOrEmpty(branchId))
            {
                employees = employees.Where(x => x.BranchId == int.Parse(branchId)).ToList();
            }
            if (!string.IsNullOrEmpty(shiftId))
            {
                employees = employees.Where(x => x.ShiftId == int.Parse(shiftId)).ToList();
            }
            if (!string.IsNullOrEmpty(jobLocationId))
            {
                employees = employees.Where(x => x.JobLocationId == int.Parse(jobLocationId)).ToList();
            }
            if (!string.IsNullOrEmpty(jobStatusId))
            {
                employees = employees.Where(x => x.JobStatus == (JobStatus)Enum.Parse(typeof(JobStatus), jobStatusId)).ToList();
            }
            if (!string.IsNullOrEmpty(bloodGroup))
            {
                employees = employees.Where(x => x.BloodGroup == bloodGroup).ToList();
            }








            //check job status
            if (searchValue == "enjoyed")
            {
                employees = employees.Where(x => x.EmployeeLeaveList.Count>0).ToList();
                searchValue = "";
            }
            if (searchValue == "not enjoyed")
            {
                employees = employees.Where(x => x.EmployeeLeaveList.Count==0).ToList();
                searchValue = "";
            }


            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                employees = employees.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                employees = employees.OrderByDescending(o => o.MaskingId).ToList();
            }






            //Search    
            if (!string.IsNullOrEmpty(searchValue) || !string.IsNullOrEmpty(keyword))
            {
                employees = employees.Where(m => m.MaskingId.ToLower().Contains(searchValue.ToLower()) ||
                                                   m.AttendanceMachineId.ToLower().Contains(searchValue.ToLower()) ||
                                                   m.Company.CompanyName.ToLower().Contains(searchValue.ToLower()) ||
                                                   m.Department.Name.ToLower().Contains(searchValue.ToLower()) ||
                                                   m.Shift.Name.ToLower().Contains(searchValue.ToLower()) ||
                                                   m.Designation.Name.ToLower().Contains(searchValue.ToLower()) ||
                                                   m.EmployeeGroup.Name.ToLower().Contains(searchValue.ToLower()) ||
                                                   m.JobLocation.Name.ToLower().Contains(searchValue.ToLower()) ||
                                                   m.FullName.ToLower().Contains(searchValue.ToLower())).ToList();

            }

            recordsTotal = employees.Count();
            var employeesIdList = employees.Select(s => s.Id).ToList();
            employees = employees.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = employees, employeesIdList= employeesIdList });


        }


        public JsonResult leaveEnjoyed()
        {
            var employees = db.Employee.GetAllWithRelatedData(x=>x.JobStatus!=JobStatus.Resigned||x.JobStatus!=JobStatus.Retired||x.JobStatus != JobStatus.Suspended||x.JobStatus != JobStatus.Terminated).ToList();

            var data = new
            {
                enjoyed = employees.Count(x => x.EmployeeLeaveList.Count>0),
                notEnjoyed = employees.Count(x => x.EmployeeLeaveList.Count == 0)
            };
            return Json(data);
        }

        public JsonResult getLeaveType()
        {
            var data = db.Leave.GetAll().Where(x => x.IsDeleted == false && x.IsActive == true).ToList();
            return Json(data);
        }

        public JsonResult assignLeave(int id,List<long>Emp)
        {
            var error = new List<long>();
            var leave = db.Leave.GetFirstOrDefault(x => x.Id == id);
            foreach (var item in Emp)
            {
                var isAvailable = db.EmployeeLeave.GetFirstOrDefault(x =>x.EmployeeId==item && x.LeaveId == id&&x.Year==DateTime.Now.Year);
                if (isAvailable == null)
                {
                    var employeeLeave = new EmployeeLeave
                    {
                        LeaveId = id,
                        Allocate = leave.Allocate,
                        Enjoyed = 0,
                        EmployeeId = item,
                        Year = DateTime.Now.Year,
                        IsActive = true,
                        IsDeleted = false,
                    };
                    db.EmployeeLeave.Add(employeeLeave);
                    db.Save();
                }
                else
                {
                    error.Add(item);
                }

            }
            return Json(error);
        }


        public JsonResult Leaveall()
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
            var leaveList = db.ApplicationApproval.GetAll().Where(x =>x.ApplicationType == ApplicationType.Leave && x.ApproverId == loggedInEmployeeId && x.Lock==false).ToList();
            var shortLeaveList = db.ApplicationApproval.GetAll().Where(x =>x.ApplicationType == ApplicationType.ShortLeave && x.ApproverId == loggedInEmployeeId && x.Lock == false).ToList();

            var todaysLeaveList = new List<TodaysLeave>();
            foreach (var item in leaveList)
            {
                var leaveData = db.LeaveApplication.GetFirstOrDefaultwithRelatedData(x => x.IsDeleted == false && x.Id==Convert.ToInt64(item.ApplicationId));
                if (leaveData != null)
                {
                    var days = leaveData.ToDate.AddDays(1) - leaveData.FromDate;
                    var leave = new TodaysLeave
                    {
                        ApproverId = loggedInEmployeeId,
                        Name = leaveData.Employee.FullName,
                        EmployeeId = leaveData.Employee.MaskingId,
                        Remarks = item.Comments??"",
                        Days = days.Days.ToString()+" days",
                        From = leaveData.FromDate.ToShortDateString(),
                        To = leaveData.ToDate.ToShortDateString(),
                        Status = item.Status.ToString(),
                        Purpose = leaveData.Purpose,
                        AppliedDate = leaveData.CreatedDate.ToString(),
                        Id = item.Id,
                        FromDate = leaveData.FromDate,
                        ToDate = leaveData.ToDate,
                        LeaveType = leaveData.Leave.Name + "(" + leaveData.Leave.Flag + ")",
                        CommentsCount = db.ApplicationApproval.GetAll().Count(x => x.ApplicationType == ApplicationType.Leave && x.Comments != null && x.ApplicationId==leaveData.Id.ToString() && x.Level>item.Level)

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
                var leaveData = db.ShortLeaveApplication.GetFirstOrDefaultwithRelatedData(x => x.IsDeleted == false && x.Id == Convert.ToInt64(item.ApplicationId));
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
                        From = leaveData.FromDate.ToShortDateString()+" "+leaveData.FromDate.ToShortTimeString(),
                        To = leaveData.ToDate.ToShortTimeString(),
                        Status = item.Status.ToString(),
                        Purpose = leaveData.Purpose,
                        AppliedDate = leaveData.CreatedDate.ToString(),
                        FromDate = leaveData.FromDate,
                        ToDate = leaveData.ToDate,
                        Id = item.Id,
                        LeaveType = "Short Leave",
                        CommentsCount = db.ApplicationApproval.GetAll().Count(x => x.ApplicationType == ApplicationType.Leave && x.Comments!=null && x.ApplicationId == leaveData.Id.ToString() && x.Level > item.Level)

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
                todaysLeaveList = todaysLeaveList.Where(x =>  x.Status=="Pending").ToList();
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
            var data = todaysLeaveList.Skip(skip).Take(pageSize).OrderBy(o=>o.Status).ThenByDescending(t=>t.FromDate).ToList();
            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}