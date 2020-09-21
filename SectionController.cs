using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Core;
using Pronali.Web.Areas.Core.Models.Section;
using Pronali.Web.Controllers;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;

namespace Pronali.Web.Areas.Core.Controllers
{
    [Area("Core")]
    [Authorize]
    public class SectionController : BaseController
    {

        public SectionController(IUnitOfWork _unitOfWork) : base(_unitOfWork)
        {
        }

        [HttpGet]
        public IActionResult Create()
        {
            SectionVm sectionVm = new SectionVm();
            return PartialView("_CreateView", sectionVm);
        }

        [HttpPost]
        public IActionResult Create(SectionVm sectionVm)
        {
            if (ModelState.IsValid)
            {
                Section section = new Section()
                {
                    Name = sectionVm.Name,
                    Description = sectionVm.Description,
                    Phone = sectionVm.Phone,
                    Email = sectionVm.Email,
                    IsActive = true,
                    IsDeleted = false
                };

                db.Section.Add(section);
                db.Save();
                
                return Json(sectionVm);
            }

            sectionVm.IsValid = false;
            sectionVm.Message = "Validation Failed!. Please try Again with valid data.";
            return Json(sectionVm);
        }
        [HttpPost]
        public IActionResult LoadSection()
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

            var section = db.Section.GetAll().Where(s => s.IsActive == true && s.IsDeleted == false).ToList();

            var sectionList = new List<SectionVm>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                section = section.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                section = section.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                section = section.Where(x => x.Name.Contains(searchValue) 
                        || (x.Description != null && x.Description.Contains(searchValue))
                        || (x.Phone != null && x.Phone.Contains(searchValue))
                        || (x.Email != null && x.Email.Contains(searchValue))
                        ).ToList();
            }

            foreach (var item in section)
            {
                sectionList.Add(new SectionVm
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Phone = item.Phone,
                    Email = item.Email,
                    IsActive = item.IsActive,
                    CreatedDate = item.CreatedDate
                });
            }

            sectionList = sectionList.OrderByDescending(i => i.CreatedDate.Date)
                .ThenByDescending(i => i.CreatedDate.TimeOfDay).ToList();
            //total number of rows count     
            recordsTotal = sectionList.Count();

            //Paging     
            var data = sectionList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var Section = db.Section.Get(id);
            SectionVm sectionVm = new SectionVm();

            sectionVm.Id = Section.Id;
            sectionVm.Name = Section.Name;
            sectionVm.Description = Section.Description;
            sectionVm.Phone = Section.Phone;
            sectionVm.Email = Section.Email;
            sectionVm.IsActive = Section.IsActive;

            return PartialView("_EditView", sectionVm);
        }


        [HttpPost]
        public IActionResult Edit(SectionVm sectionVm)
        {
            if (ModelState.IsValid)
            {
                Section section = db.Section.GetFirstOrDefault(c => c.Id == sectionVm.Id);

                section.Id = sectionVm.Id;
                section.Name = sectionVm.Name;
                section.Description = sectionVm.Description;
                section.Phone = sectionVm.Phone;
                section.Email = sectionVm.Email;

                db.Section.Update(section);
                db.Save();

                return Json(sectionVm);
            }

            sectionVm.IsValid = false;
            sectionVm.Message = "Validation Failed!. Please try Again with valid data.";
            return Json(sectionVm);
        }

        [HttpPost]
        public IActionResult Delete(SectionVm sectionVm)
        {
            if (ModelState.IsValid)
            {
                Section section = db.Section.GetFirstOrDefault(c => c.Id == sectionVm.Id);

                section.IsActive = false;
                section.IsDeleted = true;

                db.Section.Update(section);
                bool isUpdated = db.Save() > 0;

                return Json(sectionVm);
            }
            sectionVm.IsValid = false;
            sectionVm.Message = "Validation Failed!. Please try Again with valid data.";
            return Json(sectionVm);
        }

        public JsonResult IsExist(string name)
        {
            var isFound = db.Section.GetFirstOrDefault(c => c.Name == name && c.IsActive == true && c.IsDeleted == false);
            return Json(isFound);
        }
    }
}