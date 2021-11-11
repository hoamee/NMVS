using Microsoft.AspNetCore.Mvc;
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
    public class InquiryController : Controller
    {

        private readonly ISoService _soService;
        private readonly IInquiryService _inquiryService;
        private readonly ApplicationDbContext _db;

        public InquiryController(ApplicationDbContext context, ISoService soService, IInquiryService inquiryService)
        {
            _db = context;
            _soService = soService;
            _inquiryService = inquiryService;
        }

        public IActionResult Inventory()
        {
            ViewBag.ItemList = _soService.GetItemNAvail();
            return View();
        }

        public IActionResult InventoryDetail(int id)
        {
            var model = new InventoryDetail
            {
                Transacs = _db.InventoryTransacs.Where(x => x.LastId == id || (x.NewId == id && !x.IsDisposed)).ToList(),
                ItemMasterVm = (from item in _db.ItemMasters.Where(x => !string.IsNullOrEmpty(x.LocCode) && x.PtId == id)
                              join dt in _db.ItemDatas on item.ItemNo equals dt.ItemNo into itemData

                              from i in itemData.DefaultIfEmpty()
                              join loc in _db.Locs on item.LocCode equals loc.LocCode into fullData

                              from f in fullData.DefaultIfEmpty()
                              join sup in _db.Suppliers on item.SupCode equals sup.SupCode into suppliers

                              from s in suppliers.DefaultIfEmpty()

                              select new ItemMasterVm
                              {
                                  Booked = item.PtHold,
                                  DateIn = item.PtDateIn,
                                  Loc = f.LocDesc + " (" + f.LocCode + ")",
                                  Name = i.ItemName,
                                  No = i.ItemNo,
                                  Ptid = item.PtId,
                                  Qty = item.PtQty,
                                  PackingType = i.ItemPkg,
                                  Sup = s.SupDesc,
                                  Lot = item.PtLotNo,
                                  RcvBy = item.RecBy,
                                  Parent = item.ParentId
                              }).FirstOrDefault()
            };
            return View(model);
        }

        public IActionResult Warehouses()
        {
            var model = _inquiryService.GetWarehouseData();
            ViewBag.WhName = model.Select(x => x.WhName).ToArray();
            ViewBag.Remain = model.Select(x=>x.Remain).ToArray();
            ViewBag.Hold = model.Select(x => x.Hold).ToArray();
            ViewBag.Used = model.Select(x => x.Used).ToArray();
            ViewBag.OutGo = model.Select(x => x.OutGo).ToArray();
            return View(model);
        }

        public IActionResult WhDetail(string id)
        {
            var model = _inquiryService.GetWarehouseDetail(id);
            ViewBag.WhName = model.Locs.Select(x => x.WhName).ToArray();
            ViewBag.Remain = model.Locs.Select(x => x.Remain).ToArray();
            ViewBag.Hold = model.Locs.Select(x => x.Hold).ToArray();
            ViewBag.Used = model.Locs.Select(x => x.Used).ToArray();
            ViewBag.OutGo = model.Locs.Select(x => x.OutGo).ToArray();
            return View(model);
        }

        public IActionResult LocDetail(string id)
        {
            var model = _inquiryService.GetItemByLoc(id);
            return View(model);
        }

        public IActionResult MovementReport(string id)
        {
            ViewBag.SearchValue = " ";
            if (!string.IsNullOrEmpty(id))
            {
                ViewBag.SearchValue = id;
            }
            var model = _inquiryService.GetMovementReport();
            return View(model);
        }
    }
}
