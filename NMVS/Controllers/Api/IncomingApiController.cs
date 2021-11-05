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
    [Route("api/IncomingList")]
    [ApiController]
    public class IncomingApiController : Controller
    {
        private readonly IIncomingService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;
        ApplicationDbContext _context;

        public IncomingApiController(IIncomingService service, IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _context = context;
            _service = service;
            _httpContextAccessor = httpContextAccessor;

        }

        [HttpPost]
        [Route("AddItem")]
        public IActionResult AddItem(ItemMaster item)
        {
            var role = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
            ItemMaster iItem = item;
            CommonResponse<int> common = new();
            if (role)
            {
                try
                {
                    var ic = _context.IncomingLists.Find(item.IcId);
                    ic.ItemCount++;
                    _context.Update(ic);
                    _context.Add(new ItemMaster
                    {
                        ItemNo = iItem.ItemNo,
                        IcId = iItem.IcId,
                        RecQty = iItem.RecQty,
                        RefNo = iItem.RefNo,
                        RefDate = iItem.RefDate,
                        PtCmt = iItem.PtCmt
                    });
                    _context.SaveChanges();
                    common.status = 1;
                }
                catch (Exception e)
                {
                    common.status = -1;
                    common.message = e.ToString();
                }
            }
            else
            {
                common.status = 0;
            }

            return Ok(common);

        }

        [HttpPost]
        [Route("CloseList")]
        public IActionResult CloseList(ItemMaster item)
        {
            var role = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
            
            CommonResponse<int> common = new();
            if (role)
            {
                try
                {
                    var ic = _context.IncomingLists.Find(item.IcId);
                    ic.Closed = true;
                    _context.Update(ic);
                    _context.SaveChanges();
                    common.status = 1;
                }
                catch (Exception e)
                {
                    common.status = -1;
                    common.message = e.ToString();
                }
            }
            else
            {
                common.status = 0;
            }

            return Ok(common);

        }

        [HttpPost]
        [Route("QcItem")]
        public IActionResult QcItem(ItemMaster item)
        {
            var role = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;

            var receiveLoc = _context.Locs.FirstOrDefault(l => l.LocType == "receive");
            CommonResponse<string> common = new();
            if (role)
            {
                if (receiveLoc != null)
                {
                    try
                    {

                        var pt = _context.ItemMasters.Find(item.PtId);
                        pt.Accepted = pt.RecQty - item.PtQty;
                        pt.PtQty = pt.RecQty - item.PtQty;
                        pt.Qc = _httpContextAccessor.HttpContext.User.Identity.Name;
                        pt.LocCode = receiveLoc.LocCode;
                        if (item.PtQty > 0)
                        {
                            pt.PtCmt += string.IsNullOrEmpty(pt.PtCmt) ? item.PtCmt : " | " + item.PtCmt;
                        }
                        var ic = _context.IncomingLists.Find(pt.IcId);
                        ic.Checked++;
                        receiveLoc.LocRemain -= pt.PtQty;

                        _context.Update(pt);
                        _context.Update(ic);
                        _context.Update(receiveLoc);
                        _context.SaveChanges();
                        common.status = 1;
                        common.dataenum = _httpContextAccessor.HttpContext.User.Identity.Name;
                    }
                    catch (Exception e)
                    {
                        common.status = -1;
                        common.message = e.ToString();
                    }
                }
                else
                {
                    common.status = -1;
                    common.message = "No receving location found, please create one first";
                }
            }
            else
            {
                common.status = 0;
            }

            return Ok(common);

        }



    }
}
