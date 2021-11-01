using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NMVS.Common;
using NMVS.Models;
using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using NMVS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NMVS.Controllers.Api
{
    [Route("api/InvRequest")]
    [ApiController]
    public class InvRequestApiController : Controller
    {
        private readonly IIncomingService _service;
        private readonly IAllocateService _alcService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        ApplicationDbContext _context;

        public InvRequestApiController(IIncomingService service, IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, IAllocateService allocateService)
        {
            _context = context;
            _service = service;
            _httpContextAccessor = httpContextAccessor;
            _alcService = allocateService;
        }

        [HttpPost]
        [Route("PushPickListSo")]
        public async Task<IActionResult> PushPickListSo(List<JsPickingData> js)
        {
            var message = "";
            //Test response
            //string s = jsArr[1].id + ", " + jsArr[1].whcd + ", " + jsArr[1].qty;
            //return Json(id);

            //2.Get request data

            foreach (var jsArr in js)
            {
                try
                {
                    var request = _context.RequestDets.Where(x => x.DetId == jsArr.det).FirstOrDefault();
                    var shp = await _context.Shippers.FindAsync(jsArr.shipper);


                    var pt = await _context.ItemMasters.FindAsync(jsArr.id);
                    var fromLoc = await _context.Locs.FindAsync(pt.LocCode);
                    //   3. Add holding to From-item
                    pt.PtHold += jsArr.qty;

                    request.Picked += jsArr.qty;
                    // 4. Add Outgo to From-Loc
                    fromLoc.LocOutgo += jsArr.qty;


                    _context.IssueOrders.Add(new IssueOrder()
                    {
                        ExpOrdQty = jsArr.qty,
                        IssueType = shp == null ? "MFG" : "Issue",
                        ItemNo = pt.ItemNo,
                        LocCode = pt.LocCode,
                        OrderBy = User.Identity.Name,
                        ToVehicle = jsArr.shipper,
                        ToLoc = jsArr.whcd,
                        DetId = request.DetId,
                        RqID = request.RqID,
                        PtId = pt.PtId,
                        MovementTime = jsArr.reqTime
                    });

                    _context.Update(request);
                    _context.Update(pt);
                    _context.Update(fromLoc);
                    _context.SaveChanges();
                    message = "Success!";
                }
                catch (Exception e)
                {
                    message = e.ToString();
                }
            }

            return Ok(message);
        }

        [HttpPost]
        [Route("FinishIssueOrder")]
        public async Task<IActionResult> FinishIssueOrder(AllocateOrder alo)
        {
            var message = "Order not found!";

            var issueQty = alo.AlcOrdQty;

            var order = await _context.IssueOrders.FindAsync(alo.AlcOrdId);


            if (order != null)
            {

                //Check if shipper is checked out
                message = "Shipper is already checked out!";
                var shp = await _context.Shippers.FindAsync(order.ToVehicle);
                if (shp != null && order.IssueType == "Issue")
                {
                    if (!string.IsNullOrEmpty(shp.CheckOutBy))
                    {

                        return Ok(message);
                    }

                }
                //Check item exist
                message = "Item not found";
                var pt = await _context.ItemMasters.FindAsync(order.PtId);
                if (pt != null)
                {
                    pt.PtHold -= issueQty;
                    pt.PtQty -= issueQty;

                    if (pt.PtHold < 0 || pt.PtQty < 0)
                    {
                        message = "Item quantity error";
                        return Ok(message);
                    }

                    _context.Update(pt);


                    //check location
                    message = "From loc not found!";
                    var fromLoc = await _context.Locs.FindAsync(pt.LocCode);
                    if (fromLoc != null)
                    {
                        fromLoc.LocOutgo -= issueQty;
                        fromLoc.LocRemain += issueQty;

                        if (fromLoc.LocOutgo < 0 || fromLoc.LocRemain < 0)
                        {
                            message = "Location capacity error";
                            return Ok(message);
                        }
                        _context.Update(fromLoc);


                        //Check request det
                        var reDet = await _context.RequestDets.FindAsync(order.DetId);
                        message = "Request loc not found!";
                        if (reDet != null)
                        {
                            reDet.Arranged += issueQty;
                            _context.Update(reDet);
                            //
                            if (order.IssueType == "Issue")
                            {
                                var shpDet = _context.ShipperDets.FirstOrDefault(x => x.DetId == order.DetId && x.ShpId == order.ToVehicle);
                                if (shpDet == null)
                                {
                                    _context.Add(new ShipperDet
                                    {
                                        InventoryId = order.PtId,
                                        DetId = order.DetId,
                                        ItemNo = pt.ItemNo,
                                        Quantity = issueQty,
                                        RqId = order.RqID,
                                        ShpId = (int)order.ToVehicle
                                    });

                                }
                                else
                                {
                                    shpDet.Quantity += issueQty;
                                    _context.Update(shpDet);
                                }
                            }
                            else
                            {
                                var mfgIssueNote = _context.MfgIssueNotes.FirstOrDefault(x => x.RqId == order.RqID && string.IsNullOrEmpty(x.IssuedBy));
                                if (mfgIssueNote == null)
                                {
                                    var issueNote = new MfgIssueNote
                                    {
                                        RqId = order.RqID
                                    };

                                    _context.Add(issueNote);
                                    _context.SaveChanges();

                                    var det = new MfgIssueNoteDet
                                    {
                                        IsNId = issueNote.IsNId,
                                        ItemNo = order.ItemNo,
                                        PtId = order.PtId,
                                        Quantity = issueQty
                                    };
                                    _context.Add(det);
                                    _context.SaveChanges();

                                }
                                else
                                {
                                    var det = _context.IssueNoteDets.FirstOrDefault(x => x.IsNId == mfgIssueNote.IsNId && x.PtId == order.PtId && x.ItemNo == order.ItemNo);
                                    if (det == null)
                                    {
                                        _context.Add(new MfgIssueNoteDet
                                        {
                                            ItemNo = order.ItemNo,
                                            IsNId = mfgIssueNote.IsNId,
                                            PtId = order.PtId,
                                            Quantity = issueQty
                                        });
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        det.Quantity += issueQty;
                                    }

                                }

                            }

                            order.ConfirmedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
                            order.MovedQty += issueQty;
                            if (order.MovedQty >= order.ExpOrdQty)
                            {
                                order.Confirm = true;
                            }
                            _context.Update(order);
                            _context.SaveChanges();
                            message = "Success"!;
                        }


                    }

                }


            }

            return Ok(message);
        }

        [HttpPost]
        [Route("CloseNote")]
        public async Task<IActionResult> CloseNote(MfgIssueNote issueNote)
        {
            string message = "Success";
            try
            {
                var note = await _context.MfgIssueNotes.FindAsync(issueNote.IsNId);
                if (note != null)
                {
                    note.IssuedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
                    note.IssuedOn = DateTime.Now;
                    _context.Update(note);
                    _context.SaveChanges();
                }
                else
                {
                    message = "Not found!";
                }
            }
            catch(Exception e)
            {
                message = e.ToString();
            }

            return Ok(message);
        }

        [HttpPost]
        [Route("CloseSoNote")]
        public async Task<IActionResult> CloseSoNote(MfgIssueNote issueNote)
        {

            CommonResponse<int> common = new();
            try
            {

                var note = await _context.Shippers.FindAsync(issueNote.IsNId);
                if (note != null)
                {
                    int orderCount = _context.IssueOrders.Where(x => x.ToVehicle == note.ShpId && x.Confirm != true).Count();
                    if (orderCount > 0)
                    {
                        var many = orderCount > 1 ? "Unable to finish. There are " + orderCount + " unfinished movements to this vehicle"
                            : "Unable to finish. There is an unfinished movement to this vehicle";
                        common.message = many;
                        common.status = -1;
                        return Ok(common);
                    }

                    note.IssueConfirmed = true;
                    _context.Update(note);
                    _context.SaveChanges();
                    common.message = "Success!";
                    common.status = 1;
                }
                else
                {
                    common.status = 0;
                    common.message = "Not found!";
                }
            }
            catch (Exception e)
            {
                common.status = -1;
                common.message = e.ToString();
            }

            return Ok(common);
        }

    }
}
