using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NMVS.Common;
using NMVS.Models;
using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using NMVS.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
        public IActionResult AddItem(ItemMaster iItem)
        {
            var role = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
            CommonResponse<int> common = new();
            if (role)
            {
                try
                {
                    var itemData = _context.ItemDatas.Where(x => x.ItemNo == iItem.ItemNo).FirstOrDefault();
                    if (itemData == null)
                    {
                        common.message = "Item no. not found!";
                        common.status = -1;
                        return Ok(common);
                    }
                    var ic = _context.IncomingLists.Find(iItem.IcId);
                    ic.ItemCount++;
                    _context.Update(ic);
                    var newPt = new ItemMaster
                    {
                        ItemNo = iItem.ItemNo,
                        IcId = iItem.IcId,
                        RecQty = iItem.RecQty,
                        RefNo = iItem.RefNo,
                        RefDate = iItem.RefDate == null ? null : iItem.RefDate,
                        PtCmt = iItem.PtCmt
                    };

                    _context.Add(newPt);
                    _context.SaveChanges();
                    newPt.ParentId = newPt.PtId;
                    _context.Update(newPt);
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
        [Route("UpdateItem")]
        public IActionResult UpdateItem(ItemMaster iItem)
        {
            var role = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
            CommonResponse<int> common = new();
            if (role)
            {
                try
                {
                    var pt = _context.ItemMasters.Find(iItem.PtId); var itemData = _context.ItemDatas.Find(iItem.ItemNo);
                    if (itemData == null)
                    {
                        common.message = "Item no. not found!";
                        common.status = -1;
                        return Ok(common);
                    }

                    if (pt != null)
                    {
                        pt.ItemNo = iItem.ItemNo;
                        pt.RecQty = iItem.RecQty;
                        pt.RefNo = iItem.RefNo;
                        pt.RefDate = iItem.RefDate;
                        pt.PtCmt = iItem.PtCmt;

                        _context.Update(pt);
                        _context.SaveChanges();
                        common.status = 1;
                    }
                    else
                    {
                        common.message = "An error occurred!";
                        common.status = -1;
                        return Ok(common);
                    }

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
        [Route("DeleteItem")]
        public IActionResult DeleteItem(ItemMaster iItem)
        {
            var role = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated;
            CommonResponse<int> common = new();
            if (role)
            {
                try
                {
                    var pt = _context.ItemMasters.Find(iItem.PtId); var itemData = _context.ItemDatas.Find(iItem.ItemNo);

                    if (pt != null)
                    {
                        var ic = _context.IncomingLists.Find(pt.IcId);
                        ic.ItemCount--;

                        _context.Update(ic);
                        _context.Remove(pt);
                        _context.SaveChanges();
                        common.status = 1;
                    }
                    else
                    {
                        common.message = "An error occurred!";
                        common.status = -1;
                        return Ok(common);
                    }

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

            var receiveLoc = _context.Locs.FirstOrDefault(l => l.LocType == "LocReceive");
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

        [HttpPost]
        [Route("QrListExtract")]
        public IActionResult QrListExtract(List<IFormFile> files)
        {
            List<string> ls = new();
            using (var reader = new StreamReader(files[0].OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    ls.Add(reader.ReadLine());
            }
            return Ok(ls);
        }

    }
}
