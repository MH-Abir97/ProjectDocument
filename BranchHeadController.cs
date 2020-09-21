﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pronali.Data;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.BranchHead;
using Pronali.Web.Controllers;
using System.Linq.Dynamic.Core;
using Pronali.Web.Areas.HR.Models.Employee;
using Pronali.Web.Helper;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
 

    public class BranchHeadController : BaseController
    {
        private readonly IImagePath _imagePath;

        public BranchHeadController(IUnitOfWork _unitOfWork, IImagePath imagePath) : base(_unitOfWork)
        {
            _imagePath = imagePath;
        }

        public IActionResult GetEmployeeList()
        {
            var result = db.Employee.GetAll().ToList();

            return Json(result);
        }
        public IActionResult GetCompany()
        {
            var data = db.Company.GetAll().ToList();
            return Json(data);
        }
        public IActionResult GetBranch()
        {
            var data = db.Branch.GetAll().ToList();
            return Json(data);
        }
        public IActionResult GetDivision()
        {
            var data = db.Division.GetAll().ToList();
            return Json(data);
        }
        public IActionResult GetSisterConcern()
        {
            var data = db.SisterConcern.GetAll().ToList();
            return Json(data);
        }

        [HttpGet]
        public IActionResult CreateView()
        {           
            vmBranchHead vmBranch = new vmBranchHead();
            return PartialView("BHeadCreate", vmBranch);
        }
        [HttpPost]
        public IActionResult Create(vmBranchHead vmBranchHead)
        {

            if (!ModelState.IsValid)
            {
                BranchHead branchHead = new BranchHead()
                {                
                    CompanyId = vmBranchHead.CompanyId,
                    SisterConcernId = vmBranchHead.SisterConcernId,
                    EmployeeId = vmBranchHead.EmployeeId,
                    BranchId = vmBranchHead.BranchId,
                    DivisionId = vmBranchHead.DivisionId
                };
                db.BranchHead.Add(branchHead);
                db.Save();
                ModelState.Clear();

                if (branchHead.Id > 0)
                {
                    return Json(true);
                }

            }
            return Json(false);
        }

        [HttpPost]
        public IActionResult Edit(vmBranchHead modelData)
        {
            //var headObj = db.BranchHead.Get(modelData.Id);
            if (ModelState.IsValid)
            {
                BranchHead head = db.BranchHead.GetFirstOrDefault(c => c.Id == modelData.Id);
                head.CompanyId = modelData.CompanyId;
                head.EmployeeId = modelData.EmployeeId;
                head.BranchId = modelData.BranchId;
                head.DivisionId = modelData.DivisionId;
                head.SisterConcernId = modelData.SisterConcernId;
                db.BranchHead.Update(head);
                db.Save();
                if (head.Id > 0)
                {
                    return Json(true);
                }

            }
            return Json(false);
        }

        public IActionResult Delete(int id)
        {
            var head = db.BranchHead.Get(id);
            db.BranchHead.Remove(head);
            db.Save();

            return Json(true);
        }
        public IActionResult LoadBranchHeads()
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

            List<BranchHead> branches = db.BranchHead.GetAllWithRelatedData();

            var BranchHeadList = new List<vmBranchHead>();

            //Sorting    
            //if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            //{
            //    branches = branches.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            //}
            //else
            //{
            //    branches = branches.OrderByDescending(x => x.Id).ToList();
            //}

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                //branches = branches.Where(x => x.Id.Contains(searchValue)).ToList();
            }

            foreach (var item in branches)
            {
                string photoURL = "";
                if (!string.IsNullOrEmpty(item.Employee.PhotoUrl))
                {
                    photoURL = _imagePath.GetFilePathAsSourceUrl(item.Employee.PhotoUrl);
                }
                else
                {
                    photoURL = _imagePath.GetFilePathAsSourceUrl("/images/Uploads/Employee/AlterImage.png");
                }
                BranchHeadList.Add(new vmBranchHead
                {
                    Id = item.Id,
                    CompanyName = item.Company == null ? string.Empty : item.Company.CompanyName,
                    EmployeeName = item.Employee == null ? string.Empty : item.Employee.FullName,
                    BranchName = item.Branch == null ? string.Empty : item.Branch.Name,
                    DivisionName =item.Division == null ? string.Empty : item.Division.Name,
                    SisterConcernName = item.SisterConcern == null ? string.Empty : item.SisterConcern.Name,
                    PhotoUrl = photoURL,
                    //CompanyId = item.CompanyId,
                    //EmployeeId = item.EmployeeId,
                    //BranchId = item.BranchId,
                    //DivisionId = item.DivisionId,
                    //SisterConcernId = item.SisterConcernId,



                });
            }
            //total number of rows count     
            recordsTotal = BranchHeadList.Count();

            //Paging     
            var data = BranchHeadList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}