using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NMVS.Models;
using NMVS.Models.DbModels;

namespace NMVS.Controllers
{
    [Authorize(Roles = "SupperUser")]
    public class GeneralizedCodesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GeneralizedCodesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GeneralizedCodes
        public async Task<IActionResult> Browse()
        {
            return View(await _context.GeneralizedCodes.ToListAsync());
        }

        // GET: GeneralizedCodes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var generalizedCode = await _context.GeneralizedCodes
                .FirstOrDefaultAsync(m => m.CodeNo == id);
            if (generalizedCode == null)
            {
                return NotFound();
            }

            return View(generalizedCode);
        }

        // GET: GeneralizedCodes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GeneralizedCodes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CodeNo,CodeFldName,CodeValue,CodeDesc")] GeneralizedCode generalizedCode)
        {
            if (ModelState.IsValid)
            {
                _context.Add(generalizedCode);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(generalizedCode);
        }

        // GET: GeneralizedCodes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var generalizedCode = await _context.GeneralizedCodes.FindAsync(id);
            if (generalizedCode == null)
            {
                return NotFound();
            }
            return View(generalizedCode);
        }

        // POST: GeneralizedCodes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CodeNo,CodeFldName,CodeValue,CodeDesc")] GeneralizedCode generalizedCode)
        {
            if (id != generalizedCode.CodeNo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(generalizedCode);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GeneralizedCodeExists(generalizedCode.CodeNo))
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
            return View(generalizedCode);
        }

        // GET: GeneralizedCodes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var generalizedCode = await _context.GeneralizedCodes
                .FirstOrDefaultAsync(m => m.CodeNo == id);
            if (generalizedCode == null)
            {
                return NotFound();
            }

            return View(generalizedCode);
        }

        // POST: GeneralizedCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var generalizedCode = await _context.GeneralizedCodes.FindAsync(id);
            _context.GeneralizedCodes.Remove(generalizedCode);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GeneralizedCodeExists(int id)
        {
            return _context.GeneralizedCodes.Any(e => e.CodeNo == id);
        }
    }
}
