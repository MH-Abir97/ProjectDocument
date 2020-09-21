using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Core;
using Pronali.Web.Areas.Core.Models.UserActivity;
using Pronali.Web.Controllers;

namespace Pronali.Web.Areas.Core.Controllers
{
    [Area("Core")]
    public class UserActivityController : BaseController
    {
        private IUnitOfWork _db;
        public UserActivityController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
            _db = _unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoadUserActivity()
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

            List<UserActivity>userActivityList = db.UserActivity.GetAll().Where(Model => Model.IsActive == true && Model.IsDeleted == false).ToList();
            List<VmUserActivity> userActivityItem = new List<VmUserActivity>();
            foreach (var item in userActivityList)
            {
                userActivityItem.Add(new VmUserActivity
                {
                    Id = item.Id,
                   Activities=item.Activities,
                   ActivitiesTime=item.ActivitiesTime,
                   ChangeActivitiesTime=item.ActivitiesTime.ToString("hh:mm:ss tt"),
                    Remarks =item.Remarks,
                });
            }


            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {

            }
            else
            {
                userActivityItem = userActivityItem.OrderByDescending(model => model.Id).ToList();
            }

            //Search
            if (!string.IsNullOrEmpty(searchValue))
            {
                userActivityItem = userActivityItem.Where(model => model.ActivitiesTime.ToShortDateString().Contains(searchValue) || model.Remarks.Contains(searchValue)).ToList();

            }


            //total number of rows count     
            recordsTotal = userActivityItem.Count();

            //Paging     
            var data = userActivityItem.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}