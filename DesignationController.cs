using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Core;
using Pronali.Web.Areas.Core.Models.Designation;
using Pronali.Web.Controllers;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;

namespace Pronali.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    public class DesignationController :BaseController
    {
        private IUnitOfWork _db;
        public DesignationController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
            _db = _unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            vmDesignation vmDesignation = new vmDesignation();
            return PartialView("_CreateView", vmDesignation);
        }


        [HttpPost]
        public IActionResult Create(vmDesignation vmDesignation)
        {
            if (ModelState.IsValid)
            {
                Designation designation = new Designation()
                {
                    Name = vmDesignation.Name,
                    IsActive = true,
                    IsDeleted = false
                };
                _db.Designation.Add(designation);
                _db.Save();
                return Json(vmDesignation);
            }

            vmDesignation.IsValid = false;
            vmDesignation.Message = "Validation Failed!. Please try Again with valid data.";
            return Json(vmDesignation);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var designation = _db.Designation.Get(id);
            vmDesignation vmDesignation = new vmDesignation();
            vmDesignation.Id = designation.Id;
            vmDesignation.Name = designation.Name;
            return PartialView("_EditView", vmDesignation);
        }

        [HttpPost]
        public IActionResult Edit(vmDesignation vmDesignation)
        {
            if (ModelState.IsValid)
            {
                Designation designation = _db.Designation.GetFirstOrDefault(c => c.Id == vmDesignation.Id);

                designation.Name = vmDesignation.Name;

                _db.Designation.Update(designation);
                _db.Save();
                return Json(vmDesignation);
            }
            vmDesignation.IsValid = false;
            vmDesignation.Message = "Validation Failed!. Please try Again with valid data.";
            return Json(vmDesignation);
        }

        [HttpPost]
        public IActionResult Delete(vmDesignation vmDesignation)
        {
            if (ModelState.IsValid)
            {
                Designation designation = _db.Designation.GetFirstOrDefault(c => c.Id == vmDesignation.Id);

                designation.IsActive = false;
                designation.IsDeleted = false;
                _db.Designation.Update(designation);
                _db.Save();
                //_db.Designation.Remove(designation);
                return Json(vmDesignation);
            }
            vmDesignation.IsValid = false;
            vmDesignation.Message = "Validation Failed!. Please try Again with valid data.";
            return Json(vmDesignation);
        }

        public IActionResult LoadDesignation()
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

            var designation = _db.Designation.GetAll().Where(d => d.IsActive == true && d.IsDeleted == false);

            var designationList = new List<vmDesignation>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                designation = designation.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                designation = designation.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                designation = designation.Where(x => x.Name.Contains(searchValue)).ToList();
            }

            foreach (var item in designation)
            {
                designationList.Add(new vmDesignation
                {
                    Id = item.Id,
                    Name = item.Name,
                    CreatedDate = item.CreatedDate
                });
            }

            designationList = designationList.OrderByDescending(i => i.CreatedDate.Date)
                .ThenByDescending(i => i.CreatedDate.TimeOfDay).ToList();
            //total number of rows count     
            recordsTotal = designationList.Count();

            //Paging     
            var data = designationList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        public JsonResult IsExist(string name)
        {
            var isFound = _db.Designation.GetFirstOrDefault(c => c.Name == name && c.IsActive == true && c.IsDeleted == false);
            return Json(isFound);
        }
    }
}