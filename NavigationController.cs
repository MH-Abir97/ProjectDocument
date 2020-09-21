using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Web.Areas.Core.Models;
using Pronali.Web.Controllers;

namespace Pronali.Web.Areas.Core.Controllers
{
    [Area("Core")]
    //[Authorize]
    public class NavigationController : BaseController
    {
        public NavigationController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
        }
        public IActionResult Index()
        {
           
            return View();
        }

        public JsonResult Initialize()
        {
            var data = new vmNavigationPage
            {
                IncomeTaxTitle = " Income Tax",
                PromotionTitle = " Promotion",
                LeaveTitle = " Leave",
                HolidayTitle = " Holiday",
                ShiftTitle = " Shift",
                BankTitle = " Bank",
                SisterConcernTitle = " Sister Concern",
                DivisionTitle = " Division",
                BranchTitle = " Branch",
                CompanyTitle = " Company",
                DepartmentTitle = " Department",
                DesignationTitle = " Designation",
                EmailTitle = " Email",
                FloorTitle = " Floor",
                LineTitle = " Line",
                SectionTitle = " Section",
                SMSTitle = " SMS"

            };
            return Json(data);
        }

        public JsonResult GetSmsBalance()
        {
            return Json(db.Common.getSmsBalance());
        }


    }
}