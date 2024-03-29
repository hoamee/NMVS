﻿using Microsoft.AspNetCore.Http;
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
        IRequestService _service;

        public InvRequestApiController(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, IRequestService service)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _service = service;
        }

        [HttpPost]
        [Route("PushPickListSo")]
        public async Task<IActionResult> PushPickListSo(List<JsPickingData> js)
        {
            CommonResponse<string> response = new();
            response.message = "Success";
            response.status = 1;

            var workSpace = HttpContext.Session.GetString("susersite");
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

                    if (shp == null)
                    {
                        var toLoc = _context.Locs.Find(jsArr.whcd);
                        if (toLoc == null)
                        {
                            response.message = "No to location found. Please create or select a MFG location before continue";
                            response.status = -1;

                            return Ok(response);
                        }
                    }

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
                        MovementTime = jsArr.reqTime,
                        Site = workSpace
                    });

                    _context.Update(request);
                    _context.Update(pt);
                    _context.Update(fromLoc);
                    _context.SaveChanges();
                    response.message = "Success!";
                }
                catch (Exception e)
                {
                    await Common.MonitoringService.SendErrorMessage(e.ToString());
                    response.message = "An error occurred...";
                    response.status = -1;
                }
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("FinishIssueOrder")]
        public async Task<IActionResult> FinishIssueOrder(AllocateOrder alo)
        {
            CommonResponse<int> commonResponse = new();

            commonResponse.message = "Order not found!";
            commonResponse.status = -1;

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
                        if (string.IsNullOrEmpty(shp.CheckInBy))
                        {

                            commonResponse.message = "Shipper has not checked in yet";
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


                        //check FROM-location
                        commonResponse.message = "From loc not found!";
                        var fromLoc = await _context.Locs.FindAsync(pt.LocCode);
                        if (fromLoc != null)
                        {
                            fromLoc.LocRemain += issueQty;

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
                                        To = "Shipper Id: " + shp.ShpId,
                                        LastId = pt.PtId,
                                        NewId = null,
                                        OrderNo = order.ExpOrdId,
                                        IsAllocate = false,
                                        IsDisposed = false,
                                        MovementTime = DateTime.Now
                                    });
                                    var soDet = _context.SoDetails.Find(reDet.SodId);

                                    soDet.Shipped += issueQty;
                                    _context.Update(soDet);
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

                                order.ConfirmedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
                                order.MovedQty += issueQty;
                                if (order.ExpOrdQty <= (order.MovedQty + order.Reported))
                                {
                                    order.Confirm = true;
                                    order.CompletedTime = DateTime.Now;
                                }
                                pt.MovementNote += itemNote;

                                _context.Update(pt);
                                _context.Update(order);
                                _context.SaveChanges();
                                commonResponse.message = "Success"!;
                                commonResponse.status = 1;
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
                    var requestDets = _context.RequestDets.Where(x => x.RqID == note.RqId).Select(x => x.DetId).ToList();
                    var remainOrder = 0;
                    foreach (var order in _context.IssueOrders.Where(x => requestDets.Contains(x.DetId) && x.Confirm == null))
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
            CommonResponse<int> common =
                await _service.CloseShipperNote(issueNote, _httpContextAccessor.HttpContext.User.Identity.Name);

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

                        // 1. increase report qty
                        order.Reported += report.Qty;
                        order.Note += report.Note;
                        // 2. add note
                        rqDet.MovementNote += " **" + report.Note;
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



                        if (order.ExpOrdQty <= (order.MovedQty + order.Reported))
                        {
                            order.Confirm = true;
                            order.ConfirmedBy = User.Identity.Name;
                            order.CompletedTime = DateTime.Now;
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
            var workSpace = HttpContext.Session.GetString("susersite");
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
                if (shipper.RememberMe)
                {
                    _context.Add(new Shipper
                    {
                        RememberMe = shipper.RememberMe,
                        DateIn = DateTime.Now.AddHours(1),
                        DrContact = shipper.DrContact,
                        Driver = shipper.Driver,
                        IssueConfirmed = false,
                        ShpDesc = shipper.ShpDesc,
                        Loc = shipper.Loc,
                        ShpVia = shipper.ShpVia,
                        ShpTo = shipper.ShpTo,
                        RegisteredBy = User.Identity.Name,
                        Site = workSpace

                    });
                }

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
                var requestDet = _context.RequestDets.Where(x => x.RqID == request.RqID);
                if (!requestDet.Any())
                {

                    _context.Remove(request);
                }
                else
                {
                    request.SoConfirm = true;
                    _context.Update(request);
                }

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


        [Route("RemoveRq")]
        [HttpPost]
        public async Task<IActionResult> RemoveRq(JsPickingData rqd)
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
                _context.Remove(request);
                _context.RemoveRange(_context.RequestDets.Where(x => x.RqID == rqd.whcd));
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
                if (rqd.accepted == false)
                {
                    request.SoConfirm = false;
                }
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
                        if (rqd.accepted == false)
                        {
                            so.Confirm = null;
                            so.ConfirmBy = "Un-confirmed by WH reject";
                            so.Closed = false;
                        }

                        so.ConfirmationNote = rqd.loc;
                        so.RequestConfirmedBy = request.ConfirmedBy;
                        so.RequestConfirmed = rqd.accepted;
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

        [Route("ReportRequest")]
        [HttpPost]
        public async Task<IActionResult> ReportRequest(JsPickingData rqd)
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
                request.ReportedNote = "By " + _httpContextAccessor.HttpContext.User.Identity.Name + ": " + rqd.loc;
                request.Reported = true;
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
                        so.ReqReported = true;
                        so.ReqReportedNote = request.ReportedNote;
                        so.Confirm = null;
                        so.Closed = false;
                        request.SoConfirm = false;
                        request.Confirmed = null;
                        so.ConfirmBy = "Un-confirmed by WH report";
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

        [Route("FinhishedRequest")]
        [HttpPost]
        public async Task<IActionResult> FinhishedRequest(JsPickingData rqd)
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
                request.Closed = true;
                _context.Update(request);
                await _context.SaveChangesAsync();
                common.status = 1;
                common.message = "success!";
                return Ok(common);
            }
        }


    }
}
