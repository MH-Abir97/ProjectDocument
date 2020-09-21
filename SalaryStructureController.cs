using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pronali.Data;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.Employee;
using Pronali.Web.Areas.HR.Models.SalaryStructure;
using Pronali.Web.Controllers;
using Pronali.Web.Helper;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    public class SalaryStructureController : BaseController
    {
        private readonly IUnitOfWork _db;
        private readonly IImagePath _imagePath;
        public SalaryStructureController(IUnitOfWork _unitOfWork, IImagePath imagePath) : base(_unitOfWork)
        {
            _db = _unitOfWork;
            _imagePath = imagePath;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetSalaryBreakup()
        {
            var data = db.SalaryBreakup.GetAll().ToList();
            return Json(data);
        }


        public JsonResult GetSalaryStructure(int id)
        {
            var vm = new VmEmployeeCreate();
            foreach (var item in db.SalaryStructureDetails.GetAllWithRelation().Where(x => x.SalaryStructureId == id))
            {
                vm.SalaryStructureList.Add(new vmEmployeeSalaryStructure
                {
                    SalaryGroup = item.SalaryBreakup.SalaryGroup.ToString(),
                    Amount = item.Amount,
                    Percentage = item.Percentage,
                    SalaryBreakupId = item.SalaryBreakupId,
                    SalaryBreakupName = item.SalaryBreakup.Name,
                    SalaryStructureId = item.SalaryStructureId,
                    SalaryStructureName = item.SalaryStructure.Name,
                    EffectiveFrom = DateTime.Now.ToString("yyyy/MM/dd")
                });
            }
            return Json(vm.SalaryStructureList);
        }


        [HttpGet]
        public IActionResult Create()
        {
           
            ViewBag.items = new SelectList(db.SalaryBreakup.GetAll(), "Id", "Name");
            vmSalaryStructureDetails salary = new vmSalaryStructureDetails();
            foreach (var item in db.SalaryStructure.GetAll())
            {
                salary.SalaryGradeList.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            
            return PartialView("Create", salary);
        }
        [HttpPost]
        public IActionResult Create(vmSalaryStructureDetails salaryStructure)
        {
           
            bool flag = false;
            if (ModelState.IsValid)
            {
               
                try
                {
                    var structure = new SalaryStructure
                    {
                        Name = salaryStructure.Name,

                    };
                    db.SalaryStructure.Add(structure);
                    db.Save();

                    SalaryStructureDetails details = new SalaryStructureDetails()
                    {
                        SalaryStructureId = structure.Id,

                        SalaryBreakupId = salaryStructure.SalaryBreakupId,
                        Amount = salaryStructure.Amount,
                        Percentage = salaryStructure.Percentage,
                    };              
                    db.SalaryStructureDetails.Add(details);
                    db.Save();
                    flag = true;
                    //employee salary
                    foreach (var item in salaryStructure.SalaryStructureList)
                    {
                        if (salaryStructure.SalaryGrade != null)
                        {
                            var salary = new EmployeeSalaryBase()
                            {
                                SalaryStructureId = salaryStructure.SalaryGrade.Value,
                                SalaryBreakupId = item.SalaryBreakupId,
                                Amount = item.Amount,
                                EffectiveFrom = DateTime.Now,
                                Remarks = item.Remarks,
                            };
                            db.EmployeeSalaryBase.Add(salary);
                            db.Save();
                        }

                    }
                }

                catch (Exception ex)
                {

                }


               

            }
            return Json(flag);
        }

    } 
}