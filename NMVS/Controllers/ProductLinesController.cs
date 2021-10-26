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
    public class ProductLinesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWarehouseService _service;

        public ProductLinesController(ApplicationDbContext context, IWarehouseService service)
        {
            _context = context;
            _service = service;
        }

        // GET: ProdLines
        public async Task<IActionResult> ProdLineBrowse()
        {
            return View(await _context.ProdLines.ToListAsync());
        }

        // GET: ProdLines/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prodLine = await _context.ProdLines
                .FirstOrDefaultAsync(m => m.PrLnId == id);
            if (prodLine == null)
            {
                return NotFound();
            }

            return View(prodLine);
        }

        // GET: ProdLines/Create
        public IActionResult ProdLineCreate()
        {
            ViewBag.SiteList = _service.GetSite();
            return View();
        }

        // POST: ProdLines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProdLineCreate([Bind("PrLnId,SiCode,Active")] ProdLine prodLine)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prodLine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ProdLineBrowse));
            }
            ViewBag.SiteList = _service.GetSite();
            return View(prodLine);
        }

        // GET: ProdLines/Edit/5
        public async Task<IActionResult> EditProdLine(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prodLine = await _context.ProdLines.FindAsync(id);
            if (prodLine == null)
            {
                return NotFound();
            }
            ViewBag.SiteList = _service.GetSite();
            return View(prodLine);
        }

        // POST: ProdLines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProdLine(string id, [Bind("PrLnId,SiCode,Active")] ProdLine prodLine)
        {
            if (id != prodLine.PrLnId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prodLine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdLineExists(prodLine.PrLnId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ProdLineBrowse));
            }
            ViewBag.SiteList = _service.GetSite();
            return View(prodLine);
        }

        // GET: ProdLines/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prodLine = await _context.ProdLines
                .FirstOrDefaultAsync(m => m.PrLnId == id);
            if (prodLine == null)
            {
                return NotFound();
            }

            return View(prodLine);
        }

        // POST: ProdLines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var prodLine = await _context.ProdLines.FindAsync(id);
            _context.ProdLines.Remove(prodLine);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ProdLineBrowse));
        }

        private bool ProdLineExists(string id)
        {
            return _context.ProdLines.Any(e => e.PrLnId == id);
        }
    }
}
