﻿using NMVS.Models;
using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public class AllocateService : IAllocateService
    {
        private readonly ApplicationDbContext _db;

        public AllocateService(ApplicationDbContext db)
        {
            _db = db;
        }

        public CommonResponse<int> ConfirmSelectLoc(List<JsPickingData> jsArr, string wp)
        {
            CommonResponse<int> common = new();

            try
            {
                //Test response
                //string s = jsArr[0].id + ", " + jsArr[0].whcd + ", " + jsArr[0].qty;
                //return Json(s);
                foreach (var arr in jsArr)
                {

                    //   3. Add holding to To-Loc
                    var toLoc = _db.Locs.Find(arr.loc);


                    //Get Item master
                    var pt = _db.ItemMasters.Find(arr.id);
                    var fromLoc = _db.Locs.Find(pt.LocCode);

                    //   2.Add holding to From-item
                    pt.PtHold += arr.qty;


                    toLoc.LocHolding += arr.qty;

                    //   4. Add Outgo to From-Loc
                    //fromLoc.LocOutgo += arr.qty;


                    _db.AllocateRequests.Add(new AllocateRequest()
                    {
                        PtId = pt.PtId,
                        AlcFrom = pt.LocCode,
                        LocCode = arr.loc,
                        AlcQty = arr.qty,
                        AlcFromDesc = fromLoc.LocDesc,
                        MovementTime = arr.reqTime,
                        Site = wp
                    });
                    _db.Update(fromLoc);
                    _db.Update(toLoc);
                    _db.Update(pt);
                    _db.SaveChanges();
                    common.status = 1;
                    common.message = "Success";
                }


            }
            catch (Exception e)
            {
                common.status = -1;
                common.message = e.ToString();
            }
            return common;
        }

        public List<AllocateOrderVm> GetAllocateOrders(string wp)
        {
            var model = (from or in _db.AllocateOrders.Where(x=>x.Confirm != true && x.Site == wp)
                         join loc in _db.Locs on or.LocCode equals loc.LocCode
                         join pt in _db.ItemMasters on or.PtId equals pt.PtId
                         join dt in _db.ItemDatas on pt.ItemNo equals dt.ItemNo
                         select new AllocateOrderVm
                         {
                             RequestId = or.RequestID,
                             AlcFrom = or.AlcOrdFrom + "(" + or.AlcOrdFDesc +")",
                             AlcToCode = or.LocCode,
                             AlcToDesc = loc.LocDesc,
                             ConfirmBy = or.ConfirmedBy,
                             Confirmed = or.Confirm,
                             ItemNo = pt.ItemNo,
                             ItemName = dt.ItemName,
                             MvmTime = or.MovementTime,
                             OrderId = or.AlcOrdId,
                             PtId = or.PtId,
                             Qty = or.AlcOrdQty,
                             Moved = or.MovedQty,
                             Reported = or.Reported
                         }).ToList();

            return model;
        }

        public List<ItemMasterVm> GetAvailItem(string locCode, string wp)
        {
            var ls = (from item in _db.ItemMasters.Where(x => !string.IsNullOrEmpty(x.LocCode) && x.LocCode != locCode && x.Site == wp)
                      join dt in _db.ItemDatas on item.ItemNo equals dt.ItemNo into itemData

                      from i in itemData.DefaultIfEmpty()
                      join loc in _db.Locs on item.LocCode equals loc.LocCode into fullData

                      from f in fullData.DefaultIfEmpty()
                      
                      select new ItemMasterVm
                      {
                          Booked = item.PtHold,
                          DateIn = item.PtDateIn,
                          Loc = f.LocDesc + " (" + f.LocCode + ")",
                          Name = i.ItemName,
                          No = item.ItemNo,
                          Ptid = item.PtId,
                          Qty = item.PtQty
                      }).ToList();

            return ls;
        }

        public List<AllocateRequestVm> GetItemAllocateHistory(int ptId)
        {
            var model = (from al in _db.AllocateRequests.Where(x=>x.PtId == ptId)
                         join loc in _db.Locs on al.LocCode equals loc.LocCode
                         join loc2 in _db.Locs on al.AlcFrom equals loc2.LocCode
                         join pt in _db.ItemMasters on al.PtId equals pt.PtId
                         join dt in _db.ItemDatas on pt.ItemNo equals dt.ItemNo
                         orderby al.IsClosed descending
                         select new AllocateRequestVm
                         {
                             AlcFrom = loc2.LocCode + "(" + al.AlcFrom + ")",
                             AlcId = al.AlcId,
                             AlcQty = al.AlcQty,
                             AlcTo = loc.LocDesc,
                             IsClosed = al.IsClosed,
                             MovementTime = al.MovementTime,
                             Note = al.AlcCmmt,
                             PtId = al.PtId,
                             Item = dt.ItemName
                         }).ToList();

            return model;
        }

        public List<TypeVm> GetItemDistinc()
        {
            var li = (from item in _db.ItemMasters.Where(x=>x.PtQty > x.PtHold).Select(x => x.ItemNo).Distinct()
                      join dt in _db.ItemDatas on item equals dt.ItemNo
                      select new TypeVm
                      {
                          Code = item,
                          Desc = dt.ItemName
                      }).ToList();

            return li;
        }

        public List<AllocateRequestVm> GetRequestList(string wp)
        {
            var model = (from al in _db.AllocateRequests.Where(x=>x.Site == wp)
                         join loc in _db.Locs on al.LocCode equals loc.LocCode 
                         join loc2 in _db.Locs on al.AlcFrom equals loc2.LocCode into tempLoc
                         from t in tempLoc.DefaultIfEmpty()
                         join pt in _db.ItemMasters on al.PtId equals pt.PtId
                         join dt in _db.ItemDatas on pt.ItemNo equals dt.ItemNo
                         orderby al.IsClosed descending
                         select new AllocateRequestVm
                         {
                             AlcFrom =  (al.AlcFrom == "NA" ? al.AlcFromDesc : t.LocDesc ),
                             AlcId = al.AlcId,
                             AlcQty = al.AlcQty,
                             AlcTo = loc.LocDesc,
                             IsClosed = al.IsClosed,
                             MovementTime = al.MovementTime,
                             Note = al.AlcCmmt,
                             PtId = al.PtId,
                             Item = dt.ItemName,
                             Reported = al.Reported
                         }).ToList();

            return model;
        }

        public List<ItemMasterVm> GetUnAllocated(string wp)
        {
            
            var ls = (from item in _db.ItemMasters.Where(x => x.Passed == true && string.IsNullOrEmpty(x.LocCode) && x.PtQty > 0 && x.Site == wp)
                      join dt in _db.ItemDatas on item.ItemNo equals dt.ItemNo into itemData

                      from i in itemData.DefaultIfEmpty()
                      join loc in _db.IncomingLists on item.IcId equals loc.IcId into fullData

                      from f in fullData.DefaultIfEmpty()
                      join sup in _db.Suppliers on item.SupCode equals sup.SupCode into suppliers

                      from s in suppliers.DefaultIfEmpty()

                      select new ItemMasterVm
                      {
                          Booked = item.PtHold,
                          DateIn = item.PtDateIn,
                          Loc = f.Vehicle,
                          Name = i.ItemName,
                          No = i.ItemNo,
                          Ptid = item.PtId,
                          Qty = item.PtQty,
                          PackingType = i.ItemPkg,
                          Sup = s.SupDesc,
                          Lot = item.PtLotNo,
                          RcvBy = item.RecBy
                      }).ToList();

            return ls;
        }
    }
}
