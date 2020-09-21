using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pronali.Web.Areas.HR.Controllers
{
    [Area("HR")]
    [Authorize]
    public class GratuityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}