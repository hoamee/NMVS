using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NMVS.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Controllers
{
    
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db):base(db)
        {
            _logger = logger;
            _db = db;
        }

        public ActionResult Dashboard()
        {
            var listWh = _db.Warehouses.Where(x => x.Type != "MFG").ToList();
            var listLoc = _db.Locs.Where(x=>x.LocType != "MFG").ToList();
            var listRemain = new List<double>();
            var listUsed = new List<double>();
            var listHold = new List<double>();
            var listOutGo = new List<double>();
            int fullWarehouse = 0;
            foreach (var wh in listWh)
            {
                double cap = 0;
                double used = 0;
                double hold = 0;
                double outGo = 0;
                foreach (var loc in listLoc.Where(x => x.WhCode == wh.WhCode))
                {
                    hold += _db.AllocateOrders.Where(x => x.LocCode == loc.LocCode && x.Confirm == null).Sum(x => x.AlcOrdQty - x.MovedQty)
                        + _db.IssueOrders.Where(x => x.ToLoc == loc.LocCode && x.Confirm == null).Sum(x => x.ExpOrdQty - x.MovedQty);
                    cap += loc.LocCap;
                    outGo += _db.ItemMasters.Where(x => x.LocCode == loc.LocCode && x.PtQty > 0).Sum(x => x.PtHold);
                    used += _db.ItemMasters.Where(x => x.LocCode == loc.LocCode && x.PtQty > 0).Sum(x => x.PtQty) - outGo;
                }

                if ((cap - used - hold - outGo) / cap > 0.8)
                {
                    fullWarehouse++;
                }
                listHold.Add(hold);
                listRemain.Add(cap - hold - outGo - used);
                listOutGo.Add(outGo);
                listUsed.Add(used);
            }

            ViewBag.FullWH = fullWarehouse;
            ViewBag.WhName = listWh.Select(x => x.WhDesc).ToArray();
            ViewBag.Remain = listRemain.ToArray();
            ViewBag.Hold = listHold.ToArray();
            ViewBag.Used = listUsed.ToArray();
            ViewBag.OutGo = listOutGo.ToArray();
            var receiveLoc = _db.Locs.Where(x => x.LocType == "receive").Select(s=>s.LocCode).ToList();
            ViewBag.Unallocated = _db.ItemMasters.Where(x => receiveLoc.Contains(x.LocCode) && x.PtQty > 0).ToList().Count;
            //ViewBag.UnArranged = _db.Requests.Where(x => x.Arranged < x.Quantity && x.Closed == true).ToList().Count;
            //ViewBag.UnPicked = _db.Requests.Count(x => x.Picked < x.Quantity);
            ViewBag.AllocateOrder = _db.AllocateOrders.Count(x => x.Confirm != true) + _db.IssueOrders.Count(x => x.Confirm != true);
            ViewBag.Reported =
                _db.AllocateRequests.Count(x => x.IsClosed != true && x.AlcCmmt != "" && x.AlcCmmt != null);
            return View();
        }

        public IActionResult QrGenerator(){
            return View();
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewData["User"] = _db.Users.FirstOrDefault(x => x.UserName == User.Identity.Name).FullName;

                return View();
            }
            return RedirectToAction("Login","Account");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            ViewBag.message = message;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
