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
using NMVS.Services;

namespace NMVS.Controllers
{
    public class ShippersController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IRequestService _service;

        public ShippersController(ApplicationDbContext context, IRequestService service)
        {
            _context = context;
            _service = service;
        }

        // GET: Shippers
        public async Task<IActionResult> Browse()
        {
            if (User.IsInRole("Register vehicle") || User.IsInRole("Guard"))
            {
                var model = await _context.Shippers.ToListAsync();
                foreach (var item in model)
                {
                    item.IssueConfirmed = _context.ShipperDets.Where(x => x.ShpId == item.ShpId).Any();
                }
                return View(model);
            }
            else
            {
                return View("_Error403");
            }
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
            if (User.IsInRole("Register vehicle") || User.IsInRole("Guard"))
            {
                ViewBag.Loc = _context.Locs.Where(x => x.Direct == true).ToList();
                return View();
            }
            else
            {
                return View("_Error403");
            }

        }

        // POST: Shippers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register([Bind("ShpId,RememberMe,ShpDesc,Driver,DrContact,ShpTo,ShpVia,DateIn,ActualIn,ActualOut,CheckInBy,CheckOutBy,Loc,RegisteredBy")] Shipper shipper)
        {
            if (User.IsInRole("Register vehicle") || User.IsInRole("Guard"))
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Add(shipper);
                        _context.SaveChanges();
                        return RedirectToAction(nameof(Browse));
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e.ToString());
                    }
                }
                ViewBag.Loc = _context.Locs.Where(x => x.Direct == true).ToList();
                return View(shipper);
            }
            else
            {
                return View("_Error403");
            }

        }

        // GET: Shippers/Edit/5
        public IActionResult Edit(int? id)
        {
            if (User.IsInRole("Register vehicle") || User.IsInRole("Guard"))
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
                ViewBag.Loc = _context.Locs.Where(x => x.Direct == true).ToList();
                return View(shipper);
            }
            else
            {
                return View("_Error403");
            }

        }

        // POST: Shippers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShpId,RememberMe,ShpDesc,Driver,DrContact,ShpFrom,ShpTo,ShpVia,DateIn,ActualIn,ActualOut,CheckInBy,CheckOutBy,Loc,RegisteredBy")] Shipper shipper)
        {
            if (User.IsInRole("Register vehicle") || User.IsInRole("Guard"))
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
                ViewBag.Loc = _context.Locs.Where(x => x.Direct == true).ToList();
                return View(shipper);
            }
            else
            {
                return View("_Error403");
            }

        }

        // GET: Shippers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (User.IsInRole("Register vehicle") || User.IsInRole("Guard"))
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
            else
            {
                return View("_Error403");
            }

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
            if (User.IsInRole("Guard"))
            {
                return View(await _context.Shippers.ToListAsync());
            }
            else
            {
                return View("_Error403");
            }

        }

        public async Task<IActionResult> ApproveCheckIn(int id)
        {
            if (User.IsInRole("Guard"))
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
            else
            {
                return View("_Error403");
            }

        }

        public IActionResult CheckOut(int id)
        {
            if (User.IsInRole("Guard"))
            {
                var model = _service.GetVehicleNoteDetail(id);
                return View(model);
            }
            else
            {
                return View("_Error403");
            }

        }
    }
}
