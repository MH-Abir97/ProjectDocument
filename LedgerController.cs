using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pronali.Data;
using Pronali.Web.Areas.POS.Helper;
using Pronali.Web.Controllers;

namespace Pronali.Web.Areas.POS.Controllers
{
    [Area("POS")]
    public class LedgerController : BaseController
    {
        private readonly IUnitOfWork _work;
        private readonly ApplicationDbContext _context;

        public LedgerController(IUnitOfWork work, ApplicationDbContext context) : base(work)
        {
            _work = work;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetLedgerSelectList()
        {
            List<SelectListItem> selectList = new List<SelectListItem>();

            selectList.Add(new SelectListItem { Value = 1.ToString(), Text = "Rijvy", Selected = false });

            selectList.AddRange(POSHelper.GetLedgerSelectItems(_work.AccountLedger));

            return Json(selectList);
        }

    }
}