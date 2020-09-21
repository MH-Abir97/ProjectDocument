using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Core;
using Pronali.Web.Areas.Core.Models.Email;
using Pronali.Web.Controllers;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Pronali.Data.Enum;

namespace Pronali.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    public class EmailController : BaseController
    {
        private IUnitOfWork _db;
        public EmailController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
            _db = _unitOfWork;
        }

        public IActionResult Edit()
        {
            if(db.Email.Count() == 0)
            {
                db.Email.Add(new Email());
                db.Save();
            }

            var email = db.Email.GetFirstOrDefault(c => c.Id == c.Id);
            
            vmEmail vmEmail = new vmEmail();
            vmEmail.Id = email.Id;
            vmEmail.AccountName = email.AccountName;
            vmEmail.Description = email.Description;
            vmEmail.SenderAddress = email.SenderAddress;
            vmEmail.DisplayName = email.DisplayName;
            vmEmail.MailServerName = email.MailServerName;
            vmEmail.Port = email.Port;
            vmEmail.EnableSSL = email.EnableSSL;
            vmEmail.Username = email.Username;
            vmEmail.Password = email.Password;
            return PartialView("_Edit", vmEmail);
        }

        [HttpPost]
        public IActionResult Edit(vmEmail vmEmail)
        {
            if (ModelState.IsValid)
            {
                Email Email = new Email()
                {
                    Id = vmEmail.Id,
                    AccountName = vmEmail.AccountName,
                    Description = vmEmail.Description,
                    SenderAddress = vmEmail.SenderAddress,
                    DisplayName = vmEmail.DisplayName,
                    MailServerName = vmEmail.MailServerName,
                    Port = vmEmail.Port,
                    EnableSSL = vmEmail.EnableSSL,
                    Username = vmEmail.Username,
                    Password = vmEmail.Password,
                };
                _db.Email.Update(Email);
                bool isUpdated = _db.Save() > 0;
                return Json(vmEmail);
            }

            vmEmail.IsValid = false;
            vmEmail.ErrorMessage = "Validation Failed!. Please try Again with valid data.";
            return Json(vmEmail);
        }
    }
}