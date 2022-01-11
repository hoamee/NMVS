using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NMVS.Models;
using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using NMVS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Controllers
{

    public class AllocateController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IAllocateService _service;

        public AllocateController(ApplicationDbContext db, IAllocateService service)
        {
            _service = service;
            _db = db;
        }

        [Authorize(Roles = "Arrange inventory")]
        public IActionResult AllocateRequests()
        {
            var workSpace = HttpContext.Session.GetString("susersite");
            var model = _service.GetRequestList(workSpace).OrderBy(x => x.IsClosed);
            return View(model);
        }

        [Authorize(Roles = "Arrange inventory")]
        public IActionResult NewRequest()
        {
            var workSpace = HttpContext.Session.GetString("susersite");
            var whs = _db.Warehouses.Where(x => x.SiCode == workSpace).Select(s => s.WhCode);
            var locList = (from loc in _db.Locs.Where(x => whs.Contains(x.WhCode))
                           join wh in _db.Warehouses on loc.WhCode equals wh.WhCode into locs
                           from i in locs.DefaultIfEmpty()
                           select new Loc
                           {
                               LocCode = loc.LocCode,
                               LocDesc = loc.LocDesc,
                               WhCode = i.WhDesc,
                               LocCap = loc.LocCap,
                               LocCmmt = i.SiCode
                           }).ToList();
            foreach (var loc in locList)
            {
                var used = _db.ItemMasters.Where(x => x.LocCode == loc.LocCode).Sum(x => x.PtQty);
                var hold = _db.AllocateOrders.Where(x => x.LocCode == loc.LocCode).Sum(x => x.AlcOrdQty - x.MovedQty - x.Reported)
                    + _db.IssueOrders.Where(x => x.ToLoc == loc.LocCode).Sum(x => x.ExpOrdQty - x.MovedQty - x.Reported);
                var outGo = _db.AllocateOrders.Where(x => x.AlcOrdFrom == loc.LocCode).Sum(x => x.AlcOrdQty - x.MovedQty - x.Reported)
                    + _db.IssueOrders.Where(x => x.FromLoc == loc.LocCode).Sum(x => x.ExpOrdQty - x.MovedQty - x.Reported);

                loc.LocRemain = loc.LocCap - used - hold;
                loc.LocOutgo = outGo;
                loc.LocHolding = hold;

            }

            ViewBag.LocCode = locList;
            return View();
        }

        [Authorize(Roles = "Arrange inventory")]
        public async Task<ActionResult> SelectLoc([Bind("LocCode")] ItemMaster itemMaster)
        {
            if (string.IsNullOrEmpty(itemMaster.LocCode))
                return RedirectToAction(nameof(NewRequest));
            var workSpace = HttpContext.Session.GetString("susersite");
            var itemMasters = _service.GetAvailItem(itemMaster.LocCode, workSpace);
            var loc = await _db.Locs.FindAsync(itemMaster.LocCode);
            loc.LocRemain = await _db.ItemMasters.Where(x => x.LocCode == loc.LocCode).SumAsync(x => x.PtQty);
            loc.LocRemain = loc.LocCap - loc.LocRemain;
            ViewBag.Loc = loc;
            return View(itemMasters);
        }

        public IActionResult Orders()
        {
            var workSpace = HttpContext.Session.GetString("susersite");
            return View(_service.GetAllocateOrders(workSpace));
        }
        [Authorize(Roles = "Arrange inventory")]
        public IActionResult UnAllocated()
        {
            var workSpace = HttpContext.Session.GetString("susersite");
            return View(_service.GetUnAllocated(workSpace));
        }
        [Authorize(Roles = "Arrange inventory")]
        public async Task<ActionResult> ConfirmLoc(int id)
        {
            var ptmstr = await _db.ItemMasters.FindAsync(id);
            var locList = new List<LocationCapSelect>();
            var whl = await _db.Locs.Where(x => x.LocCode != ptmstr.LocCode && x.LocType != "Unqualified" && x.LocStatus == true).ToListAsync();

            ViewBag.ptid = id;

            foreach (var item in whl)
            {
                var itemQty = _db.ItemMasters.Where(x => x.LocCode == item.LocCode).Sum(s => s.PtQty);
                var locHold = _db.AllocateOrders.Where(x => x.LocCode == item.LocCode && x.Confirm == null).Sum(s => s.AlcOrdQty - s.MovedQty);
                locList.Add(new LocationCapSelect()
                {
                    Ptid = id,
                    LcpId = item.LocCode,
                    Holding = locHold,
                    RemainCapacity = item.LocCap - itemQty,
                    Framable = item.Flammable,
                    LcName = item.LocDesc
                });
            }
            ViewBag.qty = ptmstr.PtQty - ptmstr.PtHold;
            ViewBag.History = _service.GetItemAllocateHistory(id);
            return View(locList);
        }

        public ActionResult ConfirmJsAllocate(JsPickingData jsArr)
        {
            var t = jsArr;
            //Test response
            //string s = jsArr[1].id + ", " + jsArr[1].whcd + ", " + jsArr[1].qty;
            //return Json(s);

            //Get Item master
            var pt = _db.ItemMasters.Find(t.id);
            var fromLoc = _db.Locs.Find(pt.LocCode);


            //   2.Add holding to From-item
            pt.PtHold += t.qty;

            //   3. Add holding to To-Loc
            var toLoc = _db.Locs.Find(t.whcd);
            toLoc.LocHolding += t.qty;

            //   4. Add Outgo to From-Loc
            //fromLoc.LocOutgo += t.qty;


            _db.AllocateRequests.Add(new AllocateRequest()
            {
                PtId = pt.PtId,
                AlcFrom = pt.LocCode,
                LocCode = t.whcd,
                AlcQty = t.qty,
                MovementTime = t.reqTime
            });

            //Meeting 3 remove. No no
            //pt.PtHold = t.qty;
            //pt.PtQty = pt.PtQty - t.qty;
            _db.SaveChanges();

            return Json("Success");
        }
    }
}
