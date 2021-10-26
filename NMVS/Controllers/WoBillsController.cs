using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NMVS.Models;
using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Controllers
{
    public class WoBillsController : Controller
    {

        private readonly ApplicationDbContext _db;
        public WoBillsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Bills()
        {
            string usr = User.Identity.Name;

            var model = (from b in _db.WoBills.Where(x => x.Assignee == usr).ToList()
                         join w in _db.WorkOrders on b.WoNbr equals w.WoNbr

                         select new BillVM
                         {
                             WoNbr = w.WoNbr,
                             Assignee = b.Assignee,
                             ComQty = b.ComQty,
                             DueDate = b.DueDate,
                             IsClosed = b.IsClosed,
                             LastUpdate = b.LastUpdate,
                             OrdDate = b.OrdDate,
                             OrdQty =b.OrdQty,
                             Reporter = b.Reporter,
                             WoBillNbr = b.WoBillNbr, 
                             Product = w.ItemNo,
                             WoClosed = w.Closed
                         }).ToList();

            return View(model);
        }

        public async Task<IActionResult> CloseBill(int id)
        {
            var bill = await _db.WoBills.FindAsync(id);
            bill.IsClosed = !bill.IsClosed;
            await _db.SaveChangesAsync();
            return Json("Success");
        }

        // GET: WoBills/Create
        public IActionResult PushBill(string id)
        {
            ViewBag.Assignee = _db.Users.ToList();
            ViewBag.WoNbr = id;
            return View();
        }

        // POST: WoBills/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PushBill([Bind("WoBillNbr,WoNbr,OrdQty,ComQty,Assignee,Reporter,OrdDate,DueDate")] WoBill woBill)
        {
            if (ModelState.IsValid)
            {
                woBill.ComQty = 0;
                woBill.OrdDate = DateTime.Now.Date;

                _db.WoBills.Add(woBill);
                await _db.SaveChangesAsync();
                return RedirectToAction("WoDetails", "WorkOrders", new { id = woBill.WoNbr });
            }

            ViewBag.Assignee = _db.Users.ToList();
            ViewBag.WoNbr = woBill.WoNbr;
            return View(woBill);
        }

        // GET: WoBills/Edit/5
        public async Task<IActionResult> UpdateBill(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            WoBill woBill = await _db.WoBills.FindAsync(id);
            if (woBill == null)
            {
                return NotFound();
            }
            //ViewBag.WoNbr = new SelectList(_db.WorkOrders, "WoNbr", "ItemNo", woBill.WoNbr);
            return View(woBill);
        }

        // POST: WoBills/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBill([Bind("WoBillNbr,WoNbr,OrdQty,ComQty,Assignee,Reporter,OrdDate,DueDate")] WoBill woBill)
        {
            if (ModelState.IsValid)
            {
                var complete = _db.WoBills.Where(x => x.WoNbr == woBill.WoNbr && x.WoBillNbr != woBill.WoBillNbr).Select(z => z.ComQty).Sum();
                var wo = _db.WorkOrders.Find(woBill.WoNbr);
                if (wo != null)
                {
                    wo.QtyCom = complete + woBill.ComQty;
                }
                woBill.LastUpdate = DateTime.Now;
                woBill.Reporter = User.Identity.Name;
                _db.Update(woBill);
                await _db.SaveChangesAsync();
                return RedirectToAction("Bills");
            }
            //ViewBag.WoNbr = new SelectList(db.WorkOrders, "WoNbr", "ItemNo", woBill.WoNbr);
            return View(woBill);
        }
    }
}
