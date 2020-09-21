using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Controllers;
using Pronali.Web.Helper;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    public class SalaryBreakupController : BaseController
    {
            private readonly IUnitOfWork _db;
            private readonly IImagePath _imagePath;
            public SalaryBreakupController(IUnitOfWork _unitOfWork, IImagePath imagePath) : base(_unitOfWork)
            {
                _db = _unitOfWork;
                _imagePath = imagePath;
            }
            public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            SalaryBreakup salary = new SalaryBreakup();
            return PartialView("SBCreate", salary);
        }
        [HttpPost]
        public IActionResult Create(SalaryBreakup salaryBreakup)
        {

            if (ModelState.IsValid)
            {
                SalaryBreakup breakup = new SalaryBreakup()
                {
                    Name = salaryBreakup.Name,
                    SalaryGroup = salaryBreakup.SalaryGroup
                };
                db.SalaryBreakup.Add(breakup);
                db.Save();

                if (breakup.Id > 0)
                {
                    return Json(true);
                }

            }
            return Json(false);
        }
    }
}