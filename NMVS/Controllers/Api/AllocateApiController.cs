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
        private readonly IHttpContextAccessor _httpContextAccessor;
        ApplicationDbContext _context;

        public AllocateApiController(IIncomingService service, IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _context = context;
            _service = service;
            _httpContextAccessor = httpContextAccessor;

        }


        [HttpPost]
        [Route("ConfirmSelectLoc")]
        public IActionResult ConfirmSelectLoc(JsPickingData jsArr)
        {
            CommonResponse<int> common = new();

            try
            {
                //Test response
                //string s = jsArr[0].id + ", " + jsArr[0].whcd + ", " + jsArr[0].qty;
                //return Json(s);
                var arr = jsArr;
                //   3. Add holding to To-Loc
                var toLoc = _context.Locs.Find(arr.loc);

               
                    //Get Item master
                    var pt = _context.ItemMasters.Find(arr.id);
                    var fromLoc = _context.Locs.Find(pt.LocCode);

                    //   2.Add holding to From-item
                    pt.PtHold += arr.qty;


                    toLoc.LocHolding += arr.qty;

                    //   4. Add Outgo to From-Loc
                    fromLoc.LocOutgo += arr.qty;


                    _context.AllocateRequests.Add(new AllocateRequest()
                    {
                        PtId = pt.PtId,
                        AlcFrom = pt.LocCode,
                        LocCode = arr.loc,
                        AlcQty = arr.qty,
                        AlcFromDesc = fromLoc.LocDesc,
                        MovementTime = arr.reqTime
                    });

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

        public async Task<IActionResult> PostOrder(int id)
        {
            var request = await _context.AllocateRequests.FindAsync(id);
            CommonResponse<int> commonResponse = new();
            try
            {
                if(request != null)
                {
                    var AllocateOrder = new AllocateOrder
                    {
                        PtId = request.PtId,
                        AlcOrdFrom = request.AlcFrom,
                        AlcOrdFDesc = request.AlcFromDesc,
                        LocCode = request.LocCode,
                        AlcOrdQty = request.AlcQty,
                        OrderBy = User.Identity.Name,
                        RequestID = id,
                        MovementTime = request.MovementTime
                    };
                    //clear holding item
                    var pt = await _context.ItemMasters.FindAsync(request.PtId);

                    _context.AllocateOrders.Add(AllocateOrder);
                    request.IsClosed = true;
                    await _context.SaveChangesAsync();
                    commonResponse.status = 1;
                }

                
            }
            catch (Exception)
            {
                commonResponse.status = -1;
            }
            

            return Ok(commonResponse);
        }
    }
}
