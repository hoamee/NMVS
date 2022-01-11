using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NMVS.Common;
using NMVS.Models;
using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using NMVS.Services;
using System.Collections.Generic;
using System.Linq;

namespace NMVS.Controllers.Api
{
    [Route("api/Shared")]
    [ApiController]
    public class SharedApiController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        ApplicationDbContext _context;
        UserManager<ApplicationUser> _userManager;


        public SharedApiController(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, IAllocateService allocateService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("GetNotification")]
        public IActionResult GetNotification()
        {

            var workSpace = HttpContext.Session.GetString("susersite");
            if (string.IsNullOrEmpty(workSpace))
            {
                var usr = _context.Users.FirstOrDefault(x => x.UserName == HttpContext.User.Identity.Name);
                workSpace = usr.WorkSpace;
                HttpContext.Session.SetString("susersite", workSpace);
            }

            CommonResponse<List<Notification>> common = new();
            common.dataenum = new List<Notification>();
            common.message = workSpace;
            var http = _httpContextAccessor.HttpContext;

            if (http.User.IsInRole(Helper.AppSO))
            {
                var unconfirmedSo = _context.SalesOrders.Where(x => x.Confirm == null && x.Closed == true).ToList();
                if (unconfirmedSo.Any())
                {
                    common.dataenum.Add(new Notification
                    {
                        notificationType = "Sales orders",
                        message = unconfirmedSo.Count + " Unconfirmed Sales orders",
                        href = "/SalesOrders/Details/" + unconfirmedSo.FirstOrDefault().SoNbr
                    });
                }
            }

            if (http.User.IsInRole(Helper.CreateSO))
            {
                var unfinishedSO = _context.SalesOrders.Where(x => x.Closed == false
                && x.UpdatedBy == http.User.Identity.Name).ToList();

                var rejectedSo = _context.SalesOrders.Where(x => x.Confirm == false
                && x.UpdatedBy == http.User.Identity.Name).ToList();

                var rejectedWhSo = _context.SalesOrders.Where(x => x.RequestConfirmed == false
                && x.UpdatedBy == http.User.Identity.Name).ToList();
                if (rejectedSo.Any())
                {
                    common.dataenum.Add(new Notification
                    {
                        notificationType = "Sales orders",
                        message = rejectedSo.Count + " Rejected Sales orders",
                        href = "/SalesOrders/Details/" + rejectedSo.FirstOrDefault().SoNbr
                    });
                }

                if (rejectedWhSo.Any())
                {
                    common.dataenum.Add(new Notification
                    {
                        notificationType = "Sales orders",
                        message = rejectedSo.Count + " Request rejected",
                        href = "/SalesOrders/Details/" + rejectedSo.FirstOrDefault().SoNbr
                    });
                }

                if (unfinishedSO.Any())
                {
                    common.dataenum.Add(new Notification
                    {
                        notificationType = "Sales orders",
                        message = unfinishedSO.Count + " unfinished SO",
                        href = "/SalesOrders/Details/" + unfinishedSO.FirstOrDefault().SoNbr
                    });
                }
            }

            if (http.User.IsInRole(Helper.HandleRequest))
            {
                var unconfirmedRq = _context.InvRequests.Where(x => x.Confirmed == null
                    && x.SoConfirm == true
                    && x.Site == workSpace).ToList();

                if (unconfirmedRq.Any())
                {

                    common.dataenum.Add(new Notification
                    {
                        notificationType = "Inventory request",
                        message = unconfirmedRq.Count + " Inconfirmed request",
                        href = "/InvRequests/RequestDetail/" + unconfirmedRq.FirstOrDefault().RqID
                    });
                }

            }

            if (http.User.IsInRole(Helper.RequestInv))
            {
                var unconfirmedRq = _context.InvRequests.Where(x => x.Confirmed == false
                    && x.RqBy == http.User.Identity.Name
                    && x.Site == workSpace).ToList();
                if (unconfirmedRq.Any())
                {
                    common.dataenum.Add(new Notification
                    {
                        notificationType = "Inventory request",
                        message = unconfirmedRq.Count + " Rejected request",
                        href = "/InvRequests/RequestDetail/" + unconfirmedRq.FirstOrDefault().RqID
                    });
                }

            }

            if (http.User.IsInRole(Helper.MoveInventory))
            {
                var orders = _context.AllocateOrders.Where(x => x.AlcOrdQty > (x.MovedQty + x.Reported)
                    && x.Site == workSpace).ToList();
                if (orders.Any())
                {
                    common.dataenum.Add(new Notification
                    {
                        notificationType = "Allocate order",
                        message = orders.Count + " unfinished orders",
                        href = "/Inquiry/MovementReport/allocate not yet"
                    });
                }

                var issueOrders = _context.IssueOrders.Where(x => x.ExpOrdQty > (x.MovedQty + x.Reported)
                    && x.Site == workSpace).ToList();
                if (issueOrders.Any())
                {
                    common.dataenum.Add(new Notification
                    {
                        notificationType = "Issue order",
                        message = issueOrders.Count + " unfinished orders",
                        href = "/Inquiry/MovementReport/issue not yet"
                    });
                }

            }

            if (http.User.IsInRole(Helper.ArrangeInventory))
            {
                var items = _context.ItemMasters.Where(x => x.Passed == true
                    && string.IsNullOrEmpty(x.LocCode)
                    && x.PtQty > 0
                    && x.Site == workSpace).ToList();
                if (items.Any())
                {
                    common.dataenum.Add(new Notification
                    {
                        notificationType = "Inventory arrangement",
                        message = items.Count + " unallocated.",
                        href = "/Allocate/Unallocated"
                    });
                }


            }


            return Ok(common);
        }

        [HttpGet]
        [Route("GetWorkSpace")]
        public IActionResult GetWorkSpace()
        {
            var site = _context.Sites.ToList();
            return Ok(site);
        }
    }
}
