using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.Notifaction;
using Pronali.Web.Controllers;
using Pronali.Web.Helper;

namespace Pronali.Web.Areas.Core.Controllers
{
    [Area("Core")]
   // [Authorize]
    public class NotificationController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IImagePath _imagePath;

        public NotificationController(IUnitOfWork _unitOfWork, UserManager<ApplicationUser> userManager, IImagePath imagePath) : base(
            _unitOfWork)
        {
            _userManager = userManager;
            _imagePath = imagePath;
        }

        public async Task<IActionResult> LoadNotification()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var notification = db.Notification.GetAll().Where(x => x.SendTo == currentUser.Id).ToList();
            return Json(notification);
        }

        public async Task<IActionResult> Read(long id)
        {
            bool flag = false;
            try
            {
                var notification = db.Notification.GetFirstOrDefault(x => x.Id == id && x.IsRead == false);
                if (notification != null)
                {
                    notification.IsRead = true;
                    notification.ReadingTime = DateTime.Now.ToString();
                    db.Notification.Update(notification);
                    db.Save();
                    flag = true;
                }

            }
            catch (Exception e)
            {

            }

            return Json(flag);

        }
        public IActionResult Index()
        {

            return View();
        }
        public IActionResult LoadNotifaction()
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

            List<Notification> notifactionlList = db.Notification.GetAll().ToList();

            List<vmNotification> notifactionlItem = new List<vmNotification>();
            foreach (var item in notifactionlList)
            {

                string photoURL = "";
                if (!string.IsNullOrEmpty(item.Avatar))
                {
                    photoURL = _imagePath.GetFilePathAsSourceUrl(item.Avatar);
                }
                else
                {
                    photoURL = _imagePath.GetFilePathAsSourceUrl("/images/Uploads/Employee/AlterImage.png");
                }
                notifactionlItem.Add(new vmNotification
                {
                    Id = item.Id,
                    Avatar= photoURL,
                    Title = item.Title,
                    Details =item.Details,
                    SendTo = item.SendTo,
                    ReceivedFrom = item.ReceivedFrom,
                    ReadingTime = item.ReadingTime,
                    IsRead = item.IsRead,
                    Url=item.Url,
                });
            }

            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {

            }
            else
            {
                notifactionlItem = notifactionlItem.OrderByDescending(model => model.Id).ToList();
            }

            //Search
            if (!string.IsNullOrEmpty(searchValue))
            {
                notifactionlItem = notifactionlItem.Where(model => model.Title.Contains(searchValue) || model.SendTo.Contains(searchValue) || model.ReceivedFrom.Contains(searchValue) || model.ReadingTime.Contains(searchValue)|| model.Avatar.Contains(searchValue)).ToList();

            }

            //total number of rows count     
            recordsTotal = notifactionlItem.Count();

            //Paging     
            var data = notifactionlItem.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


    }
}