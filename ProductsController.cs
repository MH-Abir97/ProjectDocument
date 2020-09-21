using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.POS;
using Pronali.Web.Areas.POS.Helper;
using Pronali.Web.Areas.POS.Models;
using Pronali.Web.Areas.POS.Models.DatatableModels;
using Pronali.Web.Controllers;
using Pronali.Web.Helper;

namespace Pronali.Web.Areas.POS.Controllers
{
    [Area("POS")]
    public class ProductsController : BaseController
    {
        private readonly IUnitOfWork _work;
        private readonly IImagePath _imagePath;

        public ProductsController(IUnitOfWork work, IImagePath imagePath) : base(work)
        {
            _work = work;
            _imagePath = imagePath;
        }

        [HttpGet]
        public IActionResult CreateView()
        {
            ViewData["ProductGroups"] = POSHelper.GetProductGroupSelectItems(_work.ProductGroup);
            ViewData["ProductCategories"] = POSHelper.GetProductCategorySelectItems(_work.ProductCategory);
            ViewData["Conversions"] = POSHelper.GetConversionSelectItems(_work.Conversion);

            return PartialView("_ProductCreateView");
        }

        [HttpPost]
        public IActionResult Create(ProductViewModel productViewModel)
        {
            var product = new Product
            {
                ProductName = productViewModel.ProductName,
                ProductCategoryId = productViewModel.ProductCategoryId,
                ProductGroupId = productViewModel.ProductGroupId,
                ConversionId = productViewModel.ConversionId,
                BrandName = productViewModel.BrandName,
                Manufacturer = productViewModel.Manufacturer,
                CompanyName = productViewModel.CompanyName,
                OriginName = productViewModel.OriginName,
                Prohibit = productViewModel.Prohibit,
                SalesVAT = productViewModel.SalesVAT,
                PurchaseVAT = productViewModel.PurchaseVAT,
                BarCode = productViewModel.BarCode,
                HasColor = productViewModel.HasColor,
                HasSize = productViewModel.HasSize,
                IsNagetiveStock = productViewModel.IsNagetiveStock,
                MaxStockQuantity = productViewModel.MaxStockQuantity,
                MinStockQuantity = productViewModel.MinStockQuantity,
                HasWarranty = productViewModel.HasWarranty,
                HasMultiplePrice = productViewModel.HasMultiplePrice,
                HasMultipleUnit = productViewModel.HasMultipleUnit,
                Author = productViewModel.Author,
                PublicIp = productViewModel.Publisher,
                RakNo = productViewModel.RakNo,
                ProductCode = productViewModel.ProductCode,
                ShortQuantity = productViewModel.ShortQuantity,
                GenericName = productViewModel.GenericName,
                Advice = productViewModel.Advice,
                SideEffect = productViewModel.SideEffect,
                Description = productViewModel.Description,
                ProductType = productViewModel.ProductType
            };

            if (productViewModel.Image != null)
            {
                var fileName = ContentDispositionHeaderValue.Parse(productViewModel.Image.ContentDisposition).FileName.Trim('"').Replace(" ", string.Empty);

                var path = _imagePath.GetImagePath(fileName, "Uploads", "Products");

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    productViewModel.Image.CopyTo(stream);
                }

                product.Image = _imagePath.GetImagePathForDb(path);
            }

            _work.Product.Add(product);

            bool isSaved = _work.Save() > 0;

            if (isSaved)
            {
                return Json(true);
            }

            return Json(false);
        }

        [HttpGet]
        public IActionResult EditView(int productId)
        {
            var product = _work.Product.GetWithCategoryAndGroupAndConversion(productId);

            ViewData["ProductGroups"] = POSHelper.GetProductGroupSelectItems(_work.ProductGroup);
            ViewData["ProductCategories"] = POSHelper.GetProductCategorySelectItems(_work.ProductCategory);
            ViewData["Conversions"] = POSHelper.GetConversionSelectItems(_work.Conversion);

            return PartialView("_ProductEditView", product);
        }

        [HttpPost]
        public IActionResult Edit(ProductViewModel productViewModel)
        {
            var product = _work.Product.GetWithCategoryAndGroupAndConversion(productViewModel.Id);

            product.ProductName = productViewModel.ProductName;
            product.ProductCategoryId = productViewModel.ProductCategoryId;
            product.ProductGroupId = productViewModel.ProductGroupId;
            product.ConversionId = productViewModel.ConversionId;
            product.BrandName = productViewModel.BrandName;
            product.Manufacturer = productViewModel.Manufacturer;
            product.CompanyName = productViewModel.CompanyName;
            product.OriginName = productViewModel.OriginName;
            product.Prohibit = productViewModel.Prohibit;
            product.SalesVAT = productViewModel.SalesVAT;
            product.PurchaseVAT = productViewModel.PurchaseVAT;
            product.BarCode = productViewModel.BarCode;
            product.HasColor = productViewModel.HasColor;
            product.HasSize = productViewModel.HasSize;
            product.IsNagetiveStock = productViewModel.IsNagetiveStock;
            product.MaxStockQuantity = productViewModel.MaxStockQuantity;
            product.MinStockQuantity = productViewModel.MinStockQuantity;
            product.HasWarranty = productViewModel.HasWarranty;
            product.HasMultiplePrice = productViewModel.HasMultiplePrice;
            product.HasMultipleUnit = productViewModel.HasMultipleUnit;
            product.Author = productViewModel.Author;
            product.PublicIp = productViewModel.Publisher;
            product.RakNo = productViewModel.RakNo;
            product.ProductCode = productViewModel.ProductCode;
            product.ShortQuantity = productViewModel.ShortQuantity;
            product.GenericName = productViewModel.GenericName;
            product.Advice = productViewModel.Advice;
            product.SideEffect = productViewModel.SideEffect;
            product.Description = productViewModel.Description;
            product.ProductType = productViewModel.ProductType;

            if (productViewModel.Image != null)
            {
                var fileName = ContentDispositionHeaderValue.Parse(productViewModel.Image.ContentDisposition).FileName.Trim('"').Replace(" ", string.Empty);

                var path = _imagePath.GetImagePath(fileName, "Uploads", "Products");

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    productViewModel.Image.CopyTo(stream);
                }

                product.Image = _imagePath.GetImagePathForDb(path);
            }

            _work.Product.Update(product);

            bool isSaved = _work.Save() > 0;

            if (isSaved)
            {
                return Json(true);
            }

            return Json(false);
        }

        public IActionResult Delete(int productId)
        {
            var product = _work.Product.Get(productId);

            _work.Product.Remove(product);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        [HttpGet]
        public IActionResult ProductList()
        {
            return PartialView("_ProductList");
        }

        public IActionResult LoadProducts()
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

            var products = _work.Product.GetAllWithCategoryAndGroupAndConversion();

            var productList = new List<ProductVm>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                products = products.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                products = products.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                products = products.Where(x => x.ProductName.Contains(searchValue) || x.ProductGroup.Name.Contains(searchValue) || x.CompanyName.Contains(searchValue)).ToList();
            }

            foreach (var item in products)
            {
                productList.Add(new ProductVm
                {
                    Id = item.Id,
                    ProductName = item.ProductName,
                    BrandName = item.BrandName,
                    CompanyName = item.CompanyName,
                    GenericName = item.GenericName,
                    ConversionName = item.Conversion == null ? string.Empty : item.Conversion.Name,
                    ProductCategoryName = item.ProductCategory == null ? string.Empty : item.ProductCategory.Name,
                    ProductGroupName = item.ProductGroup == null ? string.Empty : item.ProductGroup.Name,
                    OriginName = item.OriginName,
                    ProductCode = item.ProductCode
                });
            }

            //total number of rows count     
            recordsTotal = productList.Count();

            //Paging     
            var data = productList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}