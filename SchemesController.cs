using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.POS;
using Pronali.Web.Controllers;
using Pronali.Web.Helper;
using System.Linq.Dynamic.Core;

namespace Pronali.Web.Areas.POS.Controllers
{
    [Area("POS")]
    public class SchemesController : BaseController
    {
        private readonly IUnitOfWork _work;
        private readonly IImagePath _imagePath;

        public SchemesController(IUnitOfWork work, IImagePath imagePath) : base(work)
        {
            _work = work;
            _imagePath = imagePath;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateView()
        {
            return PartialView("_SchemeCreateView");
        }

        [HttpPost]
        public IActionResult Create(Scheme scheme)
        {
            if (ModelState.IsValid)
            {
                _work.Scheme.Add(scheme);

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
        public IActionResult EditView(int schemeId)
        {
            var scheme = _work.Scheme.Get(schemeId);

            return PartialView("_SchemeEditView", scheme);
        }

        [HttpPost]
        public IActionResult Edit(Scheme scheme)
        {
            if (ModelState.IsValid)
            {
                var scheme1 = _work.Scheme.Get(scheme.Id);

                scheme1.SchemeName = scheme.SchemeName;
                scheme1.SchemeType = scheme.SchemeType;
                scheme1.StartDate = scheme.StartDate;
                scheme1.ExpiredDate = scheme.ExpiredDate;

                _work.Scheme.Update(scheme1);

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
        public IActionResult Delete(int schemeId)
        {
            var scheme = _work.Scheme.Get(schemeId);

            _work.Scheme.Remove(scheme);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        [HttpGet]
        public IActionResult SchemeList()
        {
            return PartialView("_SchemeList");
        }

        public IActionResult LoadSchemes()
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

            var scheme = _work.Scheme.GetAll();

            var schemeList = new List<Scheme>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                scheme = scheme.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                scheme = scheme.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                scheme = scheme.Where(x => x.SchemeName.Contains(searchValue)).ToList();
            }

            foreach (var item in scheme)
            {
                schemeList.Add(new Scheme
                {
                    Id = item.Id,
                    SchemeName = item.SchemeName,
                    SchemeType = item.SchemeType,
                    StartDate = item.StartDate,
                    ExpiredDate =item.ExpiredDate,
                });
            }

            //total number of rows count     
            recordsTotal = schemeList.Count();

            //Paging     
            var data = schemeList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
        }
    }
}