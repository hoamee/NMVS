using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class WorkOrdersController : Controller
    {
        private readonly ApplicationDbContext _db;
        public WorkOrdersController(ApplicationDbContext db)
        {
            _db = db;
        }


        public ActionResult WoDashboard()
        {
            return View(GetListWo(true));
        }

        // GET: WorkOrders1
        public IActionResult WoBrowse()
        {

            return View(GetListWo(null));
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
            var wovm = new WoVm
            {
                WoNbr = workOrder.WoNbr,
                Closed = workOrder.Closed,
                ExpDate = workOrder.ExpDate,
                ItemName = _db.ItemDatas.FirstOrDefault(x => x.ItemNo == workOrder.ItemNo).ItemName,
                ItemNo = workOrder.ItemNo,
                OrdBy = workOrder.OrdBy,
                OrdDate = workOrder.OrdDate,
                PrLnId = workOrder.PrLnId,
                QtyCom = workOrder.QtyCom,
                QtyOrd = workOrder.QtyOrd,
                SoNbr = workOrder.SoNbr

            };

            return View(wovm);
        }

        // GET: WorkOrders1/Create
        public IActionResult CreateWo()
        {
            var workSpace = HttpContext.Session.GetString("susersite");
            ViewBag.PrLnId = _db.ProdLines.Where(x => x.SiCode == workSpace && x.Active == true);
            return View();
        }

        // POST: WorkOrders1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWo([Bind("WoNbr,ItemNo,QtyOrd,QtyCom,SoNbr,OrdBy,OrdDate,ExpDate,PrLnId")] WorkOrder workOrder)
        {
            var workSpace = HttpContext.Session.GetString("susersite");
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
                    workOrder.Site = workSpace;
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
            ViewBag.PrLnId = _db.ProdLines.Where(x => x.SiCode == workSpace && x.Active == true);
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
            var workSpace = HttpContext.Session.GetString("susersite");
            ViewBag.PrLnId = _db.ProdLines.Where(x => x.SiCode == workSpace && x.Active == true);
            return View(workOrder);
        }

        // POST: WorkOrders1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWo([Bind("WoNbr,ItemNo,QtyOrd,QtyCom,SoNbr,OrdBy,OrdDate,ExpDate,PrLnId")] WorkOrder workOrder)
        {

            var workSpace = HttpContext.Session.GetString("susersite");
            if (ModelState.IsValid)
            {
                workOrder.OrdBy = User.Identity.Name;
                workOrder.OrdDate = DateTime.Now.Date;
                workOrder.Site = workSpace;
                _db.Entry(workOrder).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("WoBrowse");
            }
            ViewBag.PrLnId = _db.ProdLines.Where(x => x.SiCode == workSpace && x.Active == true);
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

        private List<WoVm> GetListWo(bool? close)
        {
            var workSpace = HttpContext.Session.GetString("susersite");
            List<WorkOrder> ls;
            if (close == true)
            {
                ls = _db.WorkOrders.Where(x => x.Closed != true && x.Site == workSpace).OrderByDescending(x => x.OrdDate).ToList();
            }
            else
            {
                ls = _db.WorkOrders.Where(w => w.Site == workSpace).OrderByDescending(x => x.OrdDate).ToList();
            }

            var model = (from wo in ls
                         join dt in _db.ItemDatas on wo.ItemNo equals dt.ItemNo into all
                         from d in all.DefaultIfEmpty()
                         select new WoVm
                         {
                             Closed = wo.Closed,
                             ExpDate = wo.ExpDate,
                             ItemName = d.ItemName,
                             ItemNo = d.ItemNo,
                             OrdBy = wo.OrdBy,
                             OrdDate = wo.OrdDate,
                             PrLnId = wo.PrLnId,
                             QtyCom = wo.QtyCom,
                             QtyOrd = wo.QtyOrd,
                             SoNbr = wo.SoNbr,
                             WoNbr = wo.WoNbr
                         }).ToList();
            return model;
        }
    }
}
