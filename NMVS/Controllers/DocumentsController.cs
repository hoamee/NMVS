using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Controllers
{
    public class DocumentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Register()
        {
            return View();
        }

        public IActionResult RoleManager()
        {
            return View();
        }

        public IActionResult Site()
        {
            return View();
        }

        public IActionResult Warehouse()
        {
            return View();
        }
    }
}
