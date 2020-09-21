using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Web.Areas.POS.Models.DatatableModels;
using Pronali.Web.Areas.POS.Models.ViewModel;
using Pronali.Web.Controllers;
using Pronali.Web.Helper;
using System.Linq.Dynamic.Core;
namespace Pronali.Web.Areas.POS.Controllers
{
    [Area("POS")]
    public class OwnersEquityController : BaseController
    {

        private readonly IUnitOfWork _work;
        private readonly IImagePath _imagePath;
        public OwnersEquityController(IUnitOfWork work, IImagePath imagePath) : base(work)
        {
            _work = work;
            _imagePath = imagePath;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult OwnerEquityList()
        {
            return PartialView("_OwnerEquityList");
        }
        public IActionResult CreateView()
        {
            OwnerEquityViewModel ownerEquity = new OwnerEquityViewModel();
            return PartialView("CreateView", ownerEquity);
        }
        public IActionResult Create(OwnerEquityViewModel ownerEquity)
        {
            if (ModelState.IsValid)
            {
                OwnersEquity owners = new OwnersEquity()
                {
                    OwnerName=ownerEquity.OwnerName,
                    Department=ownerEquity.Department,
                    Designattion=ownerEquity.Designattion,
                    OwnerAddress=ownerEquity.OwnerAddress,
                    Email=ownerEquity.Email,
                    NIDNumber=ownerEquity.NIDNumber,
                    Mobile=ownerEquity.Mobile,
                    BirthDate=ownerEquity.BirthDate,
                    HasCommission=ownerEquity.HasCommission,

                };
                if (ownerEquity.Photo != null)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(ownerEquity.Photo.ContentDisposition).FileName.Trim('"').Replace(" ", string.Empty);
                    var path = _imagePath.GetImagePath(fileName, "OwnersEquities", ownerEquity.OwnerName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        ownerEquity.Photo.CopyTo(stream);
                    }
                    owners.Photo = _imagePath.GetImagePathForDb(path);
                }
                _work.OwnerEquity.Add(owners);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }
            }
            return Json(false);
        }

        public IActionResult EditView(int id)
        {
            var ownerEquity = _work.OwnerEquity.Get(id);

            OwnerEquityViewModel owners = new OwnerEquityViewModel();

            owners.Id = ownerEquity.Id;
            owners.OwnerName = ownerEquity.OwnerName;
            owners.Department = ownerEquity.Department;
            owners.Designattion = ownerEquity.Designattion;
            owners.OwnerAddress = ownerEquity.OwnerAddress;
            owners.Email = ownerEquity.Email;
            owners.NIDNumber = ownerEquity.NIDNumber;
            owners.Mobile = ownerEquity.Mobile;
            owners.BirthDate = ownerEquity.BirthDate;
            owners.HasCommission = ownerEquity.HasCommission;
            
            return PartialView("_Edit", owners);
        }

        public IActionResult Edit(OwnerEquityViewModel ownerEquity)
         {
            var owner = _work.OwnerEquity.Get(ownerEquity.Id);
            if (ModelState.IsValid)
            {
                owner.OwnerName = ownerEquity.OwnerName;
                owner.Department = ownerEquity.Department;
                owner.Designattion = ownerEquity.Designattion;
                owner.OwnerAddress = ownerEquity.OwnerAddress;
                owner.Email = ownerEquity.Email;
                owner.NIDNumber = ownerEquity.NIDNumber;
                owner.Mobile = ownerEquity.Mobile;
                owner.BirthDate = ownerEquity.BirthDate;
                owner.HasCommission = ownerEquity.HasCommission;
            
                if (ownerEquity.Photo != null)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(ownerEquity.Photo.ContentDisposition).FileName.Trim('"').Replace(" ", string.Empty);
                    var path = _imagePath.GetImagePath(fileName, "OwnersEquities", ownerEquity.OwnerName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        ownerEquity.Photo.CopyTo(stream);
                    }
                    owner.Photo = _imagePath.GetImagePathForDb(path);
                }
                _work.OwnerEquity.Update(owner);

                bool isUpdate = _work.Save() > 0;

                if (isUpdate)
                {
                    return Json(true);
                }
            }
            return Json(false);
        }
        public IActionResult Delete(int id)
        {
            var ownerEquity = _work.OwnerEquity.Get(id);

            _work.OwnerEquity.Remove(ownerEquity);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        public IActionResult LoadEquities()
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

            var owner = _work.OwnerEquity.GetAll();

            var ownerList = new List<OwnerEquityVm>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                owner = owner.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                owner = owner.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                ownerList = ownerList.Where(x => x.OwnerName.Contains(searchValue) || x.Department.Contains(searchValue) || x.Email.Contains(searchValue) || x.Mobile.Contains(searchValue) || x.OwnerAddress.Contains(searchValue) || x.NIDNumber.Contains(searchValue)).ToList();
            }

            foreach (var item in owner)
            {
                ownerList.Add(new OwnerEquityVm
                {
                     Id=item.Id,
                    OwnerName = item.OwnerName,
                    JoinDate = item.JoinDate,
                    Department = item.Department,
                    Designattion = item.Designattion,
                    OwnerAddress = item.OwnerAddress,
                    Email = item.Email,
                    Mobile = item.Mobile,
                    HasCommission = item.HasCommission,

                });
            }

            //total number of rows count     
            recordsTotal = ownerList.Count();

            //Paging     
            var data = ownerList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

    }
}