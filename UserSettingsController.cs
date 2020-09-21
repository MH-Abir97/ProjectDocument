using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Enum;
using Pronali.Data.Models;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.Employee;
using Pronali.Web.Controllers;
using Pronali.Web.Extension;
using Pronali.Web.Helper;
using System.Drawing;
using Pronali.Data.Helper;
using Pronali.Web.Areas.Core.Models.UserSettings;

namespace Pronali.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    public class UserSettingsController : BaseController
    {
        private readonly IImagePath _imagePath;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _env;

        public UserSettingsController(IUnitOfWork _unitOfWork, IImagePath imagePath, UserManager<ApplicationUser> userManager, IHostingEnvironment env) : base(_unitOfWork)
        {
            _imagePath = imagePath;
            _userManager = userManager;
            _env = env;
            Now = BaseHelper.UserGeo().UserDateTime;
            Today = Now.Date;
            Tommorow = Today.AddDays(1);
            FirstDayOfTheMonth = new DateTime(Today.Year, Today.Month, 1);
            LastDayOfTheMonth = FirstDayOfTheMonth.AddMonths(1).AddDays(-1);
        }

        public IActionResult Index()
        {
            var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            var result = db.Employee.GetFirstOrDefaultWithRelatedData( e => e.Id == loggedInEmployeeId );
            if (result != null)
            {
                vmUserProfile userProfile = new vmUserProfile();
                userProfile.FullName = result.FullName;
                if (result.JobLocation != null)
                {
                    userProfile.JobLocation = result.JobLocation.Name;
                }
                if (result.Department != null)
                {
                    userProfile.DepartmentName = result.Department.Name;
                }
                if (result.Designation != null)
                {
                    userProfile.Designation = result.Designation.Name;
                }
                if (result.Shift != null)
                {
                    userProfile.ShiftName = result.Shift.Name;
                }

                userProfile.FatherName = result.FatherName;
                userProfile.MotherName = result.MotherName;
                userProfile.PresentAddress = result.PresentAddress;

                userProfile.FirstName = result.FirstName;
                userProfile.LastName = result.LastName;
                userProfile.Phone = result.Phone;
                userProfile.Email = result.Email;
                userProfile.JoiningDate = result.JoinDate.ToShortDateString();
                userProfile.DateOfBirth = result.DateofBirth;
                if (result.PhotoUrl != null)
                {
                    userProfile.PhotoUrl = _imagePath.GetFilePathAsSourceUrl(result.PhotoUrl);
                }
                else
                {
                    userProfile.PhotoUrl = _imagePath.GetFilePathAsSourceUrl("/images/Uploads/Employee/AlterImage.png");
                }

                return View(userProfile);
            }
            else
            {
                return RedirectToAction("current","Employee",new{Area="HR"});
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveInformation( vmUserProfile vmEmployeeCreate )
        {
            if(ModelState.IsValid)
            {
                var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
                var result = db.Employee.Get(loggedInEmployeeId);
                if( result != null)
                {
                    result.FirstName = vmEmployeeCreate.FirstName == null ? result.FirstName : vmEmployeeCreate.FirstName;
                    result.LastName = vmEmployeeCreate.LastName == null ? result.LastName : vmEmployeeCreate.LastName;
                    result.Phone = vmEmployeeCreate.Phone == null ? result.Phone : vmEmployeeCreate.Phone;
                    result.Email = vmEmployeeCreate.Email == null ? result.Email : vmEmployeeCreate.Email;
                    result.FatherName = vmEmployeeCreate.FatherName == null ? result.FatherName : vmEmployeeCreate.FatherName;
                    result.MotherName = vmEmployeeCreate.MotherName == null ? result.MotherName : vmEmployeeCreate.MotherName;
                    result.PresentAddress = vmEmployeeCreate.PresentAddress == null ? result.PresentAddress : vmEmployeeCreate.PresentAddress;
                    db.Employee.Update(result);
                    db.Save();

                    var currentUser = await _userManager.GetUserAsync(HttpContext.User);
                    
                    currentUser.PhoneNumber = result.Phone;
                    currentUser.Email = result.Email;
                    currentUser.EmailConfirmed = true;
                    currentUser.PhoneNumberConfirmed = true;
                    var updateUser = await _userManager.UpdateAsync(currentUser);
                    return Json(true);
                }
            } 
            return Json(false);
        }

        [HttpPost]
        public async Task<IActionResult> SavePhoto(IFormFile file)
        {
            bool flag = false;
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser.UserType == UserType.Employee || currentUser.UserType == UserType.Admin)
            {
                var employee = db.Employee.GetFirstOrDefault(x => x.UserId == currentUser.Id);
                var filePath = await _imagePath.SaveToFolderAndReturnPathForEmployee(file, employee.MaskingId);
                employee.PhotoUrl = filePath;
                db.Employee.Update(employee);
                db.Save();
                flag = true;
            }
            

            return Json(flag);
        }

        [HttpPost]
        public async Task<IActionResult> SavePassword( string oldPassword,string newPassword)
        {
            var flag = false;
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var result = await _userManager.ChangePasswordAsync(currentUser, oldPassword, newPassword);


            if (result.Succeeded)
            {
                flag = true;
            }

            return Json( flag);
        }


        public async Task<IActionResult> UserPhoto()
        {
            string photourl = "images/Uploads/Employee/AlterImage.png";

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser.UserType == UserType.SuperAdmin)
            {
                var imageBytes = _imagePath.GetImageFromUrl(photourl);
                return File(imageBytes, "image/jpeg");
            }
            else if (currentUser.UserType == UserType.Admin || currentUser.UserType == UserType.Employee)
            {
                var loggedinEmployeeId = User.GetCurrentEmployeeId(db.Employee);
                var employee = db.Employee.GetFirstOrDefault(x => x.Id == loggedinEmployeeId);
                if (!string.IsNullOrEmpty(employee.PhotoUrl))
                {
                    var imageBytes = _imagePath.GetImageFromUrl(employee.PhotoUrl);
                    return File(imageBytes, "image/jpeg");
                }
                else
                {
                    var imageBytes = _imagePath.GetImageFromUrl(photourl);
                    return File(imageBytes, "image/jpeg");
                }
            }
            else
            {
                var imageBytes = _imagePath.GetImageFromUrl(photourl);
                return File(imageBytes, "image/jpeg");
            }
        }

        public async Task<IActionResult> GetInfo()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var data = new { TwofactorbySms =currentUser.TwoFactorEnabled};
            return Json(data);
        }
        public async Task<IActionResult> SetTwoFactorbySms(bool flag)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            currentUser.TwoFactorEnabled = flag;
            var result = await _userManager.UpdateAsync(currentUser);
            return Json(flag);
        }
     
        public JsonResult GetUserSummary()
        {
            var loggedinEmployeeId = User.GetCurrentEmployeeId(db.Employee);
            var dayname = DateTime.Now.ToString("dddd");
            var employee = db.Employee.GetFirstOrDefaultWithRelatedData(x => x.IsActive == true && x.IsDeleted==false);
            var model = new vmUserDocker();

            var shift = employee.Shift.ShiftDetailsList.FirstOrDefault(x => x.DayName == dayname);
            model.Shift = shift.Shift.Name + "(" + shift.OfficeStartTime+"-"+shift.OfficeEndTime+")";
            foreach (var item in employee.Weekend)
            {
                if (!string.IsNullOrEmpty(model.Weekend))
                {
                    model.Weekend += ", ";
                }
                model.Weekend += item.Dayname;
            }
            foreach (var item in employee.EmployeeLeaveList)
            {
                model.LeaveBalance += item.Leave.Flag+":"+item.Balance + "/" + item.Allocate+" ";
            }

            foreach (var item in employee.Holidays.Where(x=>x.Holiday.From<DateTime.Now.AddDays(30)))
            {
                string duration = item.Holiday.From.ToString("dd MMMM");
                if (item.Holiday.From != item.Holiday.To)
                {
                    duration += "-"+item.Holiday.To.ToString("dd MMMM");
                }
                if (!string.IsNullOrEmpty(model.NextHolidays))
                {
                    model.NextHolidays += ", ";
                }
                model.NextHolidays += duration;
            }

            var workingHr = db.AttendanceProcessedData.GetAll().Where(x=>x.InTime.Date==Today).Sum(s=>s.WorkingHr);
            var officehr = Today.Date.Add(DateTime.Parse(shift.OfficeEndTime).TimeOfDay)- Today.Date.Add(DateTime.Parse(shift.OfficeStartTime).TimeOfDay);
            if (workingHr > officehr.TotalHours)
            {
                workingHr = officehr.TotalHours;
            }
            model.JobHour = Math.Round(workingHr, 2) + "/"+officehr.TotalHours;
            return Json(model);

        }
    }
}