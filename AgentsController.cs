using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronali.Data;
using Pronali.Data.Models.Entity.Accounts;
using Pronali.Web.Areas.POS.Models.DatatableModels;
using Pronali.Web.Areas.POS.Models.ViewModel;
using Pronali.Web.Helper;
using System.Linq.Dynamic.Core;
using Pronali.Web.Controllers;

namespace Pronali.Web.Areas.POS.Controllers
{
    [Area("POS")]
    public class AgentsController : BaseController
    {
        private readonly IUnitOfWork _work;
        private readonly IImagePath _imagePath;
        public AgentsController(IUnitOfWork work, IImagePath imagePath) : base(work)
        {
            _work = work;
            _imagePath = imagePath;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AgentsList()
        {
            return PartialView("AgentsList");
        }
        public IActionResult CreateView()
        {
            return PartialView("AgentCreateView");
        }

        public IActionResult Create(vmAgent vmAgent)
        {
            if (ModelState.IsValid)
            {
                Agent agent = new Agent()
                {
                    CompanyName = vmAgent.CompanyName,
                    AppointmentDate = vmAgent.AppointmentDate,
                    Area = vmAgent.Area,
                    Name = vmAgent.Name,
                    Phone = vmAgent.Phone,
                    Email = vmAgent.Email,
                    Address = vmAgent.Address,
                    Country = vmAgent.Country,
                    DateOfBirth = vmAgent.DateOfBirth,
                    BillByBill = vmAgent.BillByBill,

                    OnAccount = vmAgent.OnAccount,
                    HasCommission = vmAgent.HasCommission,
                };
                if (vmAgent.PaymentMethod == "onAccount")
                {
                    agent.OnAccount = true;
                }
                else if (vmAgent.PaymentMethod == "billByBill")
                {
                    agent.BillByBill = true;
                }

                if (vmAgent.Photo != null)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(vmAgent.Photo.ContentDisposition).FileName.Trim('"').Replace(" ", string.Empty);

                    var path = _imagePath.GetImagePath(fileName, "Agents", vmAgent.Name);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        vmAgent.Photo.CopyTo(stream);
                    }
                    agent.Photo = _imagePath.GetImagePathForDb(path);
                }

                _work.Agent.Add(agent);

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

            var agent = _work.Agent.Get(id);
            vmAgent vmAgent = new vmAgent();
            vmAgent.Id = agent.Id;
            vmAgent.Name = agent.Name;
            vmAgent.CompanyName = agent.CompanyName;
            vmAgent.Country = agent.Country;
            vmAgent.DateOfBirth = agent.DateOfBirth;
            vmAgent.Email = agent.Email;
            vmAgent.Phone = agent.Phone;
            vmAgent.OnAccount = agent.OnAccount;
            vmAgent.BillByBill = agent.BillByBill;
            vmAgent.HasCommission = agent.HasCommission;
            vmAgent.NID = agent.NID;
            vmAgent.Address = agent.Address;
            return PartialView("EditView", vmAgent);
        }

        public IActionResult Edit(vmAgent vmAgent)
        {
            var agent = _work.Agent.Get(vmAgent.Id);
            if (ModelState.IsValid)
            {
                agent.CompanyName = vmAgent.CompanyName;
                agent.Name = vmAgent.Name;
                agent.Country = vmAgent.Country;
                agent.DateOfBirth = vmAgent.DateOfBirth;
                agent.Email = vmAgent.Email;
                agent.Phone = vmAgent.Phone;
                agent.OnAccount = vmAgent.OnAccount;
                agent.BillByBill = vmAgent.BillByBill;
                agent.HasCommission = vmAgent.HasCommission;
                agent.NID = vmAgent.NID;
                agent.Address = vmAgent.Address;
                if (vmAgent.Photo != null)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(vmAgent.Photo.ContentDisposition).FileName.Trim('"').Replace(" ", string.Empty);
                    var path = _imagePath.GetImagePath(fileName, "Agents", vmAgent.Name).Replace(" ", string.Empty);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        vmAgent.Photo.CopyTo(stream);
                    }
                    agent.Photo = _imagePath.GetImagePathForDb(path);
                }
                _work.Agent.Update(agent);

                bool isSaved = _work.Save() > 0;

                if (isSaved)
                {
                    return Json(true);
                }
            }
            return PartialView("EditView");
        }


        public IActionResult Delete(int id)
        {
            var agent = _work.Agent.Get(id);

            _work.Agent.Remove(agent);

            bool isDeleted = _work.Save() > 0;

            if (isDeleted)
            {
                return Json(true);
            }

            return Json(false);
        }

        public IActionResult LoadAgents()
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

            var agents = _work.Agent.GetAll();

            var agentsList = new List<AgentVm>();

            //Sorting    
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
            {
                agents = agents.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
            }
            else
            {
                agents = agents.OrderByDescending(x => x.Id).ToList();
            }

            //Search    
            if (!string.IsNullOrEmpty(searchValue))
            {
                agents = agents.Where(x => x.Address.Contains(searchValue) || x.CompanyName.Contains(searchValue) || x.Phone.Contains(searchValue) || x.Country.Contains(searchValue)).ToList();
            }

            foreach (var item in agents)
            {
                agentsList.Add(new AgentVm
                {
                    Id = item.Id,
                    CompanyName = item.CompanyName,
                    Name = item.Name,
                    Address = item.Address,
                    Phone = item.Phone,
                    Email = item.Email,
                    Country = item.Country,
                    HasCommission = item.HasCommission,
                    OnAccount = item.OnAccount,
                });
            }

            //total number of rows count     
            recordsTotal = agentsList.Count();

            //Paging     
            var data = agentsList.Skip(skip).Take(pageSize).ToList();

            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
        }
    }
}