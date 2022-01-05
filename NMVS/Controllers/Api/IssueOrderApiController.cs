using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NMVS.Models;
using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;

namespace NMVS.Controllers.Api
{
    [ApiController]
    [Route("api/IssueOrder")]
    public class IssueOrderApiController : Controller
    {
        private readonly ApplicationDbContext _db;
        public IssueOrderApiController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [Route("GetMfgOrders")]
        public IActionResult GetMfgOrders()
        {
            var model = (from o in _db.IssueOrders.Where(x => x.IssueType == "MFG")
                         join loc in _db.Locs on o.LocCode equals loc.LocCode into oloc
                         from ol in oloc.DefaultIfEmpty()
                         join i in _db.ItemDatas on o.ItemNo equals i.ItemNo into it
                         from idt in it.DefaultIfEmpty()
                         join loc2 in _db.Locs on o.ToLoc equals loc2.LocCode into oloc2
                         from all2 in oloc2.DefaultIfEmpty()
                         select new IssueOrderDto
                         {
                             Item = idt.ItemName,
                             RequestNo = o.RqID,
                             InventoryId = o.PtId,
                             OrderBy = o.OrderBy,
                             Time = o.MovementTime.ToString("dd-MM-yy HH:mm:ss"),
                             Moved = o.MovedQty,
                             FromCode = o.LocCode,
                             To = all2.LocDesc,
                             Confirmed = o.Confirm,
                             ConfirmedBy = o.ConfirmedBy,
                             IssueOrderId = o.ExpOrdId,
                             Quantity = o.ExpOrdQty,
                             ToCode = o.ToLoc,
                             From = ol.LocDesc,
                             Reported = o.Reported

                         }).ToList();

            return Ok(model);
        }

        [HttpGet]
        [Route("GetListVehicle")]
        public IActionResult GetListVehicle()
        {

            var model = (from ve in _db.Shippers
                         join isd in _db.IssueOrders on ve.ShpId equals isd.ToVehicle into veis
                         from all in veis.DefaultIfEmpty()
                         group all by ve.ShpId into grouped

                         select new IssueToVehicleDto
                         {
                            //  Id = grouped.Count,
                            //  Completed = isd.,
                            //  Date = ((DateTime)ve.ActualIn).ToString("dd-MM-yy HH:mm:ss"),
                            //  Desc = ve.ShpDesc,
                            //  Status = ve.ActualOut == null,
                            //  Total = a.Count()
                         }).ToList();

            return Ok(model);
        }
    }
}