﻿using Microsoft.AspNetCore.Mvc;
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


        public IActionResult AllocateRequests()
        {
            var model = _service.GetRequestList().OrderBy(x => x.IsClosed);
            return View(model);
        }

        public IActionResult NewRequest()
        {
            ViewBag.ItemNo = _service.GetItemDistinc();
            ViewBag.LocCode = new SelectList(_db.Locs
                , "LocCode", "LocDesc");
            return View();
        }

        public async Task<ActionResult> SelectLoc([Bind("ItemNo,LocCode")] ItemMaster itemMaster)
        {
            var itemMasters = _service.GetAvailItem(itemMaster.ItemNo);
            ViewBag.Loc = await _db.Locs.FindAsync(itemMaster.LocCode);
            return View(itemMasters);
        }


        public IActionResult ConfirmSelectLoc(JsPickingData jsArr)
        {
            CommonResponse<int> common = new();
            try
            {
                //Test response
                //string s = jsArr[0].id + ", " + jsArr[0].whcd + ", " + jsArr[0].qty;
                //return Json(s);
                var arr = jsArr;
                //   3. Add holding to To-Loc
                var toLoc = _db.Locs.Find(arr.loc);

                //Get Item master
                var pt = _db.ItemMasters.Find(arr.id);
                var fromLoc = _db.Locs.Find(pt.LocCode);

                //   2.Add holding to From-item
                pt.PtHold += arr.qty;


                toLoc.LocHolding += arr.qty;

                //   4. Add Outgo to From-Loc
                fromLoc.LocOutgo += arr.qty;


                _db.AllocateRequests.Add(new AllocateRequest()
                {
                    PtId = pt.PtId,
                    AlcFrom = pt.LocCode,
                    LocCode = arr.loc,
                    AlcQty = arr.qty,
                    AlcFromDesc = fromLoc.LocDesc,
                    MovementTime = arr.reqTime
                });

                _db.SaveChanges();
                common.status = 1;
                common.message += "Success";


            }
            catch (Exception e)
            {
                common.status = -1;
                common.message = e.ToString();
            }
            return Json(common);
        }

        public IActionResult Orders()
        {

            return View(_service.GetAllocateOrders());
        }

        public IActionResult UnAllocated()
        {

            return View(_service.GetUnAllocated());
        }

        public async Task<ActionResult> ConfirmLoc(int id)
        {
            var ptmstr = await _db.ItemMasters.FindAsync(id);
            var locList = new List<LocationCapSelect>();
            var whl = await _db.Locs.Where(x => x.LocCode != ptmstr.LocCode && x.LocType != "IssueLoc").ToListAsync();

            ViewBag.ptid = id;

            foreach (var item in whl)
            {
                locList.Add(new LocationCapSelect()
                {
                    Ptid = id,
                    LcpId = item.LocCode,
                    Holding = item.LocHolding,
                    RemainCapacity = item.LocRemain,
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
            fromLoc.LocOutgo += t.qty;


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
