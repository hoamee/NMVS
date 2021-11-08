using NMVS.Models;
using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public class RequestService : IRequestService
    {
        private readonly ApplicationDbContext _db;

        public RequestService(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<ItemAvailVm> GetItemAvails(string id)
        {
            var s = (from i in _db.ItemMasters.Where(x => x.ItemNo == id)
                     join d in _db.ItemDatas on i.ItemNo equals d.ItemNo
                     select new ItemAvailVm
                     {
                         ItemNo = i.ItemNo,
                         sDateIn = i.PtDateIn.Date.ToString(),
                         Desc = d.ItemName,
                         Quantity = i.PtQty - i.PtHold
                     }).ToList();

            return s;
        }

        public List<ItemMasterVm> GetItemMasterVms(RequestDet rq)
        {

            var ptMstr = _db.ItemMasters.Where(i => i.PtQty > 0 && i.ItemNo == rq.ItemNo).OrderBy(x => x.PtDateIn)
                .ToList();
            var itemVm = (from pt in ptMstr
                          join dt in _db.ItemDatas.ToList() on pt.ItemNo equals dt.ItemNo
                          join loc in _db.Locs.ToList() on pt.LocCode equals loc.LocCode
                          select new ItemMasterVm
                          {
                              Booked = pt.PtHold,
                              DateIn = pt.PtDateIn,
                              Loc = loc.LocDesc,
                              Lot = pt.PtLotNo,
                              Name = dt.ItemName,
                              No = dt.ItemNo,
                              PackingType = dt.ItemPkg,
                              Ptid = pt.PtId,
                              Qty = pt.PtQty,
                              Sup = pt.SupCode
                          }).ToList();


            return itemVm;
        }

        public RequestDetailVm GetRequestDetail(string id)
        {
            var rqs = (from i in _db.InvRequests.Where(x => x.RqID == id)
                       select new InvRequestVm
                       {
                           RqType = i.RqType,
                           Date = i.RqDate,
                           Id = i.RqID,
                           Note = i.RqCmt,
                           Ref = i.Ref,
                           RqBy = i.RqBy,
                           SoConfirm = i.SoConfirm,
                           ConfirmationNote = i.ConfirmationNote,
                           Confirmed = i.Confirmed,
                           ConfirmedBy = i.ConfirmedBy
                       }).FirstOrDefault();

            var dets = _db.RequestDets.Where(x => x.RqID == id).ToList();


            return new RequestDetailVm
            {
                Rq = rqs,
                Dets = dets,
            };

        }

        public List<InvRequestVm> GetRequestList()
        {
            var model = (from i in _db.InvRequests
                         select new InvRequestVm
                         {
                             RqType = i.RqType,
                             Date = i.RqDate,
                             Id = i.RqID,
                             Note = i.RqCmt,
                             Ref = i.Ref,
                             RqBy = i.RqBy,
                             SoConfirm = i.SoConfirm
                         }).ToList();


            return model;
        }
    }
}
