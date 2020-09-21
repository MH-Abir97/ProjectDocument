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
    public class HospitalsController : BaseController
    {
        private readonly IUnitOfWork _work;

        public HospitalsController(IUnitOfWork work) : base(work)
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
            return PartialView("_HospitalCreateView");
        }
        [HttpPost]
        public IActionResult Create(Hospital hospital)
        {
            if (ModelState.IsValid)
            {
                _work.Hospital.Add(hospital);
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
        public IActionResult EditView(int hospitalId)
        {
            var hospital = _work.Hospital.Get(hospitalId);
            return PartialView("_HospitalEditView", hospital);
        }
        [HttpPost]
        public IActionResult Edit(Hospital hospital)
        {
            if (ModelState.IsValid)
            {
                var hospital1 = _work.Hospital.Get(hospital.Id);

                hospital1.Name = hospital.Name;
                hospital1.HospitalBranch = hospital.HospitalBranch;
                hospital1.Address = hospital.Address;
                hospital1.LandPhone = hospital.LandPhone;
                hospital1.LandPhone = hospital.LandPhone;
                hospital1.MobileNumber = hospital.MobileNumber;
                hospital1.Email = hospital.Email;
                hospital1.WebAddress = hospital.WebAddress;
                hospital1.Description = hospital.Description;
                hospital1.HasCommission = hospital.HasCommission;
                //hospital1.Balance = hospital.Balance;
                //hospital1.BalanceRemark = hospital.BalanceRemark;
                //hospital1.CommissionPercent = hospital.CommissionPercent;
                //hospital1.CommissionAmount = hospital.CommissionAmount;

                _work.Hospital.Update(hospital1);

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
        public IActionResult Delete(int hospitalId)
        {
            var hospital = _work.Hospital.Get(hospitalId);

            _work.Hospital.Remove(hospital);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        [HttpGet]
        public IActionResult HospitalList()
        {
            return PartialView("_HospitalList");
        }

        public IActionResult LoadHospitals()
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

            var hospitals = _work.Hospital.GetAll();

            var hospitalList = new List<Hospital>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                hospitals = hospitals.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                hospitals = hospitals.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                hospitals = hospitals.Where(x => x.Name.Contains(searchValue)).ToList();
            }

            foreach (var item in hospitals)
            {
                hospitalList.Add(new Hospital
                {
                    Id = item.Id,
                    Name = item.Name,
                    HospitalBranch = item.HospitalBranch,
                    Address = item.Address,
                    LandPhone = item.LandPhone,
                    MobileNumber = item.MobileNumber,
                    Email = item.Email,
                    WebAddress = item.WebAddress,
                    Description = item.Description,
                    HasCommission = item.HasCommission,
                    //Balance = item.Balance,
                    //BalanceRemark = item.BalanceRemark,
                    //CommissionPercent = item.CommissionPercent,
                    //CommissionAmount = item.CommissionAmount,
                });
            }

            //total number of rows count     
            recordsTotal = hospitalList.Count();

            //Paging     
            var data = hospitalList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
        }
    }
}