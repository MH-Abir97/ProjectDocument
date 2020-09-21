using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pronali.Data;
using Pronali.Data.Helper;
using Pronali.Data.Models;
using Pronali.Data.Models.Entity.Core;
using Pronali.Web.Areas.Core.Models;
using Pronali.Web.Controllers;
using Pronali.Web.Helper;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;

namespace Pronali.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    public class CompanyController : BaseController
    {
        private IUnitOfWork _db;
        private readonly IImagePath _imagePath;
        public CompanyController(IUnitOfWork _unitOfWork, IImagePath imagePath) : base(_unitOfWork)
        {
            _db= _unitOfWork;
            _imagePath = imagePath;
        }

        public IActionResult CreateView()
        {
            vmCompany companyCreate = new vmCompany();
            return PartialView("_CreateView", companyCreate);
        }
        public IActionResult Clear()
        {
            vmCompany companyCreate = new vmCompany();
            return PartialView("_CreateView", companyCreate);
        }
        public IActionResult Create(vmCompany vmCompany)
        {
            if (ModelState.IsValid)
            {
                Company company = new Company()
                {
                    CompanyName = vmCompany.CompanyName,
                    Phone = vmCompany.Phone,
                    Email = vmCompany.Email,
                    Address = vmCompany.Address,
                    Web = vmCompany.Web,
                    ContactPerson = vmCompany.ContactPerson,
                    ContactPersonNumber = vmCompany.ContactPersonNumber,
                    ContactPersonEmail = vmCompany.ContactPersonEmail,
                    ContactPersonDesignation = vmCompany.ContactPersonDesignation,
                    CompanyType = vmCompany.CompanyType,
                    BusinessType = vmCompany.BusinessType,
                    IsActive = true,
                    IsDeleted = false
                };
                if (vmCompany.Logo != null)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(vmCompany.Logo.ContentDisposition).FileName.Trim('"').Replace(" ", string.Empty);
                    List<string> fileSplitNames = fileName.Split(".").ToList();
                    fileName = fileSplitNames[0] + DateTime.Now.ToString("dddd_dd_MMMM_yyyy_HH_mm_ss") + "." + fileSplitNames[1];
                    var path = _imagePath.GetImagePath(fileName, "Uploads", "Company");
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        vmCompany.Logo.CopyTo(stream);
                    }
                    company.Logo = _imagePath.GetImagePathForDb(path);
                }
                _db.Company.Add(company);
                bool isSaved = _db.Save() > 0;

                ModelState.Clear();
                vmCompany model = new vmCompany();
                return PartialView("_CreateView", model);
            }
            return PartialView("_CreateView", vmCompany);
        }
        public IActionResult EditView(int id)
        {
            var Company = _db.Company.Get(id);

            vmCompany vmCompanyEdit = new vmCompany();

            vmCompanyEdit.Id = Company.Id;
            vmCompanyEdit.CompanyName = Company.CompanyName;
            vmCompanyEdit.Phone = Company.Phone;
            vmCompanyEdit.Email = Company.Email;
            vmCompanyEdit.Address = Company.Address;
            vmCompanyEdit.Web = Company.Web;
            vmCompanyEdit.Email = Company.Email;
            vmCompanyEdit.ContactPerson = Company.ContactPerson;
            vmCompanyEdit.ContactPersonNumber = Company.ContactPersonNumber;
            vmCompanyEdit.ContactPersonEmail = Company.ContactPersonEmail;
            vmCompanyEdit.ContactPersonDesignation = Company.ContactPersonDesignation;
            vmCompanyEdit.CompanyType = Company.CompanyType;
            vmCompanyEdit.BusinessType = Company.BusinessType;
            vmCompanyEdit.LogoUrl = Company.Logo;

            return PartialView("_EditView", vmCompanyEdit);
        }
        public IActionResult Edit(vmCompany vmCompany)
        {
            if (ModelState.IsValid)
            {
                Company company = db.Company.GetFirstOrDefault(c => c.Id == vmCompany.Id);
                company.CompanyName = vmCompany.CompanyName;
                company.Phone = vmCompany.Phone;
                company.Email = vmCompany.Email;
                company.Address = vmCompany.Address;
                company.Web = vmCompany.Web;
                company.ContactPerson = vmCompany.ContactPerson;
                company.ContactPersonNumber = vmCompany.ContactPersonNumber;
                company.ContactPersonEmail = vmCompany.ContactPersonEmail;
                company.ContactPersonDesignation = vmCompany.ContactPersonDesignation;
                company.IsActive = true;

                if (vmCompany.Logo != null)
                {
                    var companyUrl = _imagePath.GetSourceUrlToFilePath(company.Logo);
                    _imagePath.RemoveFileOfPath(companyUrl);
                    var fileName = ContentDispositionHeaderValue.Parse(vmCompany.Logo.ContentDisposition).FileName.Trim('"').Replace(" ", string.Empty);
                    var path = _imagePath.GetImagePath(fileName, "Company", vmCompany.CompanyName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        vmCompany.Logo.CopyTo(stream);
                    }
                    company.Logo = _imagePath.GetImagePathForDb(path);
                }
                _db.Company.Update(company);
                _db.Save();
                bool isSaved = _db.Save() > 0;
                ModelState.Clear();
                vmCompany model = new vmCompany();
                return PartialView("_CreateView", model);
            }
            return PartialView("_EditView", vmCompany);
        }

        public IActionResult ToggleActivation(int companyId)
        {
            bool result = false;

            var company = _db.Company.Get(companyId);
            if(company.IsActive == true)
            {
                company.IsActive = false;
            }
            else
            {
                company.IsActive = true;
            }
            _db.Company.Update(company);
            result = _db.Save() > 0;

            return Json(result);
        }

        public IActionResult Delete(int id)
        {
            bool result = false;

            var company = _db.Company.Get(id);
            company.IsActive = false;
            company.IsDeleted = true;

            _db.Company.Update(company);
            result = _db.Save() > 0;

            return Json(result);
        }

        public IActionResult Details(int companyId)
        {
            var company = _db.Company.Get(companyId);
            vmCompany vmCompany = new vmCompany();
            vmCompany.CompanyName = company.CompanyName;
            vmCompany.Phone = company.Phone;
            vmCompany.Email = company.Email;
            vmCompany.Address = company.Address;
            vmCompany.Web = company.Web;
            vmCompany.ContactPerson = company.ContactPerson;
            vmCompany.ContactPersonNumber = company.ContactPersonNumber;
            vmCompany.ContactPersonEmail = company.ContactPersonEmail;
            vmCompany.ContactPersonDesignation = company.ContactPersonDesignation;
            vmCompany.LogoUrl = company.Logo;
            vmCompany.IsActive = true;

            return PartialView("_Details", vmCompany);
        }

        public IActionResult LoadCompany()
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

           var company = _db.Company.GetAll().Where(c => c.IsActive == true && c.IsDeleted == false);

            var companyList = new List<vmCompany>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                company = company.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                company = company.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                company = company.Where(x => x.CompanyName.Contains(searchValue)
                        || (x.ContactPerson != null && x.ContactPerson.Contains(searchValue))
                        || (x.Email != null && x.Email.Contains(searchValue))
                        || (x.Address != null && x.Address.Contains(searchValue))
                        ).ToList();
                //company = company.Where(x => x.CompanyName.Contains(searchValue) || x.ContactPerson.Contains(searchValue)||x.Email.Contains(searchValue)|| x.Address.Contains(searchValue)).ToList();
            }
            
            foreach (var item in company)
            {
                companyList.Add(new vmCompany
                {
                    Id = item.Id,
                    CompanyName=item.CompanyName,
                    CompanyType=item.CompanyType,
                    ContactPerson=item.ContactPerson,
                    ContactPersonEmail=item.ContactPersonEmail,
                    ContactPersonDesignation=item.ContactPersonDesignation,
                    Address=item.Address,
                    ContactPersonNumber=item.ContactPersonNumber,
                    Email=item.Email,
                    Phone=item.Phone,
                    IsActive = item.IsActive,
                    CreatedDate = item.CreatedDate
                });
            }

            companyList = companyList.OrderByDescending(i => i.CreatedDate.Date)
                .ThenByDescending(i => i.CreatedDate.TimeOfDay).ToList();
            //total number of rows count     
            recordsTotal = companyList.Count();

            //Paging     
            var data = companyList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        public JsonResult IsExist(string name)
        {
            var isFound = _db.Company.GetFirstOrDefault(c => c.CompanyName == name && c.IsActive == true && c.IsDeleted == false);
            return Json(isFound);
        }
    }
}