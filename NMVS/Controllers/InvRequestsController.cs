﻿using Microsoft.AspNetCore.Authorization;
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

        public IActionResult RequestDetCreate(string id)
        {
            List<ItemAvailVm> ls = new();
            var ptMstr = _db.ItemMasters.ToList();
            foreach (var item in _db.ItemDatas.ToList())
            {
                var pt = ptMstr.Where(x => x.ItemNo == item.ItemNo);
                double avail = 0;
                if (pt.Any())
                {
                    var hold = pt.Sum(x => x.PtHold);
                    var qty = pt.Sum(x => x.PtQty);
                    avail = qty - hold;
                }

                ls.Add(new ItemAvailVm
                {
                    ItemNo = item.ItemNo,
                    Quantity = avail,
                    Desc = item.ItemName
                });
            }

            ViewBag.ItemList = ls;

            ViewBag.RqId = id;

            return View();
        }

        [HttpPost]
        public IActionResult RequestDetCreate(RequestDet det)
        {
            if (ModelState.IsValid)
            {
                bool valid = true;
                if (det.SpecDate != null)
                {
                    var pt = _db.ItemMasters.Where(x => x.ItemNo == det.ItemNo && det.SpecDate == x.PtDateIn.Date).Sum(x => x.PtQty - x.PtHold);

                    if (pt < det.Quantity || det.Quantity <= 0)
                    {
                        ModelState.AddModelError("", "Quantity should be less or equal to available quantity. And greater than 0");
                        valid = false;
                    }
                }

                if (valid)
                {
                    det.Arranged = 0;
                    det.Issued = 0;
                    det.Picked = 0;
                    _db.RequestDets.Add(det);
                    _db.SaveChanges();
                    return RedirectToAction("RequestDetail", new { id = det.RqID });
                }
            }

            List<ItemAvailVm> ls = new();
            var ptMstr = _db.ItemMasters.ToList();
            foreach (var item in _db.ItemDatas.ToList())
            {
                var pt = ptMstr.Where(x => x.ItemNo == item.ItemNo);
                double avail = 0;
                if (pt.Any())
                {
                    var hold = pt.Sum(x => x.PtHold);
                    var qty = pt.Sum(x => x.PtQty);
                    avail = qty - hold;
                }

                ls.Add(new ItemAvailVm
                {
                    ItemNo = item.ItemNo,
                    Quantity = avail,
                    Desc = item.ItemName
                });
            }

            ViewBag.ItemList = ls;

            ViewBag.RqId = det.RqID;

            return View();
        }

        public IActionResult GetItemMaster(string id)
        {
            CommonResponse<List<ItemAvailVm>> commonResponse = new();

            commonResponse.dataenum = _service.GetItemAvails(id);
            if (commonResponse.dataenum.Any())
            {
                commonResponse.status = 1;
                commonResponse.message = "OK";
            }
            else
            {
                commonResponse.status = -1;
                commonResponse.message = "No available items in warehouse";
            }

            return Ok(commonResponse);

        }

        public IActionResult PickListSO(int id)
        {
            var rq = _db.RequestDets.Find(id);

            ViewBag.DetId = id;
            ViewBag.RqId = rq.RqID;
            ViewBag.qty = rq.Quantity - rq.Picked;
            ViewBag.History = _db.IssueOrders.Where(x => x.DetId == id).ToList();

            ViewBag.LocList = new SelectList(_db.Shippers
                .Where(x => string.IsNullOrEmpty(x.CheckOutBy))
                .ToList(), "ShpId", "ShpDesc");

            if (rq.SpecDate == null)
            {
                return View(_service.GetItemMasterVms(rq));
            }
            else
            {
                return View(_service.GetItemMasterVms(rq).Where(x => x.DateIn.Date == rq.SpecDate));
            }
        }

        public IActionResult PickListMFG(int id)
        {
            var rq = _db.RequestDets.Find(id);

            ViewBag.DetId = id;
            ViewBag.RqId = rq.RqID;
            ViewBag.qty = rq.Quantity - rq.Picked;
            ViewBag.History = _db.IssueOrders.Where(x => x.DetId == id).ToList();

            ViewBag.LocList = new SelectList(_db.Locs
                .Where(x => x.LocType == "MFG")
                .ToList(), "LocCode", "LocDesc");

            if (rq.SpecDate == null)
            {
                return View(_service.GetItemMasterVms(rq));
            }
            else
            {
                return View(_service.GetItemMasterVms(rq).Where(x => x.DateIn.Date == rq.SpecDate));
            }
        }

        public IActionResult IssueNoteMfg()
        {
            var model = _db.MfgIssueNotes.ToList();
            return View(model);
        }

        public IActionResult MfgDetail(int id)
        {
            var isn = _db.MfgIssueNotes.Find(id);

            var noteDet = (from d in _db.IssueNoteDets
                           join i in _db.ItemDatas on d.ItemNo equals i.ItemNo into all
                           from a in all.DefaultIfEmpty()
                           select new MfgIssueNoteDet
                           {
                               ItemNo = d.ItemNo,
                               Id = d.Id,
                               ItemName = a.ItemName,
                               IsNId = d.IsNId,
                               PtId = d.PtId,
                               Quantity = d.Quantity
                           }).ToList();

            var model = new MfgNoteVm
            {
                Det = noteDet,
                MfgIssueNote = isn
            };

            return View(model);
        }


        public IActionResult IssueNoteSo() => View(_db.Shippers.ToList());

        public IActionResult SoNoteDetails(int id)
        {

            var shp = _db.Shippers.Find(id);

            var noteDet = (from d in _db.ShipperDets
                           join i in _db.ItemDatas on d.ItemNo equals i.ItemNo into all
                           from a in all.DefaultIfEmpty()                           
                           join s in _db.SalesOrders on d.RqId equals s.SoNbr into sOrder
                           from so in sOrder.DefaultIfEmpty()
                           join c in _db.Customers on so.CustCode equals c.CustCode into soldTo
                           from soto in soldTo.DefaultIfEmpty()
                           join c2 in _db.Customers on so.ShipTo equals c2.CustCode into shipTo
                           from shto in shipTo.DefaultIfEmpty()
                           select new ShipperDet
                           {
                               InventoryId = d.InventoryId,
                               DetId = d.DetId,
                               ItemName = a.ItemName,
                               ItemNo = a.ItemNo,
                               Quantity = d.Quantity,
                               RqId = d.RqId,
                               ShpId = d.ShpId,
                               SoldTo = soto.CustCode,
                               SoldToName = soto.CustName,
                               ShipToId = shto.CustCode,
                               ShipToAddr = shto.Addr,
                               ShipToName = shto.CustName
                           }).ToList();

            var model = new IssueNoteShipperVm
            {
                Det = noteDet,
                Shp = shp
            };
            return View(model);
        }
    }
}