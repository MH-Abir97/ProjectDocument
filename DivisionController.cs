using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pronali.Data;
using Pronali.Data.Models.Entity.Core;
using Pronali.Web.Areas.Core.Models.Division;
using Pronali.Web.Controllers;
using Pronali.Web.Areas.Core.Models.SisterConcern;

namespace Pronali.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    public class DivisionController : BaseController
    {
        private IUnitOfWork _db;

        public DivisionController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
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
            ViewData["CompanyId"] = new SelectList(_db.Company.GetAll().Where(c => c.IsActive == true && c.IsDeleted == false), "Id", "CompanyName");
            ViewData["SisterConcernId"] = new SelectList(_db.SisterConcern.GetAll().Where(c => c.IsActive == true && c.IsDeleted == false), "Id", "Name");
            //IEnumerable<SisterConcern> sisterConcern = _db.SisterConcern.GetAll();
            vmDivision vmDivision = new vmDivision();
            return PartialView("_Create", vmDivision);
        }

        [HttpPost]
        public IActionResult Create(vmDivision vmDivision)
        {
            if (ModelState.IsValid)
            {
                Division division = new Division()
                {
                    Name = vmDivision.Name,
                    CompanyId = vmDivision.CompanyId,
                    SisterConcernId = vmDivision.SisterConcernId,
                    Address = vmDivision.Address,
                    Email = vmDivision.Email,
                    Phone = vmDivision.Phone,
                    IsActive = true,
                    IsDeleted = false
                };
                _db.Division.Add(division);
                _db.Save();
                return Json(division);
            }
            return Json(vmDivision);
        }


        //Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewData["CompanyId"] = new SelectList(_db.Company.GetAll().Where(c => c.IsActive == true && c.IsDeleted == false), "Id", "CompanyName");
            ViewData["SisterConcernId"] = new SelectList(_db.SisterConcern.GetAll().Where(c => c.IsActive == true && c.IsDeleted == false), "Id", "Name");

            Division division = _db.Division.Get(id);
            vmDivision vmDivision = new vmDivision();
            vmDivision.Id = division.Id;
            vmDivision.Name = division.Name;
            vmDivision.SisterConcernId = division.SisterConcernId;
            vmDivision.CompanyId = division.CompanyId;
            vmDivision.Phone = division.Phone;
            vmDivision.Email = division.Email;
            vmDivision.Address = division.Address;
            return PartialView("_Edit", vmDivision);
        }

        [HttpPost]
        public IActionResult Edit(vmDivision vmDivision)
        {
            if (ModelState.IsValid)
            {
                Division division = _db.Division.GetFirstOrDefault(c => c.Id == vmDivision.Id);

                division.Name = vmDivision.Name;
                division.SisterConcernId = vmDivision.SisterConcernId;
                division.CompanyId = vmDivision.CompanyId;
                division.Phone = vmDivision.Phone;
                division.Email = vmDivision.Email;
                division.Address = vmDivision.Address;

                _db.Division.Update(division);
                _db.Save();
                return Json(vmDivision);
            }
            return Json(vmDivision);
        }

        [HttpPost]
        public IActionResult Delete(long id)
        {
            Division division = _db.Division.GetFirstOrDefault(c => c.Id == id);
            division.IsActive = false;
            division.IsDeleted = false;
            _db.Division.Update(division);
            _db.Save();
            return Json("Success");
        }
        public IActionResult LoadDivision()
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

            List<Division> division = _db.Division.GetAllWithRelatedData(d => d.IsActive == true && d.IsDeleted == false).ToList();

            var divisionList = new List<vmDivision>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                division = division.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                division = division.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                division = division.Where(x => x.Name.Contains(searchValue)).ToList();
            }

            foreach (var item in division)
            {
                divisionList.Add(new vmDivision
                {
                    Id = item.Id,
                    Name = item.Name,
                    Phone = item.Phone,
                    Email = item.Email,
                    Address = item.Address,
                    SisterConcern = new vmSisterConcern
                    {
                        Name = item.SisterConcern.Name
                    },
                    Company = new Models.vmCompany
                    {
                        CompanyName = item.Company.CompanyName
                    },
                    CreatedDate = item.CreatedDate
                });
            }

            divisionList = divisionList.OrderByDescending(i => i.CreatedDate.Date)
                .ThenByDescending(i => i.CreatedDate.TimeOfDay).ToList();
            //total number of rows count     
            recordsTotal = divisionList.Count();

            //Paging     
            var data = divisionList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

    }
}