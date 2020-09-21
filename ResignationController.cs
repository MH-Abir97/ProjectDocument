using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Enum;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.Resignation;
using Pronali.Web.Controllers;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    public class ResignationController : BaseController
    {
        public ResignationController(IUnitOfWork _unitOfWorks): base(_unitOfWorks)
        {
        }
        public IActionResult Index()
        {
            return View("Index");
        }
        public IActionResult Create()
        {
            return View("Create");
        }

        [HttpPost]
        public IActionResult Create(vmResignation vmResignation)
        {
            if (ModelState.IsValid)
            {
                Resignation resignation = new Resignation()
                {
                    IntendedDate = Convert.ToDateTime(vmResignation.IntendedDate),
                    Reason = vmResignation.Reason,
                    Status = 0,
                    //EmployeeId = 3
                };
                db.Resignation.Add(resignation);
                db.Save();
            }
            ModelState.Clear();
            return View("Create");
        }

        public IActionResult Edit(long id)
        {
            Resignation resignation = db.Resignation.GetFirstOrDefault(s => s.Id == id);
            vmResignation vmResignation = new vmResignation()
            {
                Id = resignation.Id,
                IntendedDate = resignation.IntendedDate.ToString("dd MMMM, yyyy"),
                Reason = resignation.Reason,
                Status = resignation.Status,
                CreatedDate = resignation.CreatedDate
            };
            return PartialView("_Edit", vmResignation);
        }

        [HttpPost]
        public IActionResult Edit(vmResignation vmResignation)
        {
            if (ModelState.IsValid)
            {
                Resignation resignation = db.Resignation.GetFirstOrDefault(r => r.Id == vmResignation.Id);
                resignation.Status = vmResignation.Status;
                db.Resignation.Update(resignation);
                db.Save();
            }
            return View("Index");
        }

        public IActionResult Delete(long id)
        {
            return Json("Success");
        }

        //[HttpPost]
        //public IActionResult ChangeStatus(List<Resignation> resignations)
        //{
        //    return Json("Success");
        //}

        public IActionResult LoadResignation()
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

            var resignations = db.Resignation.GetAllWithRelatedData(d => d.IsActive == true && d.IsDeleted == false);

            var resignationList = new List<vmResignation>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                resignations = resignations.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                resignations = resignations.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            //if (!string.IsNullOrEmpty(searchValue))
            //{
            //    resignations = resignations.Where(x => x.Name.Contains(searchValue)).ToList();
            //}

            foreach (var item in resignations)
            {
                resignationList.Add(new vmResignation
                {
                    Id = item.Id,
                    Employee = item.Employee,
                    EmployeeId = item.EmployeeId,
                    IntendedDate = item.IntendedDate.ToString("dd MMMM, yyyy"),
                    Reason = item.Reason,
                    ApplicationStatus = item.Status.ToString(),
                    Status = item.Status
            });
            }

            //total number of rows count     
            recordsTotal = resignationList.Count();

            //Paging     
            var data = resignationList.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

    }
}