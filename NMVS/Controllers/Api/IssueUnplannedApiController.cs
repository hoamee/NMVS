
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NMVS.Models;
using NMVS.Models.DbModels;

namespace NMVS.Controllers.Api
{
    [ApiController]
    [Route("api/UnplannedIssue")]
    public class IssueUnplannedApiController : Controller
    {
        private readonly ApplicationDbContext _context;
        public IssueUnplannedApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("getSoList")]
        public IActionResult GetSoList()
        {
            var soList = _context.SalesOrders.Where(x =>
            x.Closed == true
            && x.Confirm == true
            && x.Completed == false
            )
            .Select(x => x.SoNbr);
            var confirmedRequestList = _context.InvRequests.Where(x=>soList.Contains(x.RqID) && x.Confirmed == true).Select(s=>s.RqID);
            return Ok(confirmedRequestList);
        }

        [HttpGet]
        [Route("getItemList/{rqNbr}")]
        public IActionResult GetItemList(string rqNbr)
        {

            var ls = (from i in _context.RequestDets.Where(x => x.RqID == rqNbr)
                      join dt in _context.ItemDatas on i.ItemNo equals dt.ItemNo into idt
                      from all in idt.DefaultIfEmpty()
                      select new
                      {
                          itemNo = i.ItemNo,
                          itemName = all.ItemName

                      }).ToList();
            return Ok(ls);
        }

        [HttpGet]
        [Route("GetLocationList/{itemNo}")]
        public IActionResult GetLocationList(string itemNo)
        {
            var workSpace = HttpContext.Session.GetString("susersite");
            var whs = _context.Warehouses.Where(x => x.SiCode == workSpace).Select(s => s.WhCode).ToList();
            var ls = new List<LocNQtyDto>();
            foreach (var loc in _context.Locs.Where(loc => whs.Contains(loc.WhCode)))
            {
                var qty = _context.ItemMasters.Where(x => x.LocCode == loc.LocCode && x.ItemNo == itemNo).Sum(s => s.PtQty - s.PtHold);
                if (qty > 0)
                {
                    ls.Add(new LocNQtyDto
                    {
                        LocCode = loc.LocCode,
                        LocName = loc.LocDesc,
                        Quantity = qty
                    });
                }
            }
            return Ok(ls);
        }

        [HttpGet]
        [Route("getvehicles")]
        public IActionResult GetVehicleList()
        {
            var workSpace = HttpContext.Session.GetString("susersite");
            var shpList = from i in _context.Shippers
                .Where(x => !string.IsNullOrEmpty(x.CheckInBy)
                    && string.IsNullOrEmpty(x.CheckOutBy)
                    && x.IssueConfirmed != true
                    && x.Site == workSpace)
                          select new
                          {
                              shpId = i.ShpId,
                              shpDesc = i.ShpDesc
                          }
                    ;
            return Ok(shpList);

        }

        [HttpPost]
        [Route("submitIssue")]
        public IActionResult SubmitIssue(ClientFormDto clientForm)
        {
            var qty = clientForm.qty;
            var itemNo = clientForm.itemNo;
            var user = HttpContext.User.Identity.Name;
            var workSpace = HttpContext.Session.GetString("susersite");

            //update sales order
            var sodet = new SoDetail
            {
                ItemNo = itemNo,
                Discount = 0,
                NetPrice = 0,
                Quantity = qty,
                Shipped = qty,
                SoNbr = clientForm.soNbr,
                Tax = 0,
                RequiredDate = DateTime.Now
            };

            //update inventory request
            var requestDet = new RequestDet
            {
                Closed = true,
                Arranged = qty,
                Issued = qty,
                IssueOn = DateTime.Now,
                ItemNo = clientForm.itemNo,
                Picked = qty,
                ItemName = _context.ItemDatas.Find(clientForm.itemNo).ItemName,
                Quantity = qty,
                Ready = qty,
                MovementNote = "Unplanned issue by " + user,
                RequireDate = DateTime.Now,
                RqID = clientForm.soNbr,
                Site = HttpContext.Session.GetString("susersite"),
            };
            _context.Add(requestDet);
            _context.Add(sodet);
            _context.SaveChanges();
            requestDet.SodId = sodet.SodId;
            sodet.RqDetId = requestDet.DetId;
            _context.Update(requestDet);
            _context.Update(sodet);

            var pt = _context.ItemMasters.FirstOrDefault(x => x.ItemNo == clientForm.itemNo && x.LocCode == clientForm.location);
            var shp = _context.Shippers.Find(clientForm.vehicle);

            var order = new IssueOrder()
            {
                ExpOrdQty = qty,
                IssueType = "Issue",
                ItemNo = itemNo,
                LocCode = clientForm.location,
                OrderBy = User.Identity.Name,
                ToVehicle = clientForm.vehicle,
                ToLoc = "",
                DetId = requestDet.DetId,
                RqID = clientForm.soNbr,
                PtId = pt.PtId,
                MovementTime = DateTime.Now,
                Site = workSpace,
                CompletedTime = DateTime.Now,
                Confirm = true,
                ConfirmedBy = user,
                IssueToDesc = shp.ShpDesc,
                MovedQty = qty,
                Reported = 0,
                Note = "Unplanned issue by " + user
            };
            _context.IssueOrders.Add(order);
            pt.PtQty -= qty;
            _context.SaveChanges();

            //add shipper detail
            var shpDet = _context.ShipperDets.FirstOrDefault(x => x.DetId == order.DetId && x.ShpId == order.ToVehicle);
            if (shpDet == null)
            {
                _context.Add(new ShipperDet
                {
                    InventoryId = order.PtId,
                    DetId = order.DetId,
                    ItemNo = pt.ItemNo,
                    Quantity = qty,
                    RqId = order.RqID,
                    ShpId = clientForm.vehicle
                });


            }
            else
            {
                shpDet.Quantity += qty;
                _context.Update(shpDet);
            }
            _context.Add(new InventoryTransac
            {
                From = pt.LocCode,
                To = "Shipper Id: " + shp.ShpId,
                LastId = pt.PtId,
                NewId = null,
                OrderNo = order.ExpOrdId,
                IsAllocate = false,
                IsDisposed = false,
                MovementTime = DateTime.Now
            });

            _context.SaveChanges();

            return Ok(new
            {
                rqDet = requestDet,
                sod = sodet
            });
        }

        //DTO
        private class LocNQtyDto
        {
            public string LocCode { get; set; }
            public string LocName { get; set; }
            public double Quantity { get; set; }
        }
        public class ClientFormDto
        {
            public string soNbr { set; get; }
            public string itemNo { get; set; }
            public double qty { get; set; }
            public string location { get; set; }
            public int vehicle { get; set; }
        }
    }
}