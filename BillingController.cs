using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pronali.Data;
using Pronali.Web.Areas.POS.Models;
using Pronali.Web.Controllers;

namespace Pronali.Web.Areas.POS.Controllers
{
    [Area("POS")]
    public class BillingController : BaseController
    {
        private readonly IUnitOfWork _work;

        public BillingController(IUnitOfWork work) : base(work)
        {
            _work = work;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PurchaseWindow()
        {
            PurchaseWindowViewModel purchaseWindowViewModel = new PurchaseWindowViewModel();


            return View(purchaseWindowViewModel);
        }
    }
}