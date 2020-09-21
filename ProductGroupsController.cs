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
    public class ProductGroupsController : BaseController
    {
        private readonly IUnitOfWork _work;
        private readonly IImagePath _imagePath;

        public ProductGroupsController(IUnitOfWork work, IImagePath imagePath) : base(work)
        {
            _work = work;
            _imagePath = imagePath;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateView()
        {
            return PartialView("_ProductGroupsCreateView");
        }

        [HttpPost]
        public IActionResult Create(ProductGroup productGroup)
        {
            if (ModelState.IsValid)
            {
                _work.ProductGroup.Add(productGroup);

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
        public IActionResult EditView(int productGroupId)
        {
            var productGroup = _work.ProductGroup.Get(productGroupId);
            return PartialView("_ProductGroupsEditView", productGroup);
        }

        [HttpPost]
        public IActionResult Edit(ProductGroup productGroup)
        {
            if (ModelState.IsValid)
            {
                var group = _work.ProductGroup.Get(productGroup.Id);

                group.Name = productGroup.Name;

                _work.ProductGroup.Update(group);

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
        public IActionResult Delete(int productGroupId)
        {
            var productGroup = _work.ProductGroup.Get(productGroupId);

            _work.ProductGroup.Remove(productGroup);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        [HttpGet]
        public IActionResult ProductGroupsList()
        {
            return PartialView("_ProductGroupsList");
        }

        public IActionResult LoadProductGroups()
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

            var productGroups = _work.ProductGroup.GetAll();

            var productGroupList = new List<ProductGroup>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                productGroups = productGroups.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                productGroups = productGroups.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                productGroups = productGroups.Where(x => x.Name.Contains(searchValue)).ToList();
            }

            foreach (var item in productGroups)
            {
                productGroupList.Add(new ProductGroup
                {
                    Id = item.Id,
                    Name = item.Name,
                });
            }

            //total number of rows count     
            recordsTotal = productGroupList.Count();

            //Paging     
            var data = productGroupList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
        }
    }
}