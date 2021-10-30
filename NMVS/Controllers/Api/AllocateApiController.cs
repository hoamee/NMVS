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
    [Route("api/Allocate")]
    [ApiController]
    public class AllocateApiController : Controller
    {
        private readonly IIncomingService _service;
        private readonly IAllocateService _alcService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        ApplicationDbContext _context;

        public AllocateApiController(IIncomingService service, IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, IAllocateService allocateService)
        {
            _context = context;
            _service = service;
            _httpContextAccessor = httpContextAccessor;
            _alcService = allocateService;
        }


        [HttpPost]
        [Route("ConfirmJsAllocate")]
        public ActionResult ConfirmJsAllocate(JsPickingData jsArr)
        {
            CommonResponse<int> common = new();
            try
            {
                var t = jsArr;
                //Test response
                //string s = jsArr[1].id + ", " + jsArr[1].whcd + ", " + jsArr[1].qty;
                //return Json(s);

                //Get Item master
                var pt = _context.ItemMasters.Find(t.id);
                var fromLoc = _context.Locs.Find(pt.LocCode);


                //   2.Add holding to From-item
                pt.PtHold += t.qty;

                //   3. Add holding to To-Loc
                var toLoc = _context.Locs.Find(t.whcd);
                toLoc.LocHolding += t.qty;

                //   4. Add Outgo to From-Loc
                fromLoc.LocOutgo += t.qty;


                _context.AllocateRequests.Add(new AllocateRequest()
                {
                    PtId = pt.PtId,
                    AlcFrom = pt.LocCode,
                    LocCode = t.whcd,
                    AlcQty = t.qty,
                    AlcFromDesc = fromLoc.LocDesc,
                    MovementTime = t.reqTime
                });

                //Meeting 3 remove. No no
                //pt.PtHold = t.qty;
                //pt.PtQty = pt.PtQty - t.qty;
                _context.SaveChanges();
                common.status = 1;
                common.message += "Success";
            }
            catch (Exception e)
            {
                common.status = -1;
                common.message = e.ToString();
            }
            return Json(common);
        }

        [HttpPost]
        [Route("ConfirmSelectLoc")]
        public IActionResult ConfirmSelectLoc(JsPickingData jsArr)
        {
            return Ok(_alcService.ConfirmSelectLoc(jsArr));
        }

        [HttpPost]
        [Route("PostOrder")]
        public async Task<IActionResult> PostOrder(AllocateRequest alc)
        {
            var request = await _context.AllocateRequests.FindAsync(alc.AlcId);
            CommonResponse<int> commonResponse = new();
            try
            {
                if (request != null)
                {
                    var AllocateOrder = new AllocateOrder
                    {
                        PtId = request.PtId,
                        AlcOrdFrom = request.AlcFrom,
                        AlcOrdFDesc = request.AlcFromDesc,
                        LocCode = request.LocCode,
                        AlcOrdQty = request.AlcQty,
                        OrderBy = User.Identity.Name,
                        RequestID = alc.AlcId,
                        MovementTime = request.MovementTime
                    };

                    _context.AllocateOrders.Add(AllocateOrder);
                    request.IsClosed = true;
                    _context.Update(request);

                    await _context.SaveChangesAsync();
                    commonResponse.status = 1;
                }
                else
                {
                    commonResponse.status = -1;
                    commonResponse.message = "Request not found";
                }

            }
            catch (Exception e)
            {
                commonResponse.status = -1;
                commonResponse.message = e.Message;
            }


            return Ok(commonResponse);
        }

        [HttpPost]
        [Route("DeclineRequest")]
        public async Task<IActionResult> DeclineRequest(AllocateRequest alc)
        {
            var request = await _context.AllocateRequests.FindAsync(alc.AlcId);
            CommonResponse<int> commonResponse = new();
            try
            {
                if (request != null)
                {

                    var pt = await _context.ItemMasters.FindAsync(request.PtId);
                    var fromLoc = await _context.Locs.FindAsync(pt.LocCode);

                    //   2.decrease holding of From-item
                    pt.PtHold -= request.AlcQty;

                    //   3. decrease holding to To-Loc
                    var toLoc = await _context.Locs.FindAsync(request.LocCode);
                    toLoc.LocHolding -= request.AlcQty;

                    //   4. decrease Outgo to From-Loc
                    fromLoc.LocOutgo -= request.AlcQty;

                    request.IsClosed = false;
                    _context.Update(request);
                    _context.Update(fromLoc);
                    _context.Update(toLoc);
                    _context.Update(pt);
                    await _context.SaveChangesAsync();
                    commonResponse.status = 1;
                }
                else
                {
                    commonResponse.status = -1;
                    commonResponse.message = "Request not found";
                }


            }
            catch (Exception e)
            {
                commonResponse.status = -1;
                commonResponse.message = e.Message;
            }


            return Ok(commonResponse);
        }

        [HttpPost]
        [Route("FinishOrder")]
        public async Task<ActionResult> FinishOrder(AllocateOrder alo)
        {
            CommonResponse<int> response = new();
            try
            {
                //1. find Order by request parameter: id.
                //   Set confirm = true
                var order = await _context.AllocateOrders.FindAsync(alo.AlcOrdId);
                var pt = await _context.ItemMasters.FindAsync(order.PtId);
                order.MovedQty += alo.MovedQty;
                if (order.MovedQty >= order.AlcOrdQty)
                {
                    order.Confirm = true;
                }
                //   Set confirmedBy = logged user
                var loggedUser = User.Identity.Name;
                order.ConfirmedBy = loggedUser;

                //2. update toLoc:
                //   holding - OrderQuantity
                //   remain capacity - OrderQuantity
                var toLoc = await _context.Locs.FindAsync(order.LocCode);
                toLoc.LocHolding -= order.AlcOrdQty;
                toLoc.LocRemain -= order.AlcOrdQty;

                //3. update fromLoc:
                //   outgo           -= OrderQty
                //   remain capacity += OrderQty
                var fromLoc = await _context.Locs.FindAsync(order.AlcOrdFrom);
                fromLoc.LocOutgo -= order.AlcOrdQty;
                fromLoc.LocRemain += order.AlcOrdQty;

                //4. add new item master
                _context.ItemMasters.Add(new ItemMaster()
                {
                    ItemNo = pt.ItemNo,
                    PtHold = 0,
                    PtCmt = pt.PtCmt,
                    PtDateIn = pt.PtDateIn,
                    PtLotNo = pt.PtLotNo,
                    PtQty = order.AlcOrdQty,
                    SupCode = pt.SupCode,
                    RecBy = loggedUser,
                    LocCode = order.LocCode,
                    Accepted = pt.Accepted,
                    IcId = pt.IcId,
                    Qc = pt.Qc,
                    RecQty = pt.RecQty,
                    RefDate = pt.RefDate,
                    RefNo = pt.RefNo
                });

                //5. update from-item master
                pt.PtHold -= order.AlcOrdQty;
                pt.PtQty -= order.AlcOrdQty;
                response.status = 1;
                _context.Update(pt);
                await _context.SaveChangesAsync();
                return Ok(response);
            }
            catch (Exception e)
            {
                response.status = -1;
                response.message = e.ToString();
                return Ok(response);
            }
        }


        [HttpPost]
        [Route("ReportOrder")]
        public async Task<ActionResult> ReportOrder(BrokenVm report)
        {
            var common = new CommonResponse<int>();
            try
            {
                var order = await _context.AllocateOrders.FindAsync(report.OrId);
                if (order != null)
                {
                    var request = await _context.AllocateRequests.FindAsync(order.RequestID);
                    if (request != null)
                    {
                        // decrease order qty
                        order.AlcOrdQty -= report.Qty;

                        // 1. decrease request qty
                        // 2. add note
                        request.AlcCmmt += "**Order " + report.OrId + ": report quantity of " + report.Qty + ". Message:" + report.Note + ", By " + _httpContextAccessor.HttpContext.User.Identity.Name + "; ";
                        request.AlcQty -= report.Qty;

                        // decrease item master hold qty
                        var pt = await _context.ItemMasters.FindAsync(order.PtId);
                        pt.PtHold -= report.Qty;

                        // decrease loc hold
                        var loc = await _context.Locs.FindAsync(pt.LocCode);
                        loc.LocOutgo -= report.Qty;

                        var toLoc = await _context.Locs.FindAsync(order.LocCode);
                        loc.LocHolding -= report.Qty;

                        //case unqualified: moving item to unqualified location
                        if (report.Retrn)
                        {
                            //increase remain of loc
                            loc.LocRemain += report.Qty;

                            // decrease qty
                            pt.PtQty -= report.Qty;

                            // add new broken
                            _context.Unqualifieds.Add(new Unqualified
                            {
                                Description = report.Note,
                                ItemNo = pt.ItemNo,
                                Note = "",
                                PtId = pt.PtId,
                                Quantity = report.Qty
                            });
                        }
                        else
                        {

                        }

                        _context.AllocateOrders.Update(order);
                        _context.Update(pt);
                        _context.Update(request);

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

        [HttpPost]
        [Route("ProcessUnqualified")]
        public async Task<ActionResult> ProcessUnqualified(BrokenVm report)
        {
            var common = new CommonResponse<int>();
            try
            {
                var uq = await _context.Unqualifieds.FindAsync(report.OrId);
                if (uq != null)
                {
                    var transac = new UnqualifiedTransac
                    {
                        ByUser = _httpContextAccessor.HttpContext.User.Identity.Name,
                        IsDisposed = report.Retrn,
                        Note = report.Note,
                        Qty = report.Qty,
                        TransantionTime = DateTime.Now,
                        UnqId = report.OrId
                    };

                    string dispose = "";

                    if (report.Retrn)
                    {
                        uq.DisposedQty += report.Qty;
                        dispose = "disposed ";
                    }
                    else
                    {
                        uq.RecycleQty += report.Qty;
                        dispose = "recycled ";
                    }
                    uq.Note += "**" + transac.TransantionTime.ToString() + ": " + dispose + report.Qty + " item. By " + transac.ByUser + ". Note: " + report.Note;

                    _context.Add(transac);
                    _context.Update(uq);

                    _context.SaveChanges();

                    common.status = 1;
                    common.message = "Success!";
                }
                else
                {
                    common.status = 0;
                    common.message = "Request not found.";
                }
            }
            catch (Exception e)
            {
                common.status = -1;
                common.message = "Error: " + e.ToString();
            }

            return Ok(common);
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
                    IssueType = "Issue",
                    ItemNo = pt.ItemNo,
                    LocCode = pt.LocCode,
                    OrderBy = User.Identity.Name,
                    ToVehicle = shp.ShpId,
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
                if (string.IsNullOrEmpty(shp.CheckOutBy))
                {
                    //Check item exist
                    message = "Item not found";
                    var pt = await _context.ItemMasters.FindAsync(order.PtId);
                    if (pt != null)
                    {
                        pt.PtHold -= issueQty;
                        pt.PtQty -= issueQty;

                        if (pt.PtHold < 0 || pt.PtQty <0)
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

                
            }

            return Ok(message);
        }
    }
}
