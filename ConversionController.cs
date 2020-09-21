using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.POS;
using Pronali.Web.Areas.POS.Helper;
using Pronali.Web.Areas.POS.Models;
using Pronali.Web.Areas.POS.Models.DatatableModels;
using Pronali.Web.Controllers;
using Pronali.Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;


namespace Pronali.Web.Areas.POS.Controllers
{
    [Area("POS")]
    public class ConversionController : BaseController
    {
        private readonly IUnitOfWork _work;

        public ConversionController(IUnitOfWork work) : base(work)
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
            ViewData["Unit"] = POSHelper.GetUnitSelectItems(_work.Unit);
            return PartialView("_ConversionCreateView");
        }

        [HttpPost]
        public IActionResult Create(ConversionViewModel conversionViewModel)
        {
            var conversion = new Conversion
            {
                Name = conversionViewModel.Name,
                UnitValue1 = conversionViewModel.UnitValue1,
                UnitId1 = conversionViewModel.UnitId1 == 0 ? (int?)null : conversionViewModel.UnitId1,
                UnitValue2 = conversionViewModel.UnitValue2,
                UnitId2 = conversionViewModel.UnitId2 == 0 ? (int?)null : conversionViewModel.UnitId2,
                UnitValue3 = conversionViewModel.UnitValue3,
                UnitId3 = conversionViewModel.UnitId3 == 0 ? (int?)null : conversionViewModel.UnitId3,
                UnitValue4 = conversionViewModel.UnitValue4,
                UnitId4 = conversionViewModel.UnitId4 == 0 ? (int?)null : conversionViewModel.UnitId4,
                UnitValue5 = conversionViewModel.UnitValue5,
                UnitId5 = conversionViewModel.UnitId5 == 0 ? (int?)null : conversionViewModel.UnitId5
            };
            _work.Conversion.Add(conversion);

            bool isSaved = _work.Save() > 0;
            if (isSaved)
            {
                return Json(true);
            }
            return Json(false);
        }

        [HttpGet]
        public IActionResult EditView(int conversionId)
        {
            ViewData["Unit"] = POSHelper.GetUnitSelectItems(_work.Unit);
            var conversion = _work.Conversion.GetWithUnit(conversionId);

            return PartialView("_ConversionEditView", conversion);
        }

        [HttpPost]
        public IActionResult Edit(Conversion conversion)
        {
            var conversion1 = _work.Conversion.GetWithUnit(conversion.Id);

            conversion1.Name = conversion.Name;
            conversion1.UnitValue1 = conversion.UnitValue1;
            conversion1.UnitId1 = conversion.UnitId1;
            conversion1.UnitValue2 = conversion.UnitValue2;
            conversion1.UnitId2 = conversion.UnitId2;
            conversion1.UnitValue3 = conversion.UnitValue3;
            conversion1.UnitId3 = conversion.UnitId3;
            conversion1.UnitValue4 = conversion.UnitValue4;
            conversion1.UnitId4 = conversion.UnitId4;
            conversion1.UnitValue5 = conversion.UnitValue5;
            conversion1.UnitId5 = conversion.UnitId5;

            _work.Conversion.Update(conversion1);

            bool isSaved = _work.Save() > 0;

            if (isSaved)
            {
                return Json(true);
            }

            return Json(false);
        }

        public IActionResult Delete(int conversionId)
        {
            var conversion = _work.Conversion.Get(conversionId);

            _work.Conversion.Remove(conversion);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        [HttpGet]
        public IActionResult ConversionList()
        {
            return PartialView("_ConversionList");
        }
        public IActionResult LoadConversions()
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

            var conversions = _work.Conversion.GetAllWithUnit();

            var conversionList = new List<ConversionVm>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                conversions = conversions.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                conversions = conversions.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                conversions = conversions.Where(x => x.Name.Contains(searchValue)).ToList();
            }
            foreach (var item in conversions)
            {
                conversionList.Add(new ConversionVm
                {
                    Id = item.Id,
                    Name = item.Name,
                    UnitName1 = item.Unit1 == null ? string.Empty : item.Unit1.Name,
                    UnitValue1 = item.UnitValue1,
                    UnitName2 = item.Unit2 == null ? string.Empty : item.Unit2.Name,
                    UnitValue2 = item.UnitValue2,
                    UnitName3 = item.Unit3 == null ? string.Empty : item.Unit3.Name,
                    UnitValue3 = item.UnitValue3,
                    UnitName4 = item.Unit4 == null ? string.Empty : item.Unit4.Name,
                    UnitValue4 = item.UnitValue4,
                    UnitName5 = item.Unit5 == null ? string.Empty : item.Unit5.Name,
                    UnitValue5 = item.UnitValue5,
                });
            }

            //total number of rows count     
            recordsTotal = conversionList.Count();

            //Paging     
            var data = conversionList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}