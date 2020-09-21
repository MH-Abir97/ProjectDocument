using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Web.Areas.POS.Models.DatatableModels;
using Pronali.Web.Areas.POS.Models.ViewModel;
using Pronali.Web.Controllers;
using System.Linq.Dynamic.Core;
namespace Pronali.Web.Areas.POS.Controllers
{
    [Area("POS")]
    public class InvestorsController : BaseController
    {
        private readonly IUnitOfWork _work;
        public InvestorsController(IUnitOfWork work) : base(work)
        {
            _work = work;
           
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult InvestorList()
        {
            return PartialView("InvestorList");
        }
        public IActionResult CreateView()
        {
            InvestorViewModel investor = new InvestorViewModel();
            return PartialView("_CreateView", investor);
        }
        public IActionResult Create(InvestorViewModel investor)
        {
            if (ModelState.IsValid)
            {
                Investor investorObj = new Investor()
                {
                    CompanyName = investor.CompanyName,
                    InvestorName = investor.InvestorName,
                    Email = investor.Email,
                    LandPhone = investor.LandPhone,
                    Mobile=investor.Mobile,
                    Address=investor.Address,
                    HasCommission=investor.HasCommission,
                };

                _work.Investor.Add(investorObj);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }
            }
            return Json(false);
        }
        public IActionResult EditView(int id)
        {
            var investor = _work.Investor.Get(id);
            InvestorViewModel investorViewModel = new InvestorViewModel();
            investorViewModel.CompanyName = investor.CompanyName;
            investorViewModel.InvestorName = investor.InvestorName;
            investorViewModel.Email = investor.Email;
            investorViewModel.LandPhone = investor.LandPhone;
            investorViewModel.Mobile = investor.Mobile;
            investorViewModel.Address = investor.Address;
            investorViewModel.HasCommission = investor.HasCommission;
            return PartialView("_EditView", investorViewModel);
        }

        public IActionResult Edit(InvestorViewModel investor)
        {
            var investorObj = _work.Investor.Get(investor.Id);
            if (ModelState.IsValid)
            {
               // investorObj.Id = investor.Id;
                investorObj.CompanyName = investor.CompanyName;
                investorObj.InvestorName = investor.InvestorName;
                investorObj.Email = investor.Email;
                investorObj.LandPhone = investor.LandPhone;
                investorObj.Mobile = investor.Mobile;
                investorObj.Address = investor.Address;
                investorObj.HasCommission = investor.HasCommission;

            }
            _work.Investor.Update(investorObj);
            bool isUpdate = _work.Save() > 0;
            if (isUpdate)
            {
                return Json(true);
            }
            return Json(false);
        }
        public IActionResult Delete(int id)
        {
            var investor = _work.Investor.Get(id);

            _work.Investor.Remove(investor);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }
        public IActionResult LoadInvestor()
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

            var investor = _work.Investor.GetAll();

            var investorlList = new List<InvestorVm>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                investor = investor.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                investor = investor.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                investorlList = investorlList.Where(x => x.InvestorName.Contains(searchValue) || x.Address.Contains(searchValue) || x.Email.Contains(searchValue) || x.Mobile.Contains(searchValue) || x.LandPhone.Contains(searchValue)).ToList();
            }

            foreach (var item in investor)
            {
                investorlList.Add(new InvestorVm
                {
                    Id=item.Id,
                    CompanyName = item.CompanyName,
                    InvestorName = item.InvestorName,
                    Address = item.Address,
                    LandPhone = item.LandPhone,
                    Mobile = item.Mobile,
                    Email = item.Email,
                    HasCommission = item.HasCommission,

                });
            }

            //total number of rows count     
            recordsTotal = investorlList.Count();

            //Paging     
            var data = investorlList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}