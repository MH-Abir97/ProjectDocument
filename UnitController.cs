using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.POS;
using Pronali.Web.Controllers;
using System.Linq.Dynamic.Core;

namespace Pronali.Web.Areas.POS.Controllers
{
    [Area("POS")]
    public class UnitController : BaseController
    {
        private readonly IUnitOfWork _work;      

        public UnitController(IUnitOfWork work) : base(work)
        {
            _work = work;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateView()
        {
            return PartialView("_UnitCreateView");
        }

        [HttpPost]
        public IActionResult Create(Unit unit)
        {
            if (ModelState.IsValid)
            {
                _work.Unit.Add(unit);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }

                return Json(false);
            }
            return Json(false);
        }

        [HttpGet]
        public IActionResult EditView(int unitId)
        {
            var unit = _work.Unit.Get(unitId);

            return PartialView("_UnitEditView", unit);
        }

        [HttpPost]
        public IActionResult Edit(Unit unit)
        {
            if (ModelState.IsValid)
            {
                var unit1 = _work.Unit.Get(unit.Id);

                unit1.Name = unit.Name;

                _work.Unit.Update(unit1);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }

                return Json(false);
            }
            return Json(false);
        }

        [HttpGet]
        public IActionResult Delete(int unitId)
        {
            var unit = _work.Unit.Get(unitId);

            _work.Unit.Remove(unit);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        [HttpGet]
        public IActionResult UnitList()
        {
            return PartialView("_UnitList");
        }

        public IActionResult LoadUnits()
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

            var units = _work.Unit.GetAll();

            var unitList = new List<Unit>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                units = units.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                units = units.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                units = units.Where(x => x.Name.Contains(searchValue)).ToList();
            }

            foreach (var item in units)
            {
                unitList.Add(new Unit
                {
                    Id = item.Id,
                    Name = item.Name,
                });
            }

            //total number of rows count     
            recordsTotal = unitList.Count();

            //Paging     
            var data = unitList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
        }

    }
}