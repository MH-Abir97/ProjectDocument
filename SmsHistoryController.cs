using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Core;
using Pronali.Web.Areas.Core.Models.SmsHistory;
using Pronali.Web.Controllers;
namespace Pronali.Web.Areas.Core.Controllers
{
    [Area("Core")]
    public class SmsHistoryController : BaseController
    {
        private IUnitOfWork _db;
        public SmsHistoryController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
            _db = _unitOfWork;
        }

        public IActionResult Index()
        {

            return View();
        }
        public IActionResult LoadSmsHistory()
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

            List<SmsHistory> smsHistoryList = db.SmsHistory.GetAll().Where(Model => Model.IsActive == true && Model.IsDeleted == false).ToList();
            List<vmSmsHistory> smsHistoryItem = new List<vmSmsHistory>();
            foreach (var item in smsHistoryList)
            {
                smsHistoryItem.Add(new vmSmsHistory
                {
                    Id = item.Id,
                    Charge = item.Charge,
                    SendingTime = item.SendingTime,
                    ChangeSendingTime = item.SendingTime.ToString("hh:mm:ss tt"),
                    SmsCount = item.SmsCount,
                    Status = item.Status,
                    Title = item.Title
                }) ;
            }

            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {

            }
            else
            {
                smsHistoryItem = smsHistoryItem.OrderByDescending(model => model.Id).ToList();
            }

            //Search
            if (!string.IsNullOrEmpty(searchValue))
            {
                smsHistoryItem = smsHistoryItem.Where(model => model.SendingTime.ToShortDateString().Contains(searchValue) || model.Title.Contains(searchValue)).ToList();

            }


            //total number of rows count     
            recordsTotal = smsHistoryItem.Count();

            //Paging     
            var data = smsHistoryItem.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}