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
        private readonly IHttpContextAccessor _httpContextAccessor;
        ApplicationDbContext _context;

        public InvRequestApiController(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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
                    //fromLoc.LocOutgo += jsArr.qty;


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
            CommonResponse<int> commonResponse = new();

            commonResponse.message = "Order not found!";

            var issueQty = alo.AlcOrdQty;

            try
            {
                var order = await _context.IssueOrders.FindAsync(alo.AlcOrdId);

                if (order != null)
                {

                    //Check if shipper is checked out
                    commonResponse.message = "Shipper is already checked out!";
                    var shp = await _context.Shippers.FindAsync(order.ToVehicle);
                    if (shp != null && order.IssueType == "Issue")
                    {
                        if (!string.IsNullOrEmpty(shp.CheckOutBy))
                        {
                            return Ok(commonResponse);
                        }

                    }
                    //Check item exist
                    commonResponse.message = "Item not found";
                    var pt = await _context.ItemMasters.FindAsync(order.PtId);
                    if (pt != null)
                    {
                        pt.PtHold -= issueQty;
                        pt.PtQty -= issueQty;

                        if (pt.PtHold < 0 || pt.PtQty < 0)
                        {
                            commonResponse.message = "Item quantity error";
                            return Ok(commonResponse);
                        }
                        _context.Update(pt);


                        //check location
                        commonResponse.message = "From loc not found!";
                        var fromLoc = await _context.Locs.FindAsync(pt.LocCode);
                        if (fromLoc != null)
                        {
                            //fromLoc.LocOutgo -= issueQty;
                            fromLoc.LocRemain += issueQty;

                            //if (fromLoc.LocOutgo < 0 || fromLoc.LocRemain < 0)
                            //{
                            //    commonResponse.message = "Location capacity error";
                            //    return Ok(commonResponse);
                            //}
                            //_context.Update(fromLoc);


                            //Check request det
                            var reDet = _context.RequestDets.Find(order.DetId);
                            commonResponse.message = "Request not found!";
                            var itemNote = "";
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
                                    _context.Add(new InventoryTransac
                                    {
                                        From = pt.LocCode,
                                        To = "Shipper Id: "+shp.ShpId,
                                        LastId = pt.PtId,
                                        NewId = null,
                                        OrderNo = order.ExpOrdId,
                                        IsAllocate = false,
                                        IsDisposed = false,
                                        MovementTime = DateTime.Now
                                    });
                                }
                                else
                                {
                                    //Create new issue note only
                                    //To loc is not considerred
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
                                    _context.Add(new InventoryTransac
                                    {
                                        From = fromLoc.LocCode,
                                        To = pt.LocCode,
                                        LastId = pt.PtId,
                                        NewId = null,
                                        OrderNo = order.ExpOrdId,
                                        IsAllocate = false,
                                        IsDisposed = false,
                                        MovementTime = DateTime.Now
                                    });

                                }
                                var soDet = _context.SoDetails.Find(reDet.SodId);

                                soDet.Shipped += issueQty;
                                order.ConfirmedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
                                order.MovedQty += issueQty;
                                if (order.MovedQty >= order.ExpOrdQty)
                                {
                                    order.Confirm = true;
                                }
                                pt.MovementNote += itemNote;
                                _context.Update(soDet);
                                _context.Update(pt);
                                _context.Update(order);
                                _context.SaveChanges();
                                commonResponse.message = "Success"!;
                            }


                        }

                    }


                }
            }
            catch (Exception e)
            {
                commonResponse.message = e.ToString();
            }

            return Ok(commonResponse);
        }

        [HttpPost]
        [Route("CloseNote")]
        public IActionResult CloseNote(MfgIssueNote issueNote)
        {
            CommonResponse<int> common = new();
            try
            {
                var note = _context.MfgIssueNotes.Find(issueNote.IsNId);
                if (note != null)
                {
                    var requestDets = _context.RequestDets.Where(x => x.RqID == note.RqId).Select(x=>x.DetId).ToList();
                    var remainOrder = 0;
                    foreach (var order in _context.IssueOrders.Where(x=> requestDets.Contains(x.DetId) && x.Confirm == null))
                    {
                        remainOrder++;
                    }
                    if (remainOrder > 0)
                    {
                        common.status = 0;
                        common.message = remainOrder > 1 ? "Unable to finish. There are " + remainOrder + " unfinished movements for this request"
                            : "Unable to finish. There is an unfinished movement for this request";
                        return Ok(common);
                    }
                    note.IssuedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
                    note.IssuedOn = DateTime.Now;
                    _context.Update(note);
                    _context.SaveChanges();
                    common.status = 1;
                    common.message = "Success";
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
                    note.IssueConfirmedTime = DateTime.Now;
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


        [HttpPost]
        [Route("ReportOrder")]
        public async Task<ActionResult> ReportOrder(BrokenVm report)
        {
            var common = new CommonResponse<int>();
            try
            {
                var order = await _context.IssueOrders.FindAsync(report.OrId);
                if (order != null)
                {
                    var rqDet = await _context.RequestDets.FindAsync(order.DetId);
                    if (rqDet != null)
                    {
                        // decrease order qty
                        order.ExpOrdQty -= report.Qty;

                        // 1. decrease request qty
                        // 2. add note
                        rqDet.MovementNote += "**Issue order " + report.OrId + ": report quantity of " + report.Qty + ". Message:" + report.Note + ", By " + _httpContextAccessor.HttpContext.User.Identity.Name + "; ";
                        rqDet.Quantity -= report.Qty;
                        rqDet.Picked -= report.Qty;

                        // decrease item master hold qty
                        var pt = await _context.ItemMasters.FindAsync(order.PtId);
                        pt.PtHold -= report.Qty;

                        var request = await _context.InvRequests.FindAsync(rqDet.RqID);
                        // decrease loc hold
                        var fromLoc = await _context.Locs.FindAsync(pt.LocCode);
                        //fromLoc.LocOutgo -= report.Qty;


                        if (request.RqType == "MFG")
                        {

                            var toLoc = await _context.Locs.FindAsync(order.ToLoc);
                            toLoc.LocHolding -= report.Qty;

                            _context.Update(toLoc);

                        }


                        //case unqualified: moving item to unqualified location
                        if (report.Retrn)
                        {
                            //increase remain of loc
                            fromLoc.LocRemain += report.Qty;

                            // decrease qty
                            pt.PtQty -= report.Qty;

                            var uq = new Unqualified
                            {
                                Description = report.Note,
                                ItemNo = pt.ItemNo,
                                Note = "",
                                PtId = pt.PtId,
                                Quantity = report.Qty
                            };
                            // add new broken
                            _context.Unqualifieds.Add(uq);
                            _context.SaveChanges();

                            _context.Add(new InventoryTransac
                            {
                                From = fromLoc.LocCode,
                                To = "Unqualified",
                                LastId = pt.PtId,
                                NewId = uq.UqId,
                                OrderNo = order.ExpOrdId,
                                MovementTime = DateTime.Now,
                                IsDisposed = false,
                                IsAllocate = false
                            });
                        }



                        if (order.ExpOrdQty <= order.MovedQty)
                        {
                            order.Confirm = true;
                            order.ConfirmedBy = User.Identity.Name;
                        }
                        _context.Update(order);
                        _context.Update(pt);
                        _context.Update(rqDet);

                        await _context.SaveChangesAsync();
                        common.status = 1;
                        common.message = "Report sent!";
                    }
                    else
                    {
                        common.status = 0;
                        common.message = "Request not found.";
                    }
                }
                else
                {
                    common.status = 0;
                    common.message = "Order not found.";
                }
            }
            catch (Exception e)
            {
                common.status = -1;
                common.message = "Error: " + e.ToString();
            }

            return Ok(common);
        }

        [Route("ConfirmCheckOut")]
        [HttpPost]
        public async Task<IActionResult> ConfirmCheckOut(JsPickingData shpr)
        {
            CommonResponse<int> common = new();
            try
            {
                var shipper = await _context.Shippers.FindAsync(shpr.id);
                if (shipper == null)
                {
                    common.message = "System coundn't find this shipper";
                    common.status = 0;
                    return Ok(common);
                }
                shipper.CheckOutBy = User.Identity.Name;
                shipper.ActualOut = DateTime.Now;


                _context.Update(shipper);
                _context.SaveChanges();
                common.message = "Success";
                common.status = 1;
            }
            catch (Exception e)
            {
                common.message = e.ToString();
                common.status = -1;
            }
            return Ok(common);
        }

        [Route("CloseRequest")]
        [HttpPost]
        public async Task<IActionResult> CloseRequest(JsPickingData shpr)
        {
            var common = new CommonResponse<int>();
            var request = await _context.InvRequests.FindAsync(shpr.whcd);
            if (request == null)
            {
                common.status = 0;
                common.message = "not found!";
                return Ok(common);
            }
            else
            {
                request.SoConfirm = true;
                _context.Update(request);
                await _context.SaveChangesAsync();
                common.status = 1;
                common.message = "success!";
                return Ok(common);
            }
        }

        [Route("RemoveRqLine")]
        [HttpPost]
        public async Task<IActionResult> RemoveRqLine(JsPickingData rqd)
        {
            var common = new CommonResponse<int>();
            var request = await _context.RequestDets.FindAsync(rqd.id);
            if (request == null)
            {
                common.status = 0;
                common.message = "not found!";
                return Ok(common);
            }
            else
            {
                _context.Remove(request);
                await _context.SaveChangesAsync();
                common.status = 1;
                common.message = "success!";
                return Ok(common);
            }
        }

        [Route("ConfirmRequest")]
        [HttpPost]
        public async Task<IActionResult> ConfirmRequest(JsPickingData rqd)
        {
            var common = new CommonResponse<int>();
            var request = await _context.InvRequests.FindAsync(rqd.whcd);
            if (request == null)
            {
                common.status = 0;
                common.message = "not found!";
                return Ok(common);
            }
            else
            {
                request.ConfirmationNote = rqd.loc;
                request.Confirmed = rqd.accepted;
                request.ConfirmedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (request.RqType == "Issue")
                {
                    var so = await _context.SalesOrders.FindAsync(request.RqID);
                    if (so == null)
                    {
                        common.status = -1;
                        common.message = "Unable to find SO";
                        return Ok(common);
                    }
                    else
                    {
                        so.RequestConfirmed = rqd.accepted;
                        so.RequestConfirmedBy = request.ConfirmedBy;
                        so.ConfirmationNote = rqd.loc;
                        so.Confirm = null;
                        so.ConfirmBy = "Un-confirmed by WH reject";
                        so.Closed = false;
                        _context.Update(so);
                    }
                }
                _context.Update(request);
                await _context.SaveChangesAsync();
                common.status = 1;
                common.message = "success!";
                return Ok(common);
            }
        }
    }
}
