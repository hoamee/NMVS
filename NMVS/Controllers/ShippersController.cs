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
    public class ShippersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShippersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Shippers
        public async Task<IActionResult> Browse()
        {
            return View(await _context.Shippers.ToListAsync());
        }

        // GET: Shippers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipper = await _context.Shippers
                .FirstOrDefaultAsync(m => m.ShpId == id);
            if (shipper == null)
            {
                return NotFound();
            }

            return View(shipper);
        }

        // GET: Shippers/Create
        public IActionResult Register()
        {
            ViewBag.Loc = _context.Locs.Where(x => x.Direct == true).ToList();
            return View();
        }

        // POST: Shippers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register([Bind("ShpId,ShpDesc,Driver,DrContact,ShpTo,ShpVia,DateIn,ActualIn,ActualOut,CheckInBy,CheckOutBy,Loc,RegisteredBy")] Shipper shipper)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(shipper);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Browse));
                }catch(Exception e)
                {
                    ModelState.AddModelError("", e.ToString());
                }
            }
            return View(shipper);
        }

        // GET: Shippers/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipper = _context.Shippers.Find(id);
            if (shipper == null)
            {
                return NotFound();
            }
            return View(shipper);
        }

        // POST: Shippers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShpId,ShpDesc,Driver,DrContact,ShpFrom,ShpTo,ShpVia,DateIn,ActualIn,ActualOut,CheckInBy,CheckOutBy,Loc,RegisteredBy")] Shipper shipper)
        {
            if (id != shipper.ShpId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shipper);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShipperExists(shipper.ShpId))
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
            return View(shipper);
        }

        // GET: Shippers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipper = await _context.Shippers
                .FirstOrDefaultAsync(m => m.ShpId == id);
            if (shipper == null)
            {
                return NotFound();
            }

            return View(shipper);
        }

        // POST: Shippers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shipper = await _context.Shippers.FindAsync(id);
            _context.Shippers.Remove(shipper);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Browse));
        }

        private bool ShipperExists(int id)
        {
            return _context.Shippers.Any(e => e.ShpId == id);
        }

        public async Task<IActionResult> CheckIn()
        {
            return View(await _context.Shippers.ToListAsync());
        }

        public async Task<IActionResult> ApproveCheckIn(int id)
        {
            var shipper = await _context.Shippers.FindAsync(id);
            if (shipper!= null)
            {
                shipper.CheckInBy = User.Identity.Name;
                shipper.ActualIn = DateTime.Now;
                _context.Update(shipper);
                _context.SaveChanges();
            }
            return Ok();
        }

        public async Task<IActionResult> CheckOut(int id)
        {
            var shipper = await _context.Shippers.FindAsync(id);
            if (shipper != null)
            {
                shipper.CheckInBy = User.Identity.Name;
                shipper.ActualIn = DateTime.Now;
                _context.Update(shipper);
                _context.SaveChanges();
            }
            return Ok();
        }
    }
}
