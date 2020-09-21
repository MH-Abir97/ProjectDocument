using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Web.Controllers;
using Pronali.Web.Areas.HR.Models.Attendance;
using Pronali.Web.Extension;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Pronali.Data.Enum;
using Pronali.Web.Job;
using Pronali.Web.Areas.HR.Models.Employee;
using Pronali.Data.Helper;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class AttendanceController : BaseController
    {
        private readonly IConfiguration _configuration;

        public AttendanceController(IUnitOfWork _unitOfWork, IConfiguration configuration) : base(_unitOfWork)
        {
            _configuration = configuration;
            Now = BaseHelper.UserGeo().UserDateTime;
            Today = Now.Date;
            Tommorow = Today.AddDays(1);
            FirstDayOfTheMonth = new DateTime(Today.Year, Today.Month, 1);
            LastDayOfTheMonth = FirstDayOfTheMonth.AddMonths(1).AddDays(-1);
        }
        #region Custom Methods
        private vmMyApplications GetAdditionalShowingInfo(vmMyApplications vmMyApplications)
        {
            var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            vmMyApplications.LateInApplicationStatusList = GetLatePermissionApplicationStatusList(loggedInEmployeeId);
            vmMyApplications.EarlyOutApplicationStatusList = GetEarlyOutApplicationStatusList(loggedInEmployeeId);

            return vmMyApplications;
        }

        private List<SelectListItem> GetLatePermissionApplicationStatusList(long employeeId)
        {
            List<SelectListItem> applicationStatusList = new List<SelectListItem>();

            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Approved.ToString(), Value = db.LatePermission.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Approved).ToString() });
            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Pending.ToString(), Value = db.LatePermission.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Pending).ToString() });
            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Rejected.ToString(), Value = db.LatePermission.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Rejected).ToString() });

            return applicationStatusList;
        }

        private List<SelectListItem> GetEarlyOutApplicationStatusList(long employeeId)
        {
            List<SelectListItem> applicationStatusList = new List<SelectListItem>();

            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Approved.ToString(), Value = db.EarlyOutPermission.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Approved).ToString() });
            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Pending.ToString(), Value = db.EarlyOutPermission.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Pending).ToString() });
            applicationStatusList.Add(new SelectListItem() { Text = ApplicationStatus.Rejected.ToString(), Value = db.EarlyOutPermission.Count(c => c.EmployeeId == employeeId && c.IsActive == true && c.IsDeleted == false && c.Status == ApplicationStatus.Rejected).ToString() });

            return applicationStatusList;
        }
        #endregion Custom Methods

        #region Actions


        public IActionResult MyApplications()
        {
            vmMyApplications vmMyApplications = new vmMyApplications();
            vmMyApplications = GetAdditionalShowingInfo(vmMyApplications);

            return View(vmMyApplications);
        }

        public JsonResult PreviousDataSendtoQueue()
        {
            bool flag = false;
            try
            {
                var unProcessedDate = new List<vmPreviousPunchofEmployees>();
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                connectionString = DBEncrypterToolKit.Crypto.DecryptConnectionString(connectionString, "@12390");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand("SELECT  EmployeeId,convert(date,InTime) FROM   hr.AttendanceProcessedData where flag=0", connection))
                    {
                        sqlCommand.CommandType = CommandType.Text;
                        connection.Open();
                        using (SqlDataReader dr = sqlCommand.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                unProcessedDate.Add(new vmPreviousPunchofEmployees { EmployeeId = Convert.ToInt64(dr[0].ToString()), Date = Convert.ToDateTime(dr[1].ToString()) });

                            }
                        }
                        connection.Close();
                    }
                }

                foreach (var item in unProcessedDate.OrderBy(o => o.EmployeeId).ThenBy(d => d.Date))
                {
                    int shiftId = 0;
                    var scheduleTime = new DateTime();
                    //get shift id
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand sqlCommand = new SqlCommand("SELECT ShiftId FROM Hr.Employee where  Id=@ID", connection))
                        {
                            sqlCommand.CommandType = CommandType.Text;
                            sqlCommand.Parameters.AddWithValue("@ID", item.EmployeeId);
                            connection.Open();
                            using (SqlDataReader dr = sqlCommand.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    shiftId = Convert.ToInt32(dr[0].ToString());
                                }
                            }
                            connection.Close();
                        }
                    }
                    //get schedule time
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand sqlCommand = new SqlCommand("SELECT CheckOutTime,DayName FROM Hr.shiftdetails where  ShiftId=@ID and DayName=@day", connection))
                        {
                            sqlCommand.CommandType = CommandType.Text;
                            sqlCommand.Parameters.AddWithValue("@ID", shiftId);
                            sqlCommand.Parameters.AddWithValue("@day", item.Date.ToString("dddd"));
                            connection.Open();
                            using (SqlDataReader dr = sqlCommand.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    scheduleTime = item.Date.Add(DateTime.Parse(dr[0].ToString()).TimeOfDay);
                                }
                            }
                            connection.Close();
                        }
                    }
                    // set time & send to queue
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand sqlCommand = new SqlCommand("insert into Hr.DataProcessQueue (EmployeeId,ScheduleTime,Flag,CreatedDate,CreatedBy,IsActive,IsDeleted,ProcessType) VALUES(@EmployeeId,@ScheduleTime,@Flag,@CreatedDate,@CreatedBy,@IsActive,@IsDeleted,@ProcessType)", connection))
                        {
                            sqlCommand.CommandType = CommandType.Text;
                            sqlCommand.Parameters.AddWithValue("@EmployeeId", item.EmployeeId);
                            sqlCommand.Parameters.AddWithValue("@ScheduleTime", scheduleTime);
                            sqlCommand.Parameters.AddWithValue("@Flag", 0);
                            sqlCommand.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                            sqlCommand.Parameters.AddWithValue("@CreatedBy", "System Process");
                            sqlCommand.Parameters.AddWithValue("@IsActive", 1);
                            sqlCommand.Parameters.AddWithValue("@IsDeleted", 0);
                            sqlCommand.Parameters.AddWithValue("@ProcessType", 10);
                            connection.Open();
                            sqlCommand.ExecuteNonQuery();
                            connection.Close();
                        }
                    }


                }

                //remove duplicate entries
                //using (SqlConnection connection = new SqlConnection(connectionString))
                //{
                //    using (SqlCommand sqlCommand = new SqlCommand("WITH cte AS (SELECT EmployeeId,ScheduleTime,row_number() OVER(PARTITION BY EmployeeId,ScheduleTime order by EmployeeId,ScheduleTime ) AS [rn] FROM hr.DataProcessQueue where flag=0)DELETE cte WHERE [rn] > 1", connection))
                //    {
                //        sqlCommand.CommandType = CommandType.Text;
                //        connection.Open();
                //        sqlCommand.ExecuteNonQuery();
                //        connection.Close();
                //    }
                //}

                flag = true;

            }
            catch (Exception e)
            {

            }
            return Json(flag);
        }



        #endregion Actions

        public IActionResult Manage()
        {
            return View();
        }


        public JsonResult groupbyAttendanceList(DateTime date,string param)
        {
            var employees = db.Employee.GetAllWithRelatedData(x=>x.IsActive==true && x.IsDeleted==false).ToList();
            var vm = new List<vmGroupbyAttendance>();

            switch (param)
            {
                case "department":
                    foreach (var item in employees.GroupBy(g=>g.DepartmentId))
                    {
                        var name = "";
                        var total = 0;
                        var absent = 0;
                        var present = 0;
                        var late = 0;
                        var business = 0;
                        var leave = 0;
                        name = item.Select(x => x.Department.Name).FirstOrDefault();
                        total = item.Select(x => x.Id).Count();
                        foreach (var employee in item)
                        {
                            //check leave
                            var isLeaveExists = db.LeaveApplication.GetFirstOrDefault(x =>x.EmployeeId==employee.Id && x.FromDate.Date == Today && x.Status == ApplicationStatus.Pending || x.Status == ApplicationStatus.Approved);
                            if(isLeaveExists != null)
                            {
                                leave+=1;
                                continue;
                            }
                            //check business
                            var isBusinessExists = db.BusinessApplication.GetFirstOrDefault(x => x.EmployeeId == employee.Id && x.FromDate.Date == Today && x.Status == ApplicationStatus.Pending || x.Status == ApplicationStatus.Approved);
                            if (isBusinessExists != null)
                            {
                                business += 1;
                                continue;
                            }
                            //check late
                            var lateCount = db.SystemPreference.GetFirstOrDefault().LateCount;
                            var shift = employee.Shift.ShiftDetailsList.Where(x => x.DayName == Today.ToString("dddd")).FirstOrDefault();
                            var officeStart = Today.Add(DateTime.Parse(shift.OfficeStartTime).TimeOfDay);
                            var officeStart_with_lateAllow = officeStart.AddMinutes(lateCount);
                            var punch = db.AttendanceMachineDataFiltered.GetAllAsQueryable().Where(x => x.EmployeeId == employee.Id && x.TransactionTime.Date == Today).OrderBy(o => o.TransactionTime).ToList().Take(1);
                            if(punch.Count()==0)
                            {
                                absent += 1;
                                continue;
                            }
                            else if(punch.FirstOrDefault().TransactionTime>officeStart_with_lateAllow)
                            {
                                late += 1;
                                present += 1;
                                continue;
                            }
                            else
                            {
                                present += 1;
                                continue;
                            }
                        }
                        vm.Add(new vmGroupbyAttendance { Absent = absent, Business = business, Late = late, Leave = leave, Present = present, Name = name, Total = total });
                    }
                    break;
                case "designation":
                    foreach (var item in employees.GroupBy(g => g.DesignationId))
                    {
                        var name = "";
                        var total = 0;
                        var absent = 0;
                        var present = 0;
                        var late = 0;
                        var business = 0;
                        var leave = 0;
                        name = item.Select(x => x.Designation.Name).FirstOrDefault();
                        total = item.Select(x => x.Id).Count();
                        foreach (var employee in item)
                        {
                            //check leave
                            var isLeaveExists = db.LeaveApplication.GetFirstOrDefault(x => x.EmployeeId == employee.Id && x.FromDate.Date == Today && x.Status == ApplicationStatus.Pending || x.Status == ApplicationStatus.Approved);
                            if (isLeaveExists != null)
                            {
                                leave += 1;
                                continue;
                            }
                            //check business
                            var isBusinessExists = db.BusinessApplication.GetFirstOrDefault(x => x.EmployeeId == employee.Id && x.FromDate.Date == Today && x.Status == ApplicationStatus.Pending || x.Status == ApplicationStatus.Approved);
                            if (isBusinessExists != null)
                            {
                                business += 1;
                                continue;
                            }
                            //check late
                            var lateCount = db.SystemPreference.GetFirstOrDefault().LateCount;
                            var shift = employee.Shift.ShiftDetailsList.Where(x => x.DayName == Today.ToString("dddd")).FirstOrDefault();
                            var officeStart = Today.Add(DateTime.Parse(shift.OfficeStartTime).TimeOfDay);
                            var officeStart_with_lateAllow = officeStart.AddMinutes(lateCount);
                            var punch = db.AttendanceMachineDataFiltered.GetAllAsQueryable().Where(x => x.EmployeeId == employee.Id && x.TransactionTime.Date == Today).OrderBy(o => o.TransactionTime).ToList().Take(1);
                            if (punch.Count() == 0)
                            {
                                absent += 1;
                                continue;
                            }
                            else if (punch.FirstOrDefault().TransactionTime > officeStart_with_lateAllow)
                            {
                                late += 1;
                                present += 1;
                                continue;
                            }
                            else
                            {
                                present += 1;
                                continue;
                            }
                        }
                        vm.Add(new vmGroupbyAttendance { Absent = absent, Business = business, Late = late, Leave = leave, Present = present, Name = name, Total = total });
                    }
                    break;
                case "shift":
                    foreach (var item in employees.GroupBy(g => g.ShiftId))
                    {
                        var name = "";
                        var total = 0;
                        var absent = 0;
                        var present = 0;
                        var late = 0;
                        var business = 0;
                        var leave = 0;
                        name = item.Select(x => x.Shift.Name).FirstOrDefault();
                        total = item.Select(x => x.Id).Count();
                        foreach (var employee in item)
                        {
                            //check leave
                            var isLeaveExists = db.LeaveApplication.GetFirstOrDefault(x => x.EmployeeId == employee.Id && x.FromDate.Date == Today && x.Status == ApplicationStatus.Pending || x.Status == ApplicationStatus.Approved);
                            if (isLeaveExists != null)
                            {
                                leave += 1;
                                continue;
                            }
                            //check business
                            var isBusinessExists = db.BusinessApplication.GetFirstOrDefault(x => x.EmployeeId == employee.Id && x.FromDate.Date == Today && x.Status == ApplicationStatus.Pending || x.Status == ApplicationStatus.Approved);
                            if (isBusinessExists != null)
                            {
                                business += 1;
                                continue;
                            }
                            //check late
                            var lateCount = db.SystemPreference.GetFirstOrDefault().LateCount;
                            var shift = employee.Shift.ShiftDetailsList.Where(x => x.DayName == Today.ToString("dddd")).FirstOrDefault();
                            var officeStart = Today.Add(DateTime.Parse(shift.OfficeStartTime).TimeOfDay);
                            var officeStart_with_lateAllow = officeStart.AddMinutes(lateCount);
                            var punch = db.AttendanceMachineDataFiltered.GetAllAsQueryable().Where(x => x.EmployeeId == employee.Id && x.TransactionTime.Date == Today).OrderBy(o => o.TransactionTime).ToList().Take(1);
                            if (punch.Count() == 0)
                            {
                                absent += 1;
                                continue;
                            }
                            else if (punch.FirstOrDefault().TransactionTime > officeStart_with_lateAllow)
                            {
                                late += 1;
                                present += 1;
                                continue;
                            }
                            else
                            {
                                present += 1;
                                continue;
                            }
                        }
                        vm.Add(new vmGroupbyAttendance { Absent = absent, Business = business, Late = late, Leave = leave, Present = present, Name = name, Total = total });
                    }
                    break;
            }
            return Json(vm);
        }


        public JsonResult JobCard(long id,DateTime month)
        {
            var FirstDay = new DateTime(month.Year, month.Month, 1);
            var LastDay = FirstDay.AddMonths(1).AddDays(-1);
            var jobcardList = new List<vmJobCard>();
            for (DateTime i = FirstDay; i <= LastDay; i = i.AddDays(1))
            {
                jobcardList.Add(new vmJobCard { 
                    Date = i.ToString("dd/MM/yyyy"), 
                    Flag = AttendanceFlag.A.ToString(), 
                    Month = i.ToString("MMMM yyyy"),
                  
                    Late="",
                    Remarks="",
                    WorkingHr=""
                });
            }

            var data = db.DailyAttendance.GetAllWithRelatedData(x => x.EmployeeId == id && x.TransactionDate >= FirstDay && x.TransactionDate <= LastDay).ToList();

            foreach (var item in data)
            {


                var value = jobcardList.FirstOrDefault(x => x.Date == item.TransactionDate.ToString("dd/MM/yyyy"));
                value.Flag = item.Flag.ToString(); 
                value.WorkingHr = Math.Round(item.WorkingHr, 2).ToString(); 
                value.Remarks = item.Remarks; 
                value.Late = item.Late;
                if(item.Employee.SpecialCase==true)
                {
                    value.Case = "Special";
                }
                else
                {
                    value.Case = "Regular";
                }
                value.Department = item.Employee.Department.Name;
                value.JobLocation = item.Employee.JobLocation.Name;
                value.Designation = item.Employee.Designation.Name;
                value.Phone = item.Employee.Phone;
                value.InTime = item.InTime;
                value.OutTime = item.OutTime;
                value.Early = item.Early;
                value.EmployeeId = item.Employee.MaskingId+" ("+item.Employee.AttendanceMachineId+")";
                value.Name = item.Employee.FullName;
                value.OverTime = Math.Round(item.OverTimeHr, 2).ToString();
                value.Shift = item.Employee.Shift.Name + " (" + item.Employee.Shift.ShiftDetailsList.FirstOrDefault().OfficeStartTime + "-" + item.Employee.Shift.ShiftDetailsList.FirstOrDefault().OfficeEndTime + ")";
            }

            return Json(jobcardList);
        }
        public JsonResult Details(long id, DateTime month)
        {
            return Json(true);
        }

    }
}