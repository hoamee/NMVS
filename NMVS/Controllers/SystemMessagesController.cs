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
    public class SystemMessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SystemMessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SystemMessages
        public async Task<IActionResult> Index()
        {
            return View(await _context.SystemMessages.ToListAsync());
        }

        // GET: SystemMessages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemMessage = await _context.SystemMessages
                .FirstOrDefaultAsync(m => m.MsgNo == id);
            if (systemMessage == null)
            {
                return NotFound();
            }

            return View(systemMessage);
        }

        // GET: SystemMessages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SystemMessages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MsgNo,Message,Description")] SystemMessage systemMessage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(systemMessage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(systemMessage);
        }

        // GET: SystemMessages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemMessage = await _context.SystemMessages.FindAsync(id);
            if (systemMessage == null)
            {
                return NotFound();
            }
            return View(systemMessage);
        }

        // POST: SystemMessages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("MsgNo,Message,Description")] SystemMessage systemMessage)
        {
            

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(systemMessage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemMessageExists(systemMessage.MsgNo))
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
            return View(systemMessage);
        }

        // GET: SystemMessages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemMessage = await _context.SystemMessages
                .FirstOrDefaultAsync(m => m.MsgNo == id);
            if (systemMessage == null)
            {
                return NotFound();
            }

            return View(systemMessage);
        }

        // POST: SystemMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var systemMessage = await _context.SystemMessages.FindAsync(id);
            _context.SystemMessages.Remove(systemMessage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SystemMessageExists(int id)
        {
            return _context.SystemMessages.Any(e => e.MsgNo == id);
        }
    }
}
