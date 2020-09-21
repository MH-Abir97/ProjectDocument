using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Web.Controllers;
using System.Linq.Dynamic.Core;

namespace Pronali.Web.Areas.POS.Controllers
{
    [Area("POS")]
    public class DoctorController : BaseController
    {
            private readonly IUnitOfWork _work;

            public DoctorController(IUnitOfWork work) : base(work)
            {
                _work = work;
            }
            public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateView()
        {
            return PartialView("_DoctorCreateView");
        }

        [HttpPost]
        public IActionResult Create(Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                _work.Doctor.Add(doctor);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }

                return Json(false);
            }
            return Json(false);
        }

        [HttpGet]
        public IActionResult EditView(int doctorId)
        {
            var doctor = _work.Doctor.Get(doctorId);

            return PartialView("_DoctorEditView", doctor);
        }

        [HttpPost]
        public IActionResult Edit(Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                var doctor1 = _work.Doctor.Get(doctor.Id);

                doctor1.Name = doctor.Name;
                doctor1.Designation = doctor.Designation;
                doctor1.Specialist = doctor.Specialist;
                doctor1.Address = doctor.Address;
                doctor1.LandPhone = doctor.LandPhone;
                doctor1.MobileNumber = doctor.MobileNumber;
                doctor1.Email = doctor.Email;
                doctor1.HospitalName = doctor.HospitalName;                
                doctor1.Country = doctor.Country;
                doctor1.HasCommission = doctor.HasCommission;

                _work.Doctor.Update(doctor1);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }

                return Json(false);
            }
            return Json(false);
        }

        [HttpGet]
        public IActionResult Delete(int doctorId)
        {
            var doctor = _work.Doctor.Get(doctorId);

            _work.Doctor.Remove(doctor);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        [HttpGet]
        public IActionResult DoctorList()
        {
            return PartialView("_DoctorList");
        }

        public IActionResult LoadDoctors()
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

            var doctors = _work.Doctor.GetAll();

            var doctorList = new List<Doctor>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                doctors = doctors.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                doctors = doctors.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                doctors = doctors.Where(x => x.Name.Contains(searchValue)).ToList();
            }

            foreach (var item in doctors)
            {
                doctorList.Add(new Doctor
                {
                    Id = item.Id,
                    Name = item.Name,
                    Designation = item.Designation,
                    Specialist = item.Specialist,
                    Address = item.Address,
                    LandPhone = item.LandPhone,
                    MobileNumber = item.MobileNumber,
                    Email = item.Email,
                    HospitalName = item.HospitalName,
                    Country = item.Country,
                    HasCommission = item.HasCommission,
                });
            }

            //total number of rows count     
            recordsTotal = doctorList.Count();

            //Paging     
            var data = doctorList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
        }

    }
}