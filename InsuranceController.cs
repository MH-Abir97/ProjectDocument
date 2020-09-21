using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.Insurance;
using Pronali.Web.Controllers;
using System.Linq.Dynamic.Core;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class InsuranceController : BaseController
    {
        public InsuranceController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
        }
        public IActionResult Create()
        {
            return PartialView("_Create");
        }

        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        public IActionResult Create(vmInsurance vmInsurance)
        {
            if(ModelState.IsValid)
            {
                Insurance insurance = new Insurance()
                {
                    Name = vmInsurance.Name,
                    Description = vmInsurance.Description,
                    Amount = vmInsurance.Amount,
                    EffectiveFrom = Convert.ToDateTime(vmInsurance.EffectiveFrom),
                    EligibleEmployee = vmInsurance.EligibleEmployee,
                    Expiry = Convert.ToDateTime(vmInsurance.Expiry)
                };
                db.Insurance.Add(insurance);
                db.Save();
            }
            return View("Index");
        }

        public IActionResult Edit(int id)
        {
            Insurance insurance = db.Insurance.GetFirstOrDefault(i => i.Id == id);
            vmInsurance vmInsurance = new vmInsurance()
            {
                Id = insurance.Id,
                Name = insurance.Name,
                Description = insurance.Description,
                Amount = insurance.Amount,
                EffectiveFrom = insurance.EffectiveFrom.ToString("dd MMMM, yyyy"),
                EligibleEmployee = insurance.EligibleEmployee,
                Expiry = insurance.Expiry?.ToString("dd MMMM, yyyy")
            };
            return View("Edit",vmInsurance);
        }

        [HttpPost]
        public IActionResult Edit(vmInsurance vmInsurance)
        {
            if (ModelState.IsValid)
            {
                Insurance insurance = db.Insurance.GetFirstOrDefault(i => i.Id == vmInsurance.Id);
                insurance.Name = vmInsurance.Name;
                insurance.Description = vmInsurance.Description;
                insurance.Amount = vmInsurance.Amount;
                insurance.EffectiveFrom = Convert.ToDateTime(vmInsurance.EffectiveFrom);
                insurance.EligibleEmployee = vmInsurance.EligibleEmployee;
                insurance.Expiry = Convert.ToDateTime(vmInsurance.Expiry);
                db.Insurance.Update(insurance);
                db.Save();
            }
            return View("Edit");
        }


        public IActionResult LoadInsurance()
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

            List<Insurance> insurance = db.Insurance.GetAll().Where(d => d.IsActive == true && d.IsDeleted == false).ToList();
            

            var insuranceList = new List<vmInsurance>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                insurance = insurance.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                insurance = insurance.OrderByDescending(x => x.Id).ToList();
            }

            //Search
            if (!string.IsNullOrEmpty(searchValue))
            {
                insurance = insurance.Where(x => x.Name.Contains(searchValue)).ToList();
            }

            foreach (var item in insurance)
            {
                insuranceList.Add(new vmInsurance
                {
                    Id = item.Id,
                    Name = item.Name,
                    EligibleEmployee = item.EligibleEmployee,
                    EffectiveFrom = item.EffectiveFrom.ToString("dd MMMM, yyyy"),
                    Expiry = item.Expiry?.ToString("dd MMMM, yyyy"),
                    Description = item.Description,
                    CreatedDate = item.CreatedDate
                });
            }
            
            insuranceList = insuranceList.OrderByDescending(i => i.CreatedDate.Date)
                .ThenByDescending(i => i.CreatedDate.TimeOfDay).ToList();
        //total number of rows count     
        recordsTotal = insuranceList.Count();

            //Paging     
            var data = insuranceList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        public IActionResult Delete(long id)
        {
            Insurance insurance = db.Insurance.GetFirstOrDefault(i => i.Id == id);
            insurance.IsActive = false;
            insurance.IsDeleted = true;
            db.Insurance.Update(insurance);
            db.Save();
            return View("Index");
        }
    }
}