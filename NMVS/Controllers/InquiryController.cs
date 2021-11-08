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
        private readonly ApplicationDbContext _db;

        public InquiryController(ApplicationDbContext context, ISoService soService)
        {
            _db = context;
            _soService = soService;
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
    }
}
