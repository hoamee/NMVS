using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NMVS.Models;
using NMVS.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Controllers
{
    public class WorkOrdersController : Controller
    {
        private readonly ApplicationDbContext _db;
        public WorkOrdersController(ApplicationDbContext db)
        {
            _db = db;
        }


        public ActionResult WoDashboard()
        {
            var woList = _db.WorkOrders.Where(x => x.Closed != true).OrderByDescending(x => x.OrdDate).ToList();

            return View(woList);
        }

        // GET: WorkOrders1
        public async Task<IActionResult> WoBrowse()
        {
            var workOrders = _db.WorkOrders.OrderByDescending(x => x.OrdDate);
            return View(await workOrders.ToListAsync());
        }

        public IActionResult CloseWo(string id)
        {
            var wo = _db.WorkOrders.Find(id);
            wo.Closed = true;
            _db.SaveChanges();
            return RedirectToAction("WoBrowse");
        }

        // GET: WorkOrders1/Details/5
        public async Task<IActionResult> WoDetails(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            WorkOrder workOrder = await _db.WorkOrders.FindAsync(id);

            var woBills = await _db.WoBills.Where(w => w.WoNbr == id).ToListAsync();
            ViewBag.woBills = woBills;
            if (workOrder == null)
            {
                return NotFound();
            }
            return View(workOrder);
        }

        // GET: WorkOrders1/Create
        public IActionResult CreateWo()
        {
            ViewBag.ItemNo = _db.ItemDatas;
            ViewBag.PrLnId = _db.ProdLines;
            return View();
        }

        // POST: WorkOrders1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWo([Bind("WoNbr,ItemNo,QtyOrd,QtyCom,SoNbr,OrdBy,OrdDate,ExpDate,PrLnId")] WorkOrder workOrder)
        {
            if (ModelState.IsValid)
            {
                if (workOrder.ExpDate < DateTime.Now.Date)
                {
                    ModelState.AddModelError("", "Expire date should greater than or equal to today");
                }
                else
                {
                    workOrder.OrdDate = DateTime.Now;
                    workOrder.OrdBy = User.Identity.Name;
                    _db.WorkOrders.Add(workOrder);
                    try
                    {
                        await _db.SaveChangesAsync();
                        return RedirectToAction("WoBrowse");
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "Duplicate Work order number, please specify an other ID");
                    }
                }
            }
            ViewBag.ItemNo = _db.ItemDatas;
            ViewBag.PrLnId = _db.ProdLines;
            return View(workOrder);
        }

        // GET: WorkOrders1/Edit/5
        public async Task<IActionResult> EditWo(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            WorkOrder workOrder = await _db.WorkOrders.FindAsync(id);
            if (workOrder == null)
            {
                return NotFound();
            }
            ViewBag.ItemNo = _db.ItemDatas;
            ViewBag.PrLnId = _db.ProdLines;
            return View(workOrder);
        }

        // POST: WorkOrders1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWo([Bind("WoNbr,ItemNo,QtyOrd,QtyCom,SoNbr,OrdBy,OrdDate,ExpDate,PrLnId")] WorkOrder workOrder)
        {
            if (ModelState.IsValid)
            {
                workOrder.OrdBy = User.Identity.Name;
                workOrder.OrdDate = DateTime.Now.Date;
                _db.Entry(workOrder).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("WoBrowse");
            }

            ViewBag.ItemNo = _db.ItemDatas;
            ViewBag.PrLnId = _db.ProdLines;
            return View(workOrder);
        }

        // POST: WorkOrders1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            WorkOrder workOrder = await _db.WorkOrders.FindAsync(id);
            _db.WorkOrders.Remove(workOrder);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
