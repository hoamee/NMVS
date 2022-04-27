using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NMVS.Models;
using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Controllers.Api
{
    [Route("api/So")]
    [ApiController]
    public class SoApiController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        ApplicationDbContext _context;

        public SoApiController(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _context = context;

            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Route("RemoveSoLine")]
        public async Task<IActionResult> RemoveSoLine(ItemMaster sod)
        {
            CommonResponse<int> response = new();
            var soDet = await _context.SoDetails.FindAsync(sod.PtId);
            if (soDet == null)
            {
                response.status = 0;
                response.message = "An error occurred!";
            }
            else
            {
                var rqDet = await _context.RequestDets.FindAsync(soDet.RqDetId);
                _context.Remove(soDet);
                _context.Remove(rqDet);
                await _context.SaveChangesAsync();
                response.status = 1;
                response.message = "Removed!";
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("SoConfirm/{id}/{ap}")]
        public async Task<ActionResult> SoConfirm(string id, int ap)
        {
            if (id == null)
            {
                return NotFound();
            }
            SalesOrder saleOrder = await _context.SalesOrders.FindAsync(id);

            if (saleOrder == null)
            {
                return NotFound();
            }
            if (ap == 1)
            {
                saleOrder.Confirm = true;
                _context.Update(saleOrder);
                var invRequest = await _context.InvRequests.FindAsync(id);
                if (invRequest != null)
                {
                    invRequest.SoConfirm = true;
                    _context.Update(invRequest);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    invRequest = new InvRequest()
                    {
                        RqType = "Issue",
                        Ref = id,
                        RqCmt = saleOrder.Comment,
                        RqBy = saleOrder.UpdatedBy,
                        SoConfirm = true,
                        RqDate = DateTime.Now,
                        RqID = id
                    };
                    _context.InvRequests.Add(invRequest);
                }
            }
            saleOrder.ConfirmBy = _httpContextAccessor.HttpContext.User.Identity.Name;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [Route("SoReject")]
        public async Task<ActionResult> SoReject(JsPickingData data)
        {
            SalesOrder saleOrder = await _context.SalesOrders.FindAsync(data.whcd);
            CommonResponse<int> common = new();
            if (saleOrder == null)
            {
                return NotFound();
            }

            saleOrder.Confirm = false;
            _context.Update(saleOrder);
            var invRequest = await _context.InvRequests.FindAsync(data.whcd);
            if (invRequest != null)
            {
                _context.Remove(invRequest);
            }

            saleOrder.ApprovalNote = data.loc;
            saleOrder.ConfirmBy = _httpContextAccessor.HttpContext.User.Identity.Name;
            saleOrder.Closed = false;
            await _context.SaveChangesAsync();
            common.status = 1;
            return Ok(common);
        }

        [HttpPost]
        [Route("ForceComplete")]
        public async Task<ActionResult> ForceComplete(JsPickingData data)
        {
            SalesOrder saleOrder = await _context.SalesOrders.FindAsync(data.whcd);
            CommonResponse<int> common = new();
            if (saleOrder == null)
            {
                return NotFound();
            }

            var rq = _context.InvRequests.Find(data.whcd);
            rq.Closed = true;
            saleOrder.ApprovalNote = data.loc;
            saleOrder.ConfirmBy = _httpContextAccessor.HttpContext.User.Identity.Name;
            saleOrder.Completed = true;
            //Merge with Issue order
            var soDets = _context.SoDetails.Where(x => x.SoNbr == rq.RqID);
            foreach (var det in soDets)
            {
                var rqDet = await _context.RequestDets.FindAsync(det.RqDetId);
                if (det.Shipped == 0 && rqDet.Arranged == 0)
                {
                    _context.Remove(det);
                    _context.Remove(rqDet);
                }
                else if (det.Shipped != rqDet.Arranged)
                {
                    det.Shipped = rqDet.Arranged;
                }

                if (det.Quantity != det.Shipped)
                    det.Quantity = det.Shipped;


                _context.Update(det);
            }


            _context.Update(saleOrder);
            _context.Update(rq);
            await _context.SaveChangesAsync();
            common.status = 1;
            return Ok(common);
        }

        [HttpGet]
        [Route("SoSubmit/{id}")]
        public async Task<ActionResult> SoSubmit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SalesOrder saleOrder = await _context.SalesOrders.FindAsync(id);

            if (saleOrder == null)
            {
                return NotFound();
            }
            saleOrder.Closed = true;
            saleOrder.Confirm = null;
            saleOrder.UpdatedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
            saleOrder.UpdatedOn = DateTime.Now;
            _context.Update(saleOrder);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }


}
