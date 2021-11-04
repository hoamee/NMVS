using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NMVS.Models;
using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;

namespace NMVS.Controllers
{
    public class UploadReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UploadReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UploadReports
        public async Task<IActionResult> Index()
        {
            return View(await _context.UploadReports.ToListAsync());
        }

        // GET: UploadReports/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uploadReport = await _context.UploadReports
                .FirstOrDefaultAsync(m => m.UploadId == id);
            var errors = await _context.UploadErrors.Where(x => x.UploadId == id).ToListAsync();
            var model = new UploadReportVm
            {
                UploadErrors = errors,
                UploadReport = uploadReport
            };
            if (uploadReport == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // GET: UploadReports/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UploadReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UploadId,UploadTime,UploadBy,FileName,UploadFunction,TotalRecord,Inserted,Updated,Errors")] UploadReport uploadReport)
        {
            if (ModelState.IsValid)
            {
                _context.Add(uploadReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(uploadReport);
        }

        // GET: UploadReports/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uploadReport = await _context.UploadReports.FindAsync(id);
            if (uploadReport == null)
            {
                return NotFound();
            }
            return View(uploadReport);
        }

        // POST: UploadReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UploadId,UploadTime,UploadBy,FileName,UploadFunction,TotalRecord,Inserted,Updated,Errors")] UploadReport uploadReport)
        {
            if (id != uploadReport.UploadId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(uploadReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UploadReportExists(uploadReport.UploadId))
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
            return View(uploadReport);
        }

        // GET: UploadReports/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var uploadReport = await _context.UploadReports
                .FirstOrDefaultAsync(m => m.UploadId == id);
            if (uploadReport == null)
            {
                return NotFound();
            }

            return View(uploadReport);
        }

        // POST: UploadReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var uploadReport = await _context.UploadReports.FindAsync(id);
            _context.RemoveRange(_context.UploadErrors.Where(x => x.UploadId == id));
            _context.UploadReports.Remove(uploadReport);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UploadReportExists(string id)
        {
            return _context.UploadReports.Any(e => e.UploadId == id);
        }
    }
}
