using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Pronali.Web.Areas.POS.Controllers
{
    [Area("POS")]
    public class SettingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}