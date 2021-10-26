using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NMVS.Models;
using NMVS.Models.DbModels;
using NMVS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Controllers
{
    public class InvRequestsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IRequestService _service;

        public InvRequestsController(ApplicationDbContext db, IRequestService service)
        {
            _service = service;
            _db = db;
        }


        public IActionResult History()
        {
            //if (User.IsInRole("WH Manager") || User.IsInRole("Sales Manager"))
            //{
            
            return View(_service.GetRequestList());
            //}
            //else
            //{
            //    var requests = _db.InvRequests.Where(w => w.RqBy == User.Identity.Name);
            //    return View(requests.ToList());
            //}
        }

        public IActionResult NewRequest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult NewRequest(InvRequest invRequest)
        {
            if (invRequest is null)
            {
                ModelState.AddModelError("", "An error occurred");
            }
            else
            {
                var checkExist = _db.InvRequests.FirstOrDefault(x => x.Ref == invRequest.Ref);
                if (checkExist is null)
                {
                    invRequest.RqID = "MFG-" + invRequest.Ref;
                    _db.Add(invRequest);
                    _db.SaveChanges();
                    return RedirectToAction(nameof(History));
                }
                else
                {
                    ModelState.AddModelError("", "This batch is already exist");
                }
            }

            return View();
        }

        public IActionResult RequestDetail(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = _service.GetRequestDetail(id);
            
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        public ActionResult RequestDetCreate(string id)
        {
            ViewBag.ItemNo = _db.ItemDatas.ToList();

            ViewBag.RqId = id;

            return View();
        }
    }
}
