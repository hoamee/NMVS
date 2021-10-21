using NMVS.Models;
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

        public List<ItemMasterVm> GetAvailItem(string itemNo)
        {
            var ls = (from item in _db.ItemMasters.Where(x => x.ItemNo == itemNo && !string.IsNullOrEmpty(x.LocCode))
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
                          No = itemNo,
                          Ptid = item.PtId,
                          Qty = item.PtQty
                      }).ToList();

            return ls;
        }

        public List<TypeVm> GetItemDistinc()
        {
            var li = (from item in _db.ItemMasters.Select(x => x.ItemNo).Distinct()
                      join dt in _db.ItemDatas on item equals dt.ItemNo
                      select new TypeVm
                      {
                          Code = item,
                          Desc = dt.ItemName
                      }).ToList();

            return li;
        }

        public List<AllocateRequestVm> GetRequestList()
        {
            var model = (from al in _db.AllocateRequests
                         join loc in _db.Locs on al.LocCode equals loc.LocCode
                         join loc2 in _db.Locs on al.AlcFrom equals loc2.LocCode
                         join pt in _db.ItemMasters on al.PtId equals pt.PtId
                         join dt in _db.ItemDatas on pt.ItemNo equals dt.ItemNo
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
    }
}
