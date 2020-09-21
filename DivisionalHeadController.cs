using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pronali.Data;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.DivisionalHead;
using Pronali.Web.Areas.HR.Models.Employee;
using Pronali.Web.Controllers;
using Pronali.Web.Helper;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    public class DivisionalHeadController : BaseController
    {
        private readonly IImagePath _imagePath;
        public DivisionalHeadController(IUnitOfWork _unitOfWork, IImagePath imagePath) : base(_unitOfWork)
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
            vmDivisionalHead vmDivisionalHead = new vmDivisionalHead();
            return PartialView("DHCreate", vmDivisionalHead);
        }
        [HttpPost]
        public IActionResult Create(vmDivisionalHead vmDivisionalHead)
        {

            if (ModelState.IsValid)
            {
                DivisionalHead divisionalHead = new DivisionalHead()
                {
                    CompanyId = vmDivisionalHead.CompanyId,
                    SisterConcernId = vmDivisionalHead.SisterConcernId,
                    EmployeeId = vmDivisionalHead.EmployeeId,
                    DivisionId = vmDivisionalHead.DivisionId
                };
                db.DivisionalHead.Add(divisionalHead);
                db.Save();
                ModelState.Clear();

                if (divisionalHead.Id > 0)
                {
                    return Json(true);
                }

            }
            return Json(false);
        }

        [HttpPost]
        public IActionResult Edit(vmDivisionalHead divisionalHead)
        {
            //var headObj = db.BranchHead.Get(modelData.Id);
            if (ModelState.IsValid)
            {
                DivisionalHead head = db.DivisionalHead.GetFirstOrDefault(c => c.Id == divisionalHead.Id);
                head.CompanyId = divisionalHead.CompanyId;
                head.EmployeeId = divisionalHead.EmployeeId;
                head.DivisionId = divisionalHead.DivisionId;
                head.SisterConcernId = divisionalHead.SisterConcernId;
                db.DivisionalHead.Update(head);
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
            var head = db.DivisionalHead.Get(id);
            db.DivisionalHead.Remove(head);
            db.Save();

            return Json(true);
        }
        public IActionResult LoadDivisionalHeads()
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

            List<DivisionalHead> divisionalHeads = db.DivisionalHead.GetAllWithRelatedData();

            var DivisionalHeadList = new List<vmDivisionalHead>();

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

            foreach (var item in divisionalHeads)
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
                DivisionalHeadList.Add(new vmDivisionalHead
                {
                    Id = item.Id,
                    CompanyName = item.Company == null ? string.Empty : item.Company.CompanyName,
                    EmployeeName = item.Employee == null ? string.Empty : item.Employee.FullName,
                    DivisionName = item.Division == null ? string.Empty : item.Division.Name,
                    SisterConcernName = item.SisterConcern == null ? string.Empty : item.SisterConcern.Name,
                    PhotoUrl=photoURL,
                    //CompanyId = item.CompanyId,
                    //EmployeeId = item.EmployeeId,
                    //DivisionId = item.DivisionId,
                    //SisterConcernId = item.SisterConcernId,



                });
            }
            //total number of rows count     
            recordsTotal = DivisionalHeadList.Count();

            //Paging     
            var data = DivisionalHeadList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}