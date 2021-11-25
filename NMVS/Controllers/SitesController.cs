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
    public class SitesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SitesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sites
        public async Task<IActionResult> Browse()
        {
            return View(await _context.Sites.ToListAsync());
        }

        // GET: Sites/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var site = await _context.Sites
                .FirstOrDefaultAsync(m => m.SiCode == id);
            if (site == null)
            {
                return NotFound();
            }

            return View(site);
        }

        // GET: Sites/Create
        public IActionResult AddSite()
        {
            return View();
        }

        // POST: Sites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSite([Bind("SiCode,SiName,Active,SiCmmt")] Site site)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Sites.FindAsync(site.SiCode) != null)
                {
                    ModelState.AddModelError("", "Site code \"" + site.SiCode + "\" is already exist. Please select another code");
                }
                else
                {
                    _context.Add(site);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(site);
        }

        // GET: Sites/Edit/5
        public async Task<IActionResult> UpdateSite(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var site = await _context.Sites.FindAsync(id);
            if (site == null)
            {
                return NotFound();
            }
            return View(site);
        }

        // POST: Sites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSite(string id, [Bind("SiCode,SiName,Active,SiCmmt")] Site site)
        {
            if (id != site.SiCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(site);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SiteExists(site.SiCode))
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
            return View(site);
        }

        // GET: Sites/Delete/5
        public async Task<IActionResult> DeleteSite(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var site = await _context.Sites
                .FirstOrDefaultAsync(m => m.SiCode == id);
            if (site == null)
            {
                return NotFound();
            }

            return View(site);
        }

        // POST: Sites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var site = await _context.Sites.FindAsync(id);
            _context.Sites.Remove(site);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SiteExists(string id)
        {
            return _context.Sites.Any(e => e.SiCode == id);
        }
    }
}
