using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pronali.Data;
using Pronali.Data.Models.Entity.Hr;
using Pronali.Web.Areas.HR.Models.Promotion;
using Pronali.Web.Controllers;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class PromotionController : BaseController
    {
        public PromotionController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
        }
        public IActionResult LoadPromotion()
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

            List<Promotion> promotion = db.Promotion.GetAllWithRelatedData(d => d.IsActive == true && d.IsDeleted == false).ToList();
            //var promotion = db.Promotion.GetAll().Where(d => d.IsActive == true && d.IsDeleted == false);

            var promotionList = new List<vmPromotion>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                promotion = promotion.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                promotion = promotion.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            //if (!string.IsNullOrEmpty(searchValue))
            //{
            //    promotion = promotion.Where(x => x.Name.Contains(searchValue)).ToList();
            //}

            foreach (var item in promotion)
            {
                promotionList.Add(new vmPromotion
                {
                    Id = item.Id,
                    Employee = item.Employee,
                    PrevBranch = item.PrevBranch,
                    TransferedBranch = item.TransferedBranch,
                    PrevDivision = item.PrevDivision,
                    TransferedDivision = item.TransferedDivision,
                    PrevSisterConcern = item.PrevSisterConcern,
                    TransferedSisterConcern = item.TransferedSisterConcern,
                    PrevDepartment = item.PrevDepartment,
                    TransferedDepartment = item.TransferedDepartment,
                    PrevDesignation = item.PrevDesignation,
                    TransferedDesignation = item.TransferedDesignation,
                    Amount = Convert.ToInt32(item.Amount),
                    Percentage = item.Percentage,
                    CreatedDate = item.CreatedDate
                });
            }

            promotionList = promotionList.OrderByDescending(i => i.CreatedDate.Date)
                .ThenByDescending(i => i.CreatedDate.TimeOfDay).ToList();
            //total number of rows count     
            recordsTotal = promotionList.Count();

            //Paging     
            var data = promotionList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        public IActionResult Index()
        {
            ViewBag.Employee = new SelectList(db.Employee.GetAll().Where(s => s.IsActive == true && s.IsDeleted == false), "Id", "FullName");
            ViewBag.SisterConcern = new SelectList(db.SisterConcern.GetAll().Where(s => s.IsActive == true && s.IsDeleted == false), "Id", "Name");
            ViewBag.Division = new SelectList(db.Division.GetAll().Where(s => s.IsActive == true && s.IsDeleted == false), "Id", "Name");
            ViewBag.Branch = new SelectList(db.Branch.GetAll().Where(s => s.IsActive == true && s.IsDeleted == false), "Id", "Name");
            ViewBag.Department = new SelectList(db.Department.GetAll().Where(s => s.IsActive == true && s.IsDeleted == false), "Id", "Name");
            ViewBag.Designation = new SelectList(db.Designation.GetAll().Where(s => s.IsActive == true && s.IsDeleted == false), "Id", "Name");

            return View("Index");
        }

        public IActionResult Create()
        {
            //vmPromotion vmpromotion = new vmPromotion();
            ViewBag.SisterConcern = new SelectList(db.SisterConcern.GetAll().Where(s => s.IsActive == true && s.IsDeleted == false), "Id","Name");
            ViewBag.Division = new SelectList(db.Division.GetAll().Where(s => s.IsActive == true && s.IsDeleted == false), "Id", "Name");
            ViewBag.Branch = new SelectList(db.Branch.GetAll().Where(s => s.IsActive == true && s.IsDeleted == false), "Id", "Name");
            ViewBag.Department = new SelectList(db.Department.GetAll().Where(s => s.IsActive == true && s.IsDeleted == false), "Id", "Name");
            ViewBag.Designation = new SelectList(db.Designation.GetAll().Where(s => s.IsActive == true && s.IsDeleted == false), "Id", "Name");
            return View("Index");
        }

        [HttpPost]
        public IActionResult Create(vmPromotion vmpromotion)
        {
            if (ModelState.IsValid)
            {
                Promotion promotion = new Promotion
                {
                    Id = vmpromotion.Id,
                    EmployeeId = vmpromotion.EmployeeId,
                    PrevBranchId = vmpromotion.PrevBranchId,
                    TransferedBranchId = vmpromotion.TransferedBranchId,
                    PrevDivisionId = vmpromotion.PrevDivisionId,
                    TransferedDivisionId = vmpromotion.TransferedDivisionId,
                    PrevSisterConcernId = vmpromotion.PrevSisterConcernId,
                    TransferedSisterConcernId = vmpromotion.TransferedSisterConcernId,
                    PrevDepartmentId = vmpromotion.PrevDepartmentId,
                    TransferedDepartmentId = vmpromotion.TransferedDepartmentId,
                    PrevDesignationId = vmpromotion.PrevDesignationId,
                    TransferedDesignationId = vmpromotion.TransferedDesignationId,
                    Amount = vmpromotion.Amount,
                    Percentage = vmpromotion.Percentage
                };
                db.Promotion.Add(promotion);
                db.Save();
                return View("Index");
            }
            return View("Index");
        }

        //Edit
        public IActionResult Edit(int id)
        {
            ViewBag.Employee = new SelectList(db.Employee.GetAll().Where(s => s.IsActive == true && s.IsDeleted == false), "Id", "FullName");
            ViewBag.SisterConcern = new SelectList(db.SisterConcern.GetAll().Where(s => s.IsActive == true && s.IsDeleted == false), "Id", "Name");
            ViewBag.Division = new SelectList(db.Division.GetAll().Where(s => s.IsActive == true && s.IsDeleted == false), "Id", "Name");
            ViewBag.Branch = new SelectList(db.Branch.GetAll().Where(s => s.IsActive == true && s.IsDeleted == false), "Id", "Name");
            ViewBag.Department = new SelectList(db.Department.GetAll().Where(s => s.IsActive == true && s.IsDeleted == false), "Id", "Name");
            ViewBag.Designation = new SelectList(db.Designation.GetAll().Where(s => s.IsActive == true && s.IsDeleted == false), "Id", "Name");

            Promotion promotion = db.Promotion.GetFirstOrDefault(h => h.Id == id);
            vmPromotion vmpromotion = new vmPromotion()
            {
                Id = promotion.Id,
                PrevBranchId = promotion.PrevBranchId,
                TransferedBranchId = promotion.TransferedBranchId,
                PrevDivisionId = promotion.PrevDivisionId,
                TransferedDivisionId = promotion.TransferedDivisionId,
                PrevSisterConcernId = promotion.PrevSisterConcernId,
                TransferedSisterConcernId = promotion.TransferedSisterConcernId,
                PrevDepartmentId = promotion.PrevDepartmentId,
                TransferedDepartmentId = promotion.TransferedDepartmentId,
                PrevDesignationId = promotion.PrevDesignationId,
                TransferedDesignationId = promotion.TransferedDesignationId,
                //SalaryBreakup = promotion.SalaryBreakup,
                Amount = Convert.ToInt32(promotion.Amount),
                Percentage = promotion.Percentage,
                EmployeeId = promotion.EmployeeId
            };
            return View("Edit", vmpromotion);
        }

        [HttpPost]
        public IActionResult Edit(vmPromotion vmpromotion)
        {
            if (ModelState.IsValid)
            {
                Promotion promotion = db.Promotion.GetFirstOrDefault(h => h.Id == vmpromotion.Id);

                promotion.Id = vmpromotion.Id;
                promotion.PrevBranchId = vmpromotion.PrevBranchId;
                promotion.TransferedBranchId = vmpromotion.TransferedBranchId;
                promotion.PrevDivisionId = vmpromotion.PrevDivisionId;
                promotion.TransferedDivisionId = vmpromotion.TransferedDivisionId;
                promotion.PrevSisterConcernId = vmpromotion.PrevSisterConcernId;
                promotion.TransferedSisterConcernId = vmpromotion.TransferedSisterConcernId;
                promotion.PrevDepartmentId = vmpromotion.PrevDepartmentId;
                promotion.TransferedDepartmentId = vmpromotion.TransferedDepartmentId;
                promotion.PrevDesignationId = vmpromotion.PrevDesignationId;
                promotion.TransferedDesignationId = vmpromotion.TransferedDesignationId;
                promotion.Amount = vmpromotion.Amount;
                promotion.Percentage = vmpromotion.Percentage;

                db.Promotion.Update(promotion);
                db.Save();
            }
            return View("Edit");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Promotion promotion = db.Promotion.GetFirstOrDefault(c => c.Id == id);
            //UnitOfWork unitOfWork
            promotion.IsActive = false;
            promotion.IsDeleted = true;
            db.Promotion.Update(promotion);
            db.Save();
            return View("Index");
        }
    }
}