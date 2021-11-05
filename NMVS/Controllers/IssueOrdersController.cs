using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NMVS.Models;
using NMVS.Models.DbModels;

namespace NMVS.Controllers
{
    public class IssueOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IssueOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: IssueOrders
        public IActionResult Browse()
        {

            var model = (from o in _context.IssueOrders
                         join loc in _context.Locs on o.LocCode equals loc.LocCode into oloc 
                         from ol in oloc.DefaultIfEmpty()
                         join ve in _context.Shippers on o.ToVehicle equals ve.ShpId into vo
                         from all in vo.DefaultIfEmpty()
                         join loc2 in _context.Locs on o.ToLoc equals loc2.LocCode into oloc2
                         from all2 in oloc2.DefaultIfEmpty()


                         select new IssueOrder
                         {
                             ItemNo = o.ItemNo,
                             RqID = o.RqID,
                             PtId = o.PtId,
                             OrderBy = o.OrderBy,
                             MovementTime = o.MovementTime,
                             MovedQty = o.MovedQty,
                             LocCode = o.LocCode,
                             IssueToDesc = o.IssueType == "MFG" ? all2.LocDesc : all.ShpDesc,
                             Confirm = o.Confirm,
                             ConfirmedBy = o.ConfirmedBy,
                             DetId = o.DetId,
                             ExpOrdId = o.ExpOrdId,
                             ExpOrdQty = o.ExpOrdQty,
                             IssueType = o.IssueType,
                             ToLoc = o.ToLoc,
                             ToVehicle = o.ToVehicle,
                             FromLoc = ol.LocDesc
                             
                         }).ToList();

            return View(model);
        }

        // GET: IssueOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var issueOrder = await _context.IssueOrders
                .FirstOrDefaultAsync(m => m.ExpOrdId == id);
            if (issueOrder == null)
            {
                return NotFound();
            }

            return View(issueOrder);
        }

        // GET: IssueOrders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: IssueOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExpOrdId,IssueType,ExpOrdQty,LocCode,PtId,ToLoc,ToVehicle,IssueToDesc,Confirm,ConfirmedBy,RqID,DetId,OrderBy,MovementTime")] IssueOrder issueOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(issueOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Browse));
            }
            return View(issueOrder);
        }

        // GET: IssueOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var issueOrder = await _context.IssueOrders.FindAsync(id);
            if (issueOrder == null)
            {
                return NotFound();
            }
            return View(issueOrder);
        }

        // POST: IssueOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExpOrdId,IssueType,ExpOrdQty,LocCode,PtId,ToLoc,ToVehicle,IssueToDesc,Confirm,ConfirmedBy,RqID,DetId,OrderBy,MovementTime")] IssueOrder issueOrder)
        {
            if (id != issueOrder.ExpOrdId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(issueOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IssueOrderExists(issueOrder.ExpOrdId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Browse));
            }
            return View(issueOrder);
        }

        // GET: IssueOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var issueOrder = await _context.IssueOrders
                .FirstOrDefaultAsync(m => m.ExpOrdId == id);
            if (issueOrder == null)
            {
                return NotFound();
            }

            return View(issueOrder);
        }

        // POST: IssueOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var issueOrder = await _context.IssueOrders.FindAsync(id);
            _context.IssueOrders.Remove(issueOrder);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Browse));
        }

        private bool IssueOrderExists(int id)
        {
            return _context.IssueOrders.Any(e => e.ExpOrdId == id);
        }
    }
}
