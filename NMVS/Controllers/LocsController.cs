using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NMVS.Models;
using NMVS.Models.DbModels;
using NMVS.Services;

namespace NMVS.Controllers
{
    public class LocsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILocService _service;

        public LocsController(ApplicationDbContext context, ILocService locService)
        {
            _context = context;
            _service = locService;
        }

        // GET: Locs
        public async Task<IActionResult> Browse()
        {
            return View(await _context.Locs.ToListAsync());
        }

        // GET: Locs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loc = await _context.Locs
                .FirstOrDefaultAsync(m => m.LocCode == id);
            if (loc == null)
            {
                return NotFound();
            }

            return View(loc);
        }

        // GET: Locs/Create
        public IActionResult AddLoc()
        {
            ViewBag.Whs = _service.GetWhList();
            ViewBag.LocType = _service.GetLocTypes();
            return View();
        }

        // POST: Locs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLoc([Bind("LocCode,LocDesc,LocStatus,LocCmmt,LocType,LocCap,LocRemain,LocHolding,LocOutgo,Direct,WhCode,Flammable")] Loc loc)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Locs.FindAsync(loc.LocCode) == null)
                {
                    loc.LocRemain = loc.LocCap;
                    _context.Add(loc);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Location code \"" + loc.LocCode+ "\" is already existed. Please make another choice");
                }
            }
            ViewBag.Whs = _service.GetWhList();
            ViewBag.LocType = _service.GetLocTypes();
            return View(loc);
        }

        // GET: Locs/Edit/5
        public async Task<IActionResult> UpdateLoc(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.Whs = _service.GetWhList();
            ViewBag.LocType = _service.GetLocTypes();
            var loc = await _context.Locs.FindAsync(id);
            if (loc == null)
            {
                return NotFound();
            }
            return View(loc);
        }

        // POST: Locs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateLoc(string id, [Bind("LocCode,LocDesc,LocStatus,LocCmmt,LocType,LocCap,LocRemain,LocHolding,LocOutgo,Direct,WhCode,Flammable")] Loc loc)
        {
            if (id != loc.LocCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocExists(loc.LocCode))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Whs = _service.GetWhList();
            ViewBag.LocType = _service.GetLocTypes();
            return View(loc);
        }

        // GET: Locs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loc = await _context.Locs
                .FirstOrDefaultAsync(m => m.LocCode == id);
            if (loc == null)
            {
                return NotFound();
            }

            return View(loc);
        }

        // POST: Locs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var loc = await _context.Locs.FindAsync(id);
            _context.Locs.Remove(loc);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocExists(string id)
        {
            return _context.Locs.Any(e => e.LocCode == id);
        }
    }
}
