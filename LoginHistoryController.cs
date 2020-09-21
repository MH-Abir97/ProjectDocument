using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models;
using Pronali.Data.Models.Entity.Core;
using Pronali.Web.Areas.Core.Models.LogginHistory;
using Pronali.Web.Controllers;
using Pronali.Web.Extension;
using Pronali.Web.Helper;

namespace Pronali.Web.Areas.Core.Controllers
{
    [Area("Core")]
    public class LoginHistoryController : BaseController
    {
        private readonly IImagePath _imagePath;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _env;

        public LoginHistoryController(IUnitOfWork _unitOfWork, IImagePath imagePath, UserManager<ApplicationUser> userManager, IHostingEnvironment env) : base(_unitOfWork)
        {
            _imagePath = imagePath;
            _userManager = userManager;
            _env = env;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoadLoginHistory()
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

           // var loggedInEmployeeId = User.GetCurrentEmployeeId(db.Employee);
         
            List<LoginHistory> loginHistoryList = db.LoginHistory.GetAll().Where(Model => Model.IsActive == true && Model.IsDeleted == false).ToList(); 

            List<vmLogin> loginHistoryItem = new List<vmLogin>();
            foreach (var item in loginHistoryList)
            {
                var history =new vmLogin
                {
                    Id = item.Id,
                    UserId = item.UserId,
                    ChangeLoginTime = item.LoginTime.ToString("hh:mm:ss tt"),
                    Details = item.Details,
                    
                };

                var employee = db.Employee.GetFirstOrDefault(x => x.UserId == history.UserId);
                if(employee!=null)
                {
                    //history.UserId = employee.UserId;
                    history.UserName=employee.FullName;

                }

                loginHistoryItem.Add(history);

            }


            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
              
            }
            else
            {
                loginHistoryItem = loginHistoryItem.OrderByDescending(model => model.Id).ToList();
            }

            //Search
            if (!string.IsNullOrEmpty(searchValue))
            {
                loginHistoryItem = loginHistoryItem.Where(model => model.LoginTime.ToShortDateString().Contains(searchValue) || model.UserId.Contains(searchValue)).ToList();

            }


            //total number of rows count     
            recordsTotal = loginHistoryItem.Count();

            //Paging     
            var data = loginHistoryItem.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}