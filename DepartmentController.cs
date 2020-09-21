using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Core;
using Pronali.Web.Areas.Core.Models.Department;
using Pronali.Web.Controllers;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;

namespace Pronali.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    public class DepartmentController : BaseController
    {
        private IUnitOfWork _db;
        public DepartmentController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
            _db = _unitOfWork;
        }


        [HttpGet]
        public IActionResult Create()
        {
            vmDepartment vmDepartment = new vmDepartment();
            return PartialView("_CreateView", vmDepartment);
        }

        [HttpPost]
        public IActionResult Create(vmDepartment vmDepartment)
        {

            if (ModelState.IsValid)
            {
                Department department = new Department()
                {
                    Name = vmDepartment.Name,
                    Description = vmDepartment.Description,
                    Phone = vmDepartment.Phone,
                    Email = vmDepartment.Email,
                    IsActive = true,
                    IsDeleted = false
                };
                _db.Department.Add(department);
                bool isUpdated = _db.Save() > 0;
                //if (isUpdated)
                //{
                //    vmDepartment.IsValid = true;
                //    vmDepartment.Message = "Department saved successfully!";

                //    return Json(vmDepartment);
                //}
                //vmDepartment.IsValid = false;
                //vmDepartment.Message = "Department can not be Updated. Something went wrong. Please try Again.";
                return Json(vmDepartment);
            }

            vmDepartment.IsValid = false;
            vmDepartment.Message = "Validation Failed!. Please try Again with valid data.";
            return Json(vmDepartment);
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {

            var department = _db.Department.Get(id);
            vmDepartment vmDepartment = new vmDepartment();
            vmDepartment.Id = department.Id;
            vmDepartment.Name = department.Name;
            vmDepartment.Email = department.Email;
            vmDepartment.Phone = department.Phone;
            vmDepartment.Description = department.Description;
            //vmDepartment.IsActive = department.IsActive;
            return PartialView("_EditView", vmDepartment);
        }

        [HttpPost]
        public IActionResult Edit(vmDepartment vmDepartment)
        {

            if (ModelState.IsValid)
            {
                Department department = _db.Department.GetFirstOrDefault(c => c.Id == vmDepartment.Id);

                department.Id = vmDepartment.Id;
                department.Name = vmDepartment.Name;
                department.Description = vmDepartment.Description;
                department.Phone = vmDepartment.Phone;
                department.Email = vmDepartment.Email;
                //department.IsActive = vmDepartment.IsActive;

                _db.Department.Update(department);

                bool isUpdated = _db.Save() > 0;

                //if (isUpdated)
                //{
                //    vmDepartment.IsValid = true;
                //    vmDepartment.Message = "Department updated successfully!";

                //    return Json(vmDepartment);
                //}
                //vmDepartment.IsValid = false;
                //vmDepartment.Message = "Department can not be Updated. Something went wrong. Please try Again.";
                return Json(vmDepartment);
            }

            vmDepartment.IsValid = false;
            vmDepartment.Message = "Validation Failed!. Please try Again with valid data.";
            return Json(vmDepartment);
        }

       public IActionResult Delete(vmDepartment vmDepartment)
        {
            if (ModelState.IsValid)
            {
                Department department = _db.Department.GetFirstOrDefault(c => c.Id == vmDepartment.Id);

                department.IsActive = false;
                department.IsDeleted = true;
                _db.Department.Update(department);

                bool isUpdated = _db.Save() > 0;
                //if (isUpdated)
                //{
                //    vmDepartment.IsValid = true;
                //    vmDepartment.Message = "Department deleted successfully!";

                //    return Json(vmDepartment);
                //}
                //vmDepartment.IsValid = false;
                //vmDepartment.Message = "Department can not be deleted. Something went wrong. Please try Again.";
                return Json(vmDepartment);
            }
            vmDepartment.IsValid = false;
            vmDepartment.Message = "Validation Failed!. Please try Again with valid data.";
            return Json(vmDepartment);
        }

        public IActionResult LoadDepartment()
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

            var department = _db.Department.GetAll().Where(d=> d.IsActive == true && d.IsDeleted == false).OrderBy(d => d.CreatedDate).ToList();

            var departmentList = new List<vmDepartment>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                department = department.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                department = department.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                department = department.Where(x => x.Name.Contains(searchValue) || x.Email.Contains(searchValue) || x.Email.Contains(searchValue) || x.Phone.Contains(searchValue)).ToList();
            }

            foreach (var item in department)
            {
                departmentList.Add(new vmDepartment
                {
                    Id = item.Id,
                    Name=item.Name,
                    Email = item.Email,
                    Phone = item.Phone,
                    Description = item.Description,
                    //IsActive = item.IsActive,
                    CreatedDate = item.CreatedDate
                });
            }

            departmentList = departmentList.OrderByDescending(i => i.CreatedDate.Date)
                .ThenByDescending(i => i.CreatedDate.TimeOfDay).ToList();
            //total number of rows count     
            recordsTotal = departmentList.Count();

            //Paging     
            var data = departmentList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        public JsonResult IsExist(string name)
        {
            var isFound = _db.Department.GetFirstOrDefault(c => c.Name == name && c.IsActive == true && c.IsDeleted == false);
            return Json(isFound);
        }
    }
}