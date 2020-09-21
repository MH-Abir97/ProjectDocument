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
    public class ProductCategoriesController : BaseController
    {
        private readonly IUnitOfWork _work;
        private readonly IImagePath _imagePath;

        public ProductCategoriesController(IUnitOfWork work, IImagePath imagePath) : base(work)
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
            return PartialView("_ProductCategoriesCreateView");
        }

        [HttpPost]
        public IActionResult Create(ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                _work.ProductCategory.Add(productCategory);

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
        public IActionResult EditView(int productCategoryId)
        {
            var productCategory = _work.ProductCategory.Get(productCategoryId);

            return PartialView("_ProductCategoriesEditView", productCategory);
        }

        [HttpPost]
        public IActionResult Edit(ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                var category = _work.ProductCategory.Get(productCategory.Id);

                category.Name = productCategory.Name;

                _work.ProductCategory.Update(category);

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
        public IActionResult Delete(int productCategoryId)
        {
            var productCategory = _work.ProductCategory.Get(productCategoryId);

            _work.ProductCategory.Remove(productCategory);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        [HttpGet]
        public IActionResult ProductCategoriesList()
        {
            return PartialView("_ProductCategoriesList");
        }

        public IActionResult LoadProductCategories()
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

            var productCategories = _work.ProductCategory.GetAll();

            var productCategoryList = new List<ProductCategory>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                productCategories = productCategories.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                productCategories = productCategories.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                productCategories = productCategories.Where(x => x.Name.Contains(searchValue)).ToList();
            }

            foreach (var item in productCategories)
            {
                productCategoryList.Add(new ProductCategory
                {
                    Id = item.Id,
                    Name = item.Name,
                });
            }

            //total number of rows count     
            recordsTotal = productCategoryList.Count();

            //Paging     
            var data = productCategoryList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
        }
    }
}