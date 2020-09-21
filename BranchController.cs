using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pronali.Data;
using Pronali.Data.Models.Entity.Core;
using Pronali.Web.Areas.Core.Models.Branch;
using Pronali.Web.Controllers;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;

namespace Pronali.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    public class BranchController : BaseController
    {
       public BranchController(IUnitOfWork _unitOfWork): base(_unitOfWork)
        {
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(db.Company.GetAll(), "Id", "CompanyName");
            vmBranch vmBranch = new vmBranch();
            return PartialView("_CreateView", vmBranch);
        }

        [HttpPost]
        public IActionResult Create(vmBranch vmBranch)
        {

            if (ModelState.IsValid)
            {
                Branch branch = new Branch()
                {
                    Name = vmBranch.Name,
                    Phone = vmBranch.Phone,
                    Email = vmBranch.Email,
                    Address = vmBranch.Address,
                    Description = vmBranch.Description,
                    CompanyId = vmBranch.CompanyId,
                    IsActive = true,
                    IsDeleted = false
                    //ReservationId = vmBranch.ReservationId+500000,
                };
                db.Branch.Add(branch);
                bool isUpdated = db.Save() > 0;
                if (isUpdated)
                {
                    vmBranch.IsValid = true;
                    vmBranch.Message = "Branch saved successfully!";

                    return Json(vmBranch);
                }
                vmBranch.IsValid = false;
                vmBranch.Message = "Branch can not be Updated. Something went wrong. Please try Again.";
                return Json(vmBranch);
            }

            vmBranch.IsValid = false;
            vmBranch.Message = "Validation Failed!. Please try Again with valid data.";
            return Json(vmBranch);
        }

      
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewData["CompanyId"] = new SelectList(db.Company.GetAll(), "Id", "CompanyName");
            var branch = db.Branch.Get(id);
            vmBranch vmbranch = new vmBranch();
            vmbranch.Id = branch.Id;
            vmbranch.Name = branch.Name;
            vmbranch.Email = branch.Email;
            vmbranch.Phone = branch.Phone;
            vmbranch.Description= branch.Description;
            vmbranch.CompanyId= branch.CompanyId;
            vmbranch.Address= branch.Address;
       
            return PartialView("_EditView", vmbranch);
        }

        [HttpPost]
        public IActionResult Edit(vmBranch vmBranch)
        {
            if (ModelState.IsValid)
            {
                Branch branch = db.Branch.GetFirstOrDefault(c => c.Id == vmBranch.Id);

                branch.Id = vmBranch.Id;
                branch.Name = vmBranch.Name;
                branch.Phone = vmBranch.Phone;
                branch.Email = vmBranch.Email;
                branch.Address = vmBranch.Address;
                branch.Description = vmBranch.Description;

                db.Branch.Update(branch);
                db.Save();

                return Json(vmBranch);
            }

            vmBranch.IsValid = false;
            vmBranch.Message = "Validation Failed!. Please try Again with valid data.";
            return Json(vmBranch);
        }

        [HttpPost]
        public IActionResult Delete(vmBranch vmBranch)
        {
            if (ModelState.IsValid)
            {
                Branch branch = db.Branch.GetFirstOrDefault(c => c.Id == vmBranch.Id);

                branch.IsActive = false;
                branch.IsDeleted = true;
                db.Branch.Update(branch);

                //_db.Branch.Remove(branch);

                bool isUpdated = db.Save() > 0;
                if (isUpdated)
                {
                    vmBranch.IsValid = true;
                    vmBranch.Message = "Branch deleted successfully!";

                    return Json(vmBranch);
                }
                vmBranch.IsValid = false;
                vmBranch.Message = "Branch can not be deleted. Something went wrong. Please try Again.";
            }
            vmBranch.IsValid = false;
            vmBranch.Message = "Validation Failed!. Please try Again with valid data.";
            return Json(vmBranch);
        }
        public IActionResult LoadBranch()
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

            var branch = db.Branch.GetAllWithRelatedData(b=> b.IsActive==true && b.IsDeleted == false).ToList();

            var branchList = new List<vmBranch>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                branch = branch.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                branch = branch.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                branch = branch.Where(x => x.Name.Contains(searchValue)).ToList();
            }
            foreach (var item in branch)
            {
                branchList.Add(new vmBranch
                {
                    Id = item.Id,
                    Name=item.Name,
                    Email = item.Email,
                    Phone = item.Phone,
                    Address = item.Address,
                    Description = item.Description,
                    CompanyId = item.CompanyId,
                    Company = item.Company,
                    CreatedDate = item.CreatedDate
                });
            }

            branchList = branchList.OrderByDescending(i => i.CreatedDate.Date)
                .ThenByDescending(i => i.CreatedDate.TimeOfDay).ToList();
            //total number of rows count     
            recordsTotal = branchList.Count();

            //Paging     
            var data = branchList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        public JsonResult IsExist(string name, int companyId)
        {
            var isFound = db.Branch.GetFirstOrDefault(c => c.Name == name && c.CompanyId == companyId && c.IsActive == true && c.IsDeleted == false);
            return Json(isFound);
        }
    }
}