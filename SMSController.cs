using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pronali.Data;
using Pronali.Data.Enum;
using Pronali.Data.Models.Entity.Core;
using Pronali.Web.Areas.Core.Models.SMS;
using Pronali.Web.Controllers;

namespace Pronali.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    public class SMSController : BaseController
    {

        private readonly IUnitOfWork _db;

        public SMSController(IUnitOfWork _unitOfWork) : base (_unitOfWork)
        {
            _db = _unitOfWork;
        }



        [HttpGet]
        public IActionResult Edit()
        {
            if (_db.SMS.Count() == 0)
            {
                _db.SMS.Add(new SMS());
                _db.Save();
            }

            var sms = _db.SMS.GetFirstOrDefault(c => c.Id == c.Id);

            SmsVm smsVm = new SmsVm();
            smsVm.Id = sms.Id;
            smsVm.BaseAddress = sms.BaseAddress;
            smsVm.UserName = sms.UserName;
            smsVm.SenderNumber = sms.SenderNumber;
            smsVm.SenderType = sms.SenderType;
            smsVm.SmsType = sms.SmsType;
            smsVm.SmsSenderTypeList.Add(new SelectListItem() { Text = SmsSenderType.NonMasking.ToString(), Value = SmsSenderType.NonMasking.ToString() });
            smsVm.SmsSenderTypeList.Add(new SelectListItem() { Text = SmsSenderType.Masking.ToString(), Value = SmsSenderType.Masking.ToString() });
            smsVm.SmsTypeList.Add(new SelectListItem() { Text = SmsType.Text.ToString(), Value = SmsType.Text.ToString() });
            smsVm.SmsTypeList.Add(new SelectListItem() { Text = SmsType.Unicode.ToString(), Value = SmsType.Unicode.ToString() });

            return PartialView("_Edit", smsVm);
        }


        [HttpPost]
        public IActionResult Edit(SmsVm SmsVm)
        {
            SmsVm.SmsSenderTypeList.Add(new SelectListItem() { Text = SmsSenderType.NonMasking.ToString(), Value = SmsSenderType.NonMasking.ToString() });
            SmsVm.SmsSenderTypeList.Add(new SelectListItem() { Text = SmsSenderType.Masking.ToString(), Value = SmsSenderType.Masking.ToString() });
            SmsVm.SmsTypeList.Add(new SelectListItem() { Text = SmsType.Text.ToString(), Value = SmsType.Text.ToString() });
            SmsVm.SmsTypeList.Add(new SelectListItem() { Text = SmsType.Unicode.ToString(), Value = SmsType.Unicode.ToString() });
            if (ModelState.IsValid)
            {
                SMS SMS = new SMS()
                {
                    Id = SmsVm.Id,
                    BaseAddress = SmsVm.BaseAddress,
                    UserName = SmsVm.UserName,
                    SenderNumber = SmsVm.SenderNumber,
                    SenderType = SmsVm.SenderType,
                    SmsType = SmsVm.SmsType,
                };
                _db.SMS.Update(SMS);
                _db.Save();
                
                return Json(SmsVm);
            }

            SmsVm.IsValid = false;
            SmsVm.Message = "Validation Failed!. Please try Again with valid data.";
            return Json(SmsVm);
        }

    }
}