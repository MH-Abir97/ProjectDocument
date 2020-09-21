using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pronali.Data;
using Pronali.Data.Models.Entity.Core;
using Pronali.Web.Areas.Core.Models.Floor;
using Pronali.Web.Controllers;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;

namespace Pronali.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    public class FloorController : BaseController
    {
        private IUnitOfWork _db;

        public FloorController(IUnitOfWork _unitOfWork) : base(_unitOfWork) 
        {
            _db = _unitOfWork;
        }


        [HttpPost]
        public IActionResult LoadFloor()
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

            var floor = _db.Floor.GetAll().Where(f => f.IsActive == true && f.IsDeleted == false);

            var floorList = new List<FloorVm>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                floor = floor.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                floor = floor.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                floor = floor.Where(x => x.Name.Contains(searchValue)).ToList();
            }

            foreach (var item in floor)
            {
                floorList.Add(new FloorVm
                {
                    Id = item.Id,
                    Name = item.Name,
                    Decription = item.Decription,
                    DepartmentId = item.DepartmentId,
                    IsActive = item.IsActive,
                    CreatedDate = item.CreatedDate
                });
            }

            floorList = floorList.OrderByDescending(i => i.CreatedDate.Date)
                .ThenByDescending(i => i.CreatedDate.TimeOfDay).ToList();
            //total number of rows count     
            recordsTotal = floorList.Count();

            //Paging     
            var data = floorList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }


        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Name"] = new SelectList(_db.Department.GetAll(), "Id", "Name");
            FloorVm floorVm = new FloorVm();
            return PartialView("_CreateView", floorVm);
        }

        [HttpPost]
        public IActionResult Create(FloorVm floorVm)
        {
            if (ModelState.IsValid)
            {
                Floor floor = new Floor()
                {
                    Name = floorVm.Name,
                    Decription = floorVm.Decription,
                    DepartmentId = floorVm.DepartmentId,
                    IsActive = floorVm.IsActive,
                    IsDeleted = false
                };
                _db.Floor.Add(floor);
                bool isUpdated = _db.Save() > 0;
                
                return Json(floorVm);
            }
            floorVm.IsValid = false;
            floorVm.Message = "Validation Failed!. Please try Again with valid data.";
            return Json(floorVm);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewData["Name"] = new SelectList(_db.Department.GetAll(), "Id", "Name");
            var floor = _db.Floor.Get(id);

            FloorVm floorVm = new FloorVm();
            floorVm.Id = floor.Id;
            floorVm.Name = floor.Name;
            floorVm.Decription = floor.Decription;
            floorVm.DepartmentId = floor.DepartmentId;

            return PartialView("_EditView", floorVm);
        }
        [HttpPost]
        public IActionResult Edit(FloorVm floorVm)
        {
            if (ModelState.IsValid)
            {
                Floor floor = _db.Floor.GetFirstOrDefault(c => c.Id == floorVm.Id);

                floor.Id = floorVm.Id;
                floor.Name = floorVm.Name;
                floor.Decription = floorVm.Decription;
                floor.DepartmentId = floorVm.DepartmentId;

                _db.Floor.Update(floor);
                bool isUpdated = _db.Save() > 0;

                return Json(floorVm);
            }

            floorVm.IsValid = false;
            floorVm.Message = "Validation Failed!. Please try Again with valid data.";
            return Json(floorVm);
        }

        [HttpPost]
        public IActionResult Delete(FloorVm floorVm)
        {
            if (ModelState.IsValid)
            {
                Floor floor = _db.Floor.GetFirstOrDefault(c => c.Id == floorVm.Id);

                floor.IsActive = false;
                floor.IsDeleted = true;

                _db.Floor.Update(floor);
                bool isUpdated = _db.Save() > 0;

                return Json(floorVm);
            }
            floorVm.IsValid = false;
            floorVm.Message = "Validation Failed!. Please try Again with valid data.";
            return Json(floorVm);
        }

        public JsonResult IsExist(string name, int deptId)
        {
            var isFound = _db.Floor.GetFirstOrDefault(c => c.Name == name && c.DepartmentId == deptId && c.IsActive == true && c.IsDeleted == false);
            return Json(isFound);
        }
    }
}