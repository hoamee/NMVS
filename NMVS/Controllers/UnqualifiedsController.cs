using Microsoft.AspNetCore.Mvc;
using NMVS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Controllers
{
    public class UnqualifiedsController : Controller
    {
      
        ApplicationDbContext _context;

        public UnqualifiedsController( ApplicationDbContext context)
        {
            _context = context;

        }
        public IActionResult OverView()
        {

            var model = _context.Unqualifieds.ToList();
            return View(model);
        }
    }
}
