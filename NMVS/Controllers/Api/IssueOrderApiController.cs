using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var workSpace = HttpContext.Session.GetString("susersite");
            var model = (from o in _db.IssueOrders.Where(x => x.IssueType == "MFG" && x.Confirm != true && x.Site == workSpace)
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
            var workSpace = HttpContext.Session.GetString("susersite");
            var isd = _db.IssueOrders.Where(x => x.ToVehicle != null && x.Site == workSpace).ToList();

            var model = (from ve in _db.Shippers.Where(x => !string.IsNullOrEmpty(x.CheckInBy) && string.IsNullOrEmpty(x.CheckOutBy)).ToList()
                         select new IssueToVehicleDto
                         {
                             Id = ve.ShpId,
                             Completed = isd.Where(x => x.ToVehicle == ve.ShpId && x.Confirm == true).Count(),
                             DateIn = ((DateTime)ve.ActualIn).ToString("dd-MM-yy HH:mm:ss"),
                             Desc = ve.ShpDesc,
                             Total = isd.Where(x => x.ToVehicle == ve.ShpId).Count()
                         }).ToList();

            return Ok(model);
        }

        [HttpGet]
        [Route("GetVeDetail/{id}")]
        public IActionResult GetVeDetail(int id)
        {
            var workSpace = HttpContext.Session.GetString("susersite");
            var issueOrders = (from o in _db.IssueOrders.Where(x => x.IssueType == "Issue" && x.ToVehicle == id && x.Site == workSpace)
                               join loc in _db.Locs on o.LocCode equals loc.LocCode into oloc
                               from ol in oloc.DefaultIfEmpty()
                               join i in _db.ItemDatas on o.ItemNo equals i.ItemNo into it
                               from idt in it.DefaultIfEmpty()
                               select new IssueOrderDto
                               {
                                   Item = idt.ItemName,
                                   RequestNo = o.RqID,
                                   InventoryId = o.PtId,
                                   OrderBy = o.OrderBy,
                                   Time = o.MovementTime.ToString("dd-MM-yy HH:mm:ss"),
                                   Moved = o.MovedQty,
                                   FromCode = o.LocCode,
                                   Confirmed = o.Confirm,
                                   ConfirmedBy = o.ConfirmedBy,
                                   IssueOrderId = o.ExpOrdId,
                                   Quantity = o.ExpOrdQty,
                                   From = ol.LocDesc,
                                   Reported = o.Reported

                               }).ToList();



            return Ok(issueOrders);
        }

        [HttpGet]
        [Route("GetVeInfo/{id}")]
        public IActionResult GetVeInfo(int id)
        {
            var model = _db.Shippers.Find(id);

            return Ok(model);
        }
    }
}