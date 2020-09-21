using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.EmployeeFile;
using Pronali.Web.Controllers;
using Pronali.Web.Helper;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    public class EmployeFileController : BaseController
    {
        private readonly IImagePath _imagePath;

        public EmployeFileController(IUnitOfWork _unitOfWork, IImagePath imagePath) : base(_unitOfWork)
        {
            _imagePath = imagePath;
        }
        public IActionResult Index( )
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateView()
        {
            //ViewBag.itemsTime = DateTime.Now.ToShortDateString();
            vmEmployeeFile employeeFile = new vmEmployeeFile();
            return PartialView("Create", employeeFile);
        }       
        [HttpPost]
        public IActionResult Create(vmEmployeeFile employeeFile)
        {

            if (ModelState.IsValid)
            {
                EmployeFile file = new EmployeFile()
                {
                    EmployeeId = employeeFile.EmployeeId,
                    FileUrl = employeeFile.FileUrl,
                    Remarks = employeeFile.Remarks
                };
                db.EmployeFile.Add(file);
                db.Save();
                ModelState.Clear();

                if (file.Id > 0)
                {
                    return Json(true);
                }

            }
            return Json(false);
        }
    }
}