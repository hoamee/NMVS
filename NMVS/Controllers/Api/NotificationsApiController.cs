using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    [Route("api/Notifications")]
    [ApiController]
    public class NotificationsApiController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        ApplicationDbContext _context;
        UserManager<ApplicationUser> _userManager;


        public NotificationsApiController(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, IAllocateService allocateService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("GetNotification")]
        public IActionResult GetNotification()
        {
            CommonResponse<List<Notifiaction>> common = new();
            common.dataenum = new List<Notifiaction>();
            
            if (_httpContextAccessor.HttpContext.User.IsInRole(Helper.AppSO))
            {
                var unconfirmedSo = _context.SalesOrders.Where(x => x.Confirm == null && x.Closed == true).ToList();
                if (unconfirmedSo.Any())
                {
                    common.dataenum.Add(new Notifiaction
                    {
                        notificationType = "Sales orders",
                        message = unconfirmedSo.Count + " Unconfirmed Sales orders",
                        href = "/SalesOrders/Details/" + unconfirmedSo.FirstOrDefault().SoNbr
                    });
                }
            }

            if (_httpContextAccessor.HttpContext.User.IsInRole(Helper.CreateSO))
            {
                var unfinishedSO = _context.SalesOrders.Where(x => x.Closed == false && x.UpdatedBy == _httpContextAccessor.HttpContext.User.Identity.Name).ToList();
                var rejectedSo = _context.SalesOrders.Where(x => x.Confirm == false && x.UpdatedBy == _httpContextAccessor.HttpContext.User.Identity.Name).ToList();
                var rejectedWhSo = _context.SalesOrders.Where(x => x.RequestConfirmed == false && x.UpdatedBy == _httpContextAccessor.HttpContext.User.Identity.Name).ToList();
                if (rejectedSo.Any())
                {
                    common.dataenum.Add(new Notifiaction
                    {
                        notificationType = "Sales orders",
                        message = rejectedSo.Count + " Rejected Sales orders",
                        href = "/SalesOrders/Details/" + rejectedSo.FirstOrDefault().SoNbr
                    });
                }

                if (rejectedWhSo.Any())
                {
                    common.dataenum.Add(new Notifiaction
                    {
                        notificationType = "Sales orders",
                        message = rejectedSo.Count + " Request rejected",
                        href = "/SalesOrders/Details/" + rejectedSo.FirstOrDefault().SoNbr
                    });
                }

                if (unfinishedSO.Any())
                {
                    common.dataenum.Add(new Notifiaction
                    {
                        notificationType = "Sales orders",
                        message = unfinishedSO.Count + " unfinished SO",
                        href = "/SalesOrders/Details/" + unfinishedSO.FirstOrDefault().SoNbr
                    });
                }
            }

            if (_httpContextAccessor.HttpContext.User.IsInRole(Helper.HandleRequest))
            {
                var unconfirmedRq = _context.InvRequests.Where(x => x.Confirmed == null && x.SoConfirm == true).ToList();
                if (unconfirmedRq.Any())
                {

                    common.dataenum.Add(new Notifiaction
                    {
                        notificationType = "Inventory request",
                        message = unconfirmedRq.Count + " Inconfirmed request",
                        href = "/InvRequests/RequestDetail/" + unconfirmedRq.FirstOrDefault().RqID
                    });
                }

            }

            if (_httpContextAccessor.HttpContext.User.IsInRole(Helper.RequestInv))
            {
                var unconfirmedRq = _context.InvRequests.Where(x => x.Confirmed == false).ToList();
                if (unconfirmedRq.Any())
                {
                    common.dataenum.Add(new Notifiaction
                    {
                        notificationType = "Inventory request",
                        message = unconfirmedRq.Count + " Rejected request",
                        href = "/InvRequests/RequestDetail/" + unconfirmedRq.FirstOrDefault().RqID
                    });
                }

            }

            if (_httpContextAccessor.HttpContext.User.IsInRole(Helper.MoveInventory))
            {
                var orders = _context.AllocateOrders.Where(x => x.AlcOrdQty > (x.MovedQty + x.Reported)).ToList();
                if (orders.Any())
                {
                    common.dataenum.Add(new Notifiaction
                    {
                        notificationType = "Allocate order",
                        message = orders.Count + " unfinished orders",
                        href = "/Inquiry/MovementReport/allocate not yet"
                    });
                }

                var issueOrders = _context.IssueOrders.Where(x => x.ExpOrdQty > (x.MovedQty + x.Reported)).ToList();
                if (issueOrders.Any())
                {
                    common.dataenum.Add(new Notifiaction
                    {
                        notificationType = "Issue order",
                        message = issueOrders.Count + " unfinished orders",
                        href = "/Inquiry/MovementReport/issue not yet"
                    });
                }

            }

            if (_httpContextAccessor.HttpContext.User.IsInRole(Helper.ArrangeInventory))
            {
                var receiveLoc = _context.Locs.Where(x => x.LocType == "LocReceive").Select(x=>x.LocCode).ToList();
                var items = _context.ItemMasters.Where(x => x.PtQty - x.PtHold > 0  && receiveLoc.Contains(x.LocCode)).ToList();
                if (items.Any())
                {
                    common.dataenum.Add(new Notifiaction
                    {
                        notificationType = "Inventory arrangement",
                        message = items.Count + " unallocated. (At Receive location)",
                        href = "/Allocate/Unallocated"
                    });
                }


            }


            return Ok(common);
        }


        
    }
}
