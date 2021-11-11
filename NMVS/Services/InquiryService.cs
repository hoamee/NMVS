using NMVS.Models;
using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public class InquiryService : IInquiryService
    {
        private readonly ApplicationDbContext _db;
        public InquiryService(ApplicationDbContext db)
        {
            _db = db;
        }

        public LocDetailVm GetItemByLoc(string id)
        {
            LocDetailVm locDet = new();
            var location = _db.Locs.Find(id);
            var ls = (from item in _db.ItemMasters.Where(x => x.LocCode == id)
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
                          RcvBy = item.RecBy
                      }).ToList();

            var hold = _db.AllocateOrders.Where(x => x.LocCode == location.LocCode && x.Confirm == null).Sum(x => x.AlcOrdQty - x.MovedQty)
                    + _db.IssueOrders.Where(x => x.ToLoc == location.LocCode && x.Confirm == null).Sum(x => x.ExpOrdQty - x.MovedQty);
            var cap = location.LocCap;
            var outGo = _db.ItemMasters.Where(x => x.LocCode == location.LocCode && x.PtQty > 0).Sum(x => x.PtHold);
            var used = _db.ItemMasters.Where(x => x.LocCode == location.LocCode && x.PtQty > 0).Sum(x => x.PtQty - x.PtHold);

            locDet.Loc = new WarehouseVm
            {
                Cap = cap,
                Remain = cap - hold - outGo - used,
                Hold = hold,
                OutGo = outGo,
                Status = location.LocStatus,
                Used = used,
                WhCode = location.LocCode,
                WhName = location.LocDesc
            };
            locDet.Inv = ls;
            return locDet;
        }

        public List<MovementVm> GetMovementReport()
        {
            var issue = (from o in _db.IssueOrders
                         join dt in _db.ItemDatas on o.ItemNo equals dt.ItemNo into itd
                         from itdt in itd
                         join loc in _db.Locs on o.LocCode equals loc.LocCode into oloc
                         from ol in oloc.DefaultIfEmpty()
                         join ve in _db.Shippers on o.ToVehicle equals ve.ShpId into vo
                         from all in vo.DefaultIfEmpty()
                         join loc2 in _db.Locs on o.ToLoc equals loc2.LocCode into oloc2
                         from all2 in oloc2.DefaultIfEmpty()
                         select new MovementVm
                         {
                             ItemName = itdt.ItemName,
                             OrderType = "Issue",
                             RequestNo = o.RqID,
                             PtId = o.PtId,
                             OrderBy = o.OrderBy,
                             MovementTime = o.MovementTime,
                             CompletedTime = o.CompletedTime,
                             CompletedQty = o.MovedQty,
                             From = o.LocCode + " - " + ol.LocDesc,
                             To = o.IssueType == "MFG" ? o.ToLoc + " - " + all2.LocDesc : all.ShpDesc + "(id: " + all.ShpId + ")",
                             Completed = o.Confirm,
                             MoveBy = o.ConfirmedBy,
                             Id = o.ExpOrdId,
                             OrderQty = o.ExpOrdQty,
                             ReportedQty = o.Reported,
                             Note = o.Note,
                             ItemNo = o.ItemNo

                         }).ToList();

            var oders = (from or in _db.AllocateOrders
                         join loc in _db.Locs on or.LocCode equals loc.LocCode
                         join pt in _db.ItemMasters on or.PtId equals pt.PtId
                         join dt in _db.ItemDatas on pt.ItemNo equals dt.ItemNo
                         select new MovementVm
                         {
                             RequestNo = or.RequestID.ToString(),
                             From = or.AlcOrdFrom + " - " + or.AlcOrdFDesc,
                             To = or.LocCode + " - " + loc.LocDesc,
                             MoveBy = or.ConfirmedBy,
                             Completed = or.Confirm,
                             ItemNo = pt.ItemNo,
                             MovementTime = or.MovementTime,
                             Id = or.AlcOrdId,
                             PtId = or.PtId,
                             OrderQty = or.AlcOrdQty,
                             CompletedQty = or.MovedQty,
                             ReportedQty = or.Reported,
                             CompletedTime = or.CompletedTime,
                             Note = or.MovementNote,
                             OrderBy = or.OrderBy,
                             OrderType = "Allocate",
                             ItemName = dt.ItemName

                         }).ToList();

            return oders.Concat(issue).ToList();
        }

        public List<WarehouseVm> GetWarehouseData()
        {

            List<WarehouseVm> warehouseVms = new();
            var listWh = _db.Warehouses.Where(x => x.Type != "MFG").ToList();
            var listLoc = _db.Locs.Where(x => x.LocType != "MFG").ToList();
            foreach (var wh in listWh)
            {
                double cap = 0;
                double used = 0;
                double hold = 0;
                double outGo = 0;
                foreach (var loc in listLoc.Where(x => x.WhCode == wh.WhCode))
                {
                    hold += _db.AllocateOrders.Where(x => x.LocCode == loc.LocCode && x.Confirm == null).Sum(x => x.AlcOrdQty - x.MovedQty)
                        + _db.IssueOrders.Where(x => x.ToLoc == loc.LocCode && x.Confirm == null).Sum(x => x.ExpOrdQty - x.MovedQty);
                    cap += loc.LocCap;
                    outGo += _db.ItemMasters.Where(x => x.LocCode == loc.LocCode && x.PtQty > 0).Sum(x => x.PtHold);
                    used += _db.ItemMasters.Where(x => x.LocCode == loc.LocCode && x.PtQty > 0).Sum(x => x.PtQty - x.PtHold);
                }
                warehouseVms.Add(new WarehouseVm
                {
                    Cap = cap,
                    Remain = cap - hold - outGo - used,
                    Hold = hold,
                    Site = wh.SiCode,
                    OutGo = outGo,
                    Status = wh.WhStatus,
                    Used = used,
                    WhCode = wh.WhCode,
                    WhName = wh.WhDesc
                });

            }


            return warehouseVms;
        }

        public WarehouseDetailVm GetWarehouseDetail(string id)
        {
            WarehouseDetailVm warehouseDetail = new();
            List<WarehouseVm> locs = new();
            var wh = _db.Warehouses.Where(x => x.WhCode == id).FirstOrDefault();
            var listLoc = _db.Locs.Where(x => x.LocType != "MFG").ToList();

            double icap = 0;
            double iused = 0;
            double ihold = 0;
            double ioutGo = 0;
            foreach (var loc in listLoc.Where(x => x.WhCode == wh.WhCode))
            {
                var hold = _db.AllocateOrders.Where(x => x.LocCode == loc.LocCode && x.Confirm == null).Sum(x => x.AlcOrdQty - x.MovedQty)
                    + _db.IssueOrders.Where(x => x.ToLoc == loc.LocCode && x.Confirm == null).Sum(x => x.ExpOrdQty - x.MovedQty);
                var cap = loc.LocCap;
                var outGo = _db.ItemMasters.Where(x => x.LocCode == loc.LocCode && x.PtQty > 0).Sum(x => x.PtHold);
                var used = _db.ItemMasters.Where(x => x.LocCode == loc.LocCode && x.PtQty > 0).Sum(x => x.PtQty - x.PtHold);

                locs.Add(new WarehouseVm
                {
                    Cap = cap,
                    Remain = cap - hold - outGo - used,
                    Hold = hold,
                    OutGo = outGo,
                    Status = loc.LocStatus,
                    Used = used,
                    WhCode = loc.LocCode,
                    WhName = loc.LocDesc
                });

                ihold += hold;
                icap += cap;
                ioutGo += outGo;
                iused += used;

            }

            warehouseDetail.Wh = new WarehouseVm
            {
                Cap = icap,
                Remain = icap - ihold - ioutGo - iused,
                Hold = ihold,
                Site = wh.SiCode,
                OutGo = ioutGo,
                Status = wh.WhStatus,
                Used = iused,
                WhCode = wh.WhCode,
                WhName = wh.WhDesc
            };
            warehouseDetail.Locs = locs;


            return warehouseDetail;
        }
    }
}
