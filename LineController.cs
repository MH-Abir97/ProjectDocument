using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pronali.Data;
using Pronali.Data.Models.Entity.Core;
using Pronali.Web.Areas.Core.Models.Line;
using Pronali.Web.Controllers;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;

namespace Pronali.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    public class LineController : BaseController
    {
        private IUnitOfWork _db;

        public LineController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
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
            ViewData["Name"] = new SelectList(_db.Floor.GetAll(), "Id", "Name");
            vmLine vmLine = new vmLine();
            return PartialView("_CreateView", vmLine);
        }

        [HttpPost]
        public IActionResult Create(vmLine vmLine)
        {
            if (ModelState.IsValid)
            {
                Line line = new Line()
                {
                    Name = vmLine.Name,
                    Decription = vmLine.Decription,
                    FloorId = vmLine.FloorId,
                    IsActive = true,
                    IsDeleted = false
                };

                _db.Line.Add(line);
                bool isUpdated = _db.Save() > 0;
                
                return Json(vmLine);
            }

            vmLine.IsValid = false;
            vmLine.Message = "Validation Failed!. Please try Again with valid data.";
            return Json(vmLine);
        }

        [HttpGet]
        public IActionResult EditView(int id)
        {
            ViewData["Name"] = new SelectList(_db.Floor.GetAll(), "Id", "Name");
            var line = _db.Line.Get(id);

            vmLine vmLine = new vmLine();
            vmLine.Id = line.Id;
            vmLine.Name = line.Name;
            vmLine.Decription = line.Decription;
            vmLine.FloorId = line.FloorId;
            vmLine.IsActive = line.IsActive;

            return PartialView("_EditView", vmLine);
        }

        [HttpPost]
        public IActionResult Edit(vmLine vmLine)
        {
            if (ModelState.IsValid)
            {
                Line line = _db.Line.GetFirstOrDefault(c => c.Id == vmLine.Id);

                line.Id = vmLine.Id;
                line.Name = vmLine.Name;
                line.Decription = vmLine.Decription;
                line.FloorId = vmLine.FloorId;

                _db.Line.Update(line);
                bool isUpdated = _db.Save() > 0;

                return Json(vmLine);
            }

            vmLine.IsValid = false;
            vmLine.Message = "Validation Failed!. Please try Again with valid data.";
            return Json(vmLine);
        }

        [HttpPost]
        public IActionResult LoadLine()
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
            //var line = _db.Line.GetAll();
            var line = _db.Line.GetAll().Where(l => l.IsActive == true && l.IsDeleted == false);

            var lineList = new List<vmLine>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                line = line.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                line = line.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                line = line.Where(x => x.Name.Contains(searchValue)).ToList();
                //|| (x.Description != null && x.Description.Contains(searchValue))
            }

            foreach (var item in line)
            {
                lineList.Add(new vmLine
                {
                    Id = item.Id,
                    Name = item.Name,
                    FloorId = item.FloorId,
                    Decription = item.Decription,
                    IsActive = item.IsActive,
                    CreatedDate = item.CreatedDate
                });
            }

            lineList = lineList.OrderByDescending(i => i.CreatedDate.Date)
                .ThenByDescending(i => i.CreatedDate.TimeOfDay).ToList();
            //total number of rows count     
            recordsTotal = lineList.Count();

            //Paging     
            var data = lineList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpPost]
        public IActionResult Delete(vmLine vmLine)
        {
            if (ModelState.IsValid)
            {
                Line line = _db.Line.GetFirstOrDefault(c => c.Id == vmLine.Id);

                line.IsActive = false;
                line.IsDeleted = true;
                _db.Line.Update(line);

                bool isUpdated = _db.Save() > 0;
                if (isUpdated)
                {
                    vmLine.IsValid = true;
                    vmLine.Message = "Line deleted successfully!";

                    return Json(vmLine);
                }
                vmLine.IsValid = false;
                vmLine.Message = "Line can not be deleted. Something went wrong. Please try Again.";

            }
            vmLine.IsValid = false;
            vmLine.Message = "Validation Failed!. Please try Again with valid data.";
            return Json(vmLine);
        }

        public JsonResult IsExist(string name, int floorId)
        {
            var isFound = _db.Line.GetFirstOrDefault(c => c.Name == name && c.FloorId == floorId && c.IsActive == true && c.IsDeleted == false);
            return Json(isFound);
        }
    }
}