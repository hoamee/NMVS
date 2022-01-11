using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NMVS.Models;
using NMVS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Controllers
{
    public class QcController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IIncomingService _service;

        public QcController(ApplicationDbContext context, IIncomingService service)
        {
            _service = service;
            _context = context;
        }

        // GET: IncomingLists
        public IActionResult Browse()
        {
            var workSpace = HttpContext.Session.GetString("susersite");
            return View(_service.BrowseIncomingList(true, workSpace));
        }

        // GET: IncomingLists/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = _service.GetListDetail(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }
    }
}
