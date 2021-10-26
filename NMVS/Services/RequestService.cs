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
                           SoConfirm = i.SoConfirm
                       }).FirstOrDefault();

            var dets = _db.RequestDets.Where(x=>x.RqID == id).ToList();


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
