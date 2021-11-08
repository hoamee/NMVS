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
        [Route("SoConfirm/{id}")]
        public async Task<ActionResult> SoConfirm(string id)
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
            saleOrder.Confirm = true;
            saleOrder.ConfirmBy = _httpContextAccessor.HttpContext.User.Identity.Name;

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
                await _context.SaveChangesAsync();
            }


            return Ok();
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
            saleOrder.UpdatedBy = _httpContextAccessor.HttpContext.User.Identity.Name;
            saleOrder.UpdatedOn = DateTime.Now;
            _context.Update(saleOrder);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }

    
}
