using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pronali.Data;
using Pronali.Data.Models.Entity.Core;
using Pronali.Web.Areas.Core.Models.SisterConcern;
using Pronali.Web.Controllers;

namespace Pronali.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    public class SisterConcernController : BaseController
    {
        public SisterConcernController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["CompanyId"] = new SelectList(db.Company.GetAll().Where(c => c.IsActive == true && c.IsDeleted == false), "Id", "CompanyName");
            vmSisterConcern vmSisterConcern = new vmSisterConcern();
            return PartialView("_Create", vmSisterConcern);
        }

        [HttpPost]
        public IActionResult Create(vmSisterConcern vmSisterConcern)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    SisterConcern sisterConcern = new SisterConcern()
                    {
                        Name = vmSisterConcern.Name,
                        CompanyId = vmSisterConcern.CompanyId,
                        Address = vmSisterConcern.Address,
                        Email = vmSisterConcern.Email,
                        Phone = vmSisterConcern.Phone,
                        IsActive = true,
                        IsDeleted = false
                    };
                    db.SisterConcern.Add(sisterConcern);
                    db.Save();
                }
                catch (Exception ex)
                {
                    return Json(ex.Message);
                }
            }
            return Json(vmSisterConcern);
        }


        //Edit
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewData["CompanyId"] = new SelectList(db.Company.GetAll().Where(c => c.IsActive == true && c.IsDeleted == false), "Id", "CompanyName");

            SisterConcern sisterConcern = db.SisterConcern.Get(id);
            vmSisterConcern vmSisterConcern = new vmSisterConcern();
            vmSisterConcern.Id = sisterConcern.Id;
            vmSisterConcern.Name = sisterConcern.Name;
            vmSisterConcern.CompanyId = sisterConcern.CompanyId;
            vmSisterConcern.Phone = sisterConcern.Phone;
            vmSisterConcern.Email = sisterConcern.Email;
            vmSisterConcern.Address = sisterConcern.Address;
            return PartialView("_Edit", vmSisterConcern);
        }

        [HttpPost]
        public IActionResult Edit(vmSisterConcern vmSisterConcern)
        {
            if (ModelState.IsValid)
            {
                SisterConcern sisterConcern = db.SisterConcern.GetFirstOrDefault(c => c.Id == vmSisterConcern.Id);

                sisterConcern.Name = vmSisterConcern.Name;
                sisterConcern.CompanyId = vmSisterConcern.CompanyId;
                sisterConcern.Phone = vmSisterConcern.Phone;
                sisterConcern.Email = vmSisterConcern.Email;
                sisterConcern.Address = vmSisterConcern.Address;

                db.SisterConcern.Update(sisterConcern);
                db.Save();
                return Json(vmSisterConcern);
            }
            return Json(vmSisterConcern);
        }

        [HttpPost]
        public IActionResult Delete(long id)
        {
            SisterConcern sisterConcern = db.SisterConcern.GetFirstOrDefault(c => c.Id == id);
            sisterConcern.IsActive = false;
            sisterConcern.IsDeleted = false;
            db.SisterConcern.Update(sisterConcern);
            db.Save();
            return Json("Deleted.");
        }

        public IActionResult LoadSisterConcern()
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

            List<SisterConcern> sisterConcerns = db.SisterConcern.GetAllWithRelatedData(d => d.IsActive == true && d.IsDeleted == false).ToList();

            var sisterConcernList = new List<vmSisterConcern>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                sisterConcerns = sisterConcerns.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                sisterConcerns = sisterConcerns.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                sisterConcerns = sisterConcerns.Where(x => x.Name.Contains(searchValue)).ToList();
            }

            foreach (var item in sisterConcerns)
            {
                sisterConcernList.Add(new vmSisterConcern
                {
                    Id = item.Id,
                    Name = item.Name,
                    Phone = item.Phone,
                    Email = item.Email,
                    Address = item.Address,
                    Company = new Models.vmCompany
                    {
                        CompanyName = item.Company.CompanyName
                    },
                    CreatedDate = item.CreatedDate
                });
            }

            sisterConcernList = sisterConcernList.OrderByDescending(i => i.CreatedDate.Date)
                .ThenByDescending(i => i.CreatedDate.TimeOfDay).ToList();
            //total number of rows count     
            recordsTotal = sisterConcernList.Count();

            //Paging     
            var data = sisterConcernList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

    }
}