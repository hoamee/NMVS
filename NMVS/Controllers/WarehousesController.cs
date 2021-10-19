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
    public class WarehousesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWarehouseService _service;

        public WarehousesController(ApplicationDbContext context, IWarehouseService service)
        {
            _context = context;
            _service = service;
        }

        // GET: Warehouses
        public async Task<IActionResult> Browse()
        {
            return View(await _context.Warehouses.ToListAsync());
        }

        // GET: Warehouses/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(m => m.WhCode == id);
            if (warehouse == null)
            {
                return NotFound();
            }

            return View(warehouse);
        }

        // GET: Warehouses/Create
        public IActionResult AddWarehouse()
        {
            ViewBag.SiteList = _service.GetSite();
            ViewBag.WhTypes = _service.GetWhType();
            return View();
        }

        // POST: Warehouses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWarehouse([Bind("WhCode,WhDesc,WhStatus,WhCmmt,SiCode,Type")] Warehouse warehouse)
        {
            if (ModelState.IsValid)
            {
                _context.Add(warehouse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SiteList = _service.GetSite();
            ViewBag.WhTypes = _service.GetWhType();
            return View(warehouse);
        }

        // GET: Warehouses/Edit/5
        public async Task<IActionResult> UpdateWH(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.SiteList = _service.GetSite();
            ViewBag.WhTypes = _service.GetWhType();
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }
            return View(warehouse);
        }

        // POST: Warehouses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateWH(string id, [Bind("WhCode,WhDesc,WhStatus,WhCmmt,SiCode,Type")] Warehouse warehouse)
        {
            if (id != warehouse.WhCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(warehouse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WarehouseExists(warehouse.WhCode))
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
            ViewBag.SiteList = _service.GetSite();
            ViewBag.WhTypes = _service.GetWhType();
            return View(warehouse);
        }

        // GET: Warehouses/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(m => m.WhCode == id);
            if (warehouse == null)
            {
                return NotFound();
            }

            return View(warehouse);
        }

        // POST: Warehouses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var warehouse = await _context.Warehouses.FindAsync(id);
            _context.Warehouses.Remove(warehouse);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WarehouseExists(string id)
        {
            return _context.Warehouses.Any(e => e.WhCode == id);
        }
    }
}
