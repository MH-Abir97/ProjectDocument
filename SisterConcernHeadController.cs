using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pronali.Data;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.Employee;
using Pronali.Web.Areas.HR.Models.SisterConcernHead;
using Pronali.Web.Controllers;
using Pronali.Web.Helper;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    public class SisterConcernHeadController : BaseController
    {
        private readonly IImagePath _imagePath;
        public SisterConcernHeadController(IUnitOfWork _unitOfWork, IImagePath imagePath) : base(_unitOfWork)
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
        public IActionResult GetSisterConcern()
        {
            var data = db.SisterConcern.GetAll().ToList();
            return Json(data);
        }
       
        [HttpGet]
        public IActionResult CreateView()
        {          
            vmSisterConcernHead vmSisterConcernHead = new vmSisterConcernHead();
            return PartialView("SCHCreate", vmSisterConcernHead);
        }
        [HttpPost]
        public IActionResult Create(vmSisterConcernHead vmSisterConcernHead)
        {

            if (ModelState.IsValid)
            {
                SisterConcernHead concern = new SisterConcernHead()
                {
                    CompanyId = vmSisterConcernHead.CompanyId,
                    SisterConcernId = vmSisterConcernHead.SisterConcernId,
                    EmployeeId = vmSisterConcernHead.EmployeeId
                };
                db.SisterConcernHead.Add(concern);
                db.Save();
                ModelState.Clear();

                if (concern.Id > 0)
                {
                    return Json(true);
                }

            }
            return Json(false);
        }
        [HttpPost]
        public IActionResult Edit(vmSisterConcernHead modelData)
        {
            //var headObj = db.BranchHead.Get(modelData.Id);
            if (ModelState.IsValid)
            {
                SisterConcernHead head = db.SisterConcernHead.GetFirstOrDefault(c => c.Id == modelData.Id);
                head.CompanyId = modelData.CompanyId;
                head.EmployeeId = modelData.EmployeeId;
                head.SisterConcernId = modelData.SisterConcernId;
                db.SisterConcernHead.Update(head);
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
            var head = db.SisterConcernHead.Get(id);
            db.SisterConcernHead.Remove(head);
            db.Save();

            return Json(true);
        }
        public IActionResult LoadSisterConcernHeads()
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

            List<SisterConcernHead> concernHeads = db.SisterConcernHead.GetAllWithRelatedData();

            var SisterConcernHeadList = new List<vmSisterConcernHead>();

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

            foreach (var item in concernHeads)
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
                SisterConcernHeadList.Add(new vmSisterConcernHead
                {
                    Id = item.Id,
                    CompanyName = item.Company == null ? string.Empty : item.Company.CompanyName,
                    EmployeeName = item.Employee == null ? string.Empty : item.Employee.FullName,
                    SisterConcernName = item.SisterConcern == null ? string.Empty : item.SisterConcern.Name,
                    PhotoUrl = photoURL,
                    //CompanyId = item.CompanyId,
                    //EmployeeId = item.EmployeeId,
                    //SisterConcernId = item.SisterConcernId,



                });
            }
            //total number of rows count     
            recordsTotal = SisterConcernHeadList.Count();

            //Paging     
            var data = SisterConcernHeadList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}