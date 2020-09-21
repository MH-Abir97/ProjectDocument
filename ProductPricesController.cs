using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.POS;
using Pronali.Web.Areas.POS.Models.DatatableModels;
using Pronali.Web.Controllers;
using Pronali.Web.Helper;
using System.Linq.Dynamic.Core;
using Pronali.Web.Areas.POS.Models.ViewModel;
using Pronali.Web.Areas.POS.Helper;

namespace Pronali.Web.Areas.POS.Controllers
{
    [Area("POS")]
    public class ProductPricesController : BaseController
    {
        private readonly IUnitOfWork _work;
        private readonly IImagePath _imagePath;

        public ProductPricesController(IUnitOfWork work, IImagePath imagePath) : base(work)
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
            ViewData["Product"] = POSHelper.GetProductSelectItems(_work.Product);
            ViewData["Price"] = POSHelper.GetPriceSelectItems(_work.Price);

            return PartialView("_ProductPriceCreateView");
        }

        [HttpPost]
        public IActionResult Create(ProductPriceViewModel productPriceViewModel)
        {
            productPriceViewModel.ProductId = productPriceViewModel.ProductId == 0 ? null : productPriceViewModel.ProductId;
            productPriceViewModel.PriceId = productPriceViewModel.PriceId == 0 ? null : productPriceViewModel.PriceId;
            if (ModelState.IsValid)
            {
                var productPrice = new ProductPrice
                {
                    ProductId = productPriceViewModel.ProductId,
                    PriceValue = productPriceViewModel.PriceValue,
                    PriceId = productPriceViewModel.PriceId

                };
                _work.ProductPrice.Add(productPrice);

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
        public IActionResult EditView(int productPriceId)
        {
            ViewData["Product"] = POSHelper.GetProductSelectItems(_work.Product);
            ViewData["Price"] = POSHelper.GetPriceSelectItems(_work.Price);

            var productPrice = _work.ProductPrice.GetWithProductAndPrice(productPriceId);
            return PartialView("_ProductPriceEditView", productPrice);
        }

        [HttpPost]
        public IActionResult Edit(ProductPrice productPrice)
        {
            if (ModelState.IsValid)
            {
                var price = _work.ProductPrice.GetWithProductAndPrice(productPrice.Id);

                price.ProductId = productPrice.ProductId;
                price.PriceValue = productPrice.PriceValue;
                price.PriceId = productPrice.PriceId;

                _work.ProductPrice.Update(price);

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
        public IActionResult Delete(int productPriceId)
        {
            var productPrice = _work.ProductPrice.Get(productPriceId);

            _work.ProductPrice.Remove(productPrice);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        [HttpGet]
        public IActionResult ProductPriceList()
        {
            return PartialView("_ProductPriceList");
        }

        public IActionResult LoadProductPrices()
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

            var productPrices = _work.ProductPrice.GetAllWithProductAndPrice();

            var productPriceList = new List<ProductPriceVm>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                productPrices = productPrices.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                productPrices = productPrices.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                productPrices = productPrices.Where(x => x.PriceValue.Contains(searchValue)).ToList();
            }

            foreach (var item in productPrices)
            {
                productPriceList.Add(new ProductPriceVm
                {
                    Id = item.Id,
                    ProductName = item.Product == null ? string.Empty : item.Product.ProductName,
                    PriceValue = item.PriceValue,
                    PriceName = item.Price == null ? string.Empty : item.Price.Name,
                });
            }

            //total number of rows count     
            recordsTotal = productPriceList.Count();

            //Paging     
            var data = productPriceList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
        }
    }
}