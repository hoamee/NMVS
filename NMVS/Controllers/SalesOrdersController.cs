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

    public class SalesOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICustomerService _custService;
        private readonly ISoService _soService;

        public SalesOrdersController(ApplicationDbContext context, ICustomerService customerService, ISoService soService)
        {
            _custService = customerService;
            _context = context;
            _soService = soService;
        }

        // GET: SalesOrders
        public IActionResult Browse()
        {
            return View(_soService.GetBrowseData());
        }

        // GET: SalesOrders/Details/5
        public IActionResult Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = _soService.GetSoDetail(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // GET: SalesOrders/NewSo
        public IActionResult NewSo()
        {
            ViewBag.Customers = _custService.GetCustomerList();
            return View();
        }

        // POST: SalesOrders/NewSo
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewSo([Bind("SoNbr,CustCode,ShipTo,OrdDate,ReqDate,DueDate,PriceDate,SoCurr,ShipVia,Comment,Confirm,ConfirmBy,UpdatedBy,UpdatedOn")] SalesOrder salesOrder)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    salesOrder.UpdatedBy = User.Identity.Name;
                    salesOrder.UpdatedOn = DateTime.Now;
                    _context.Add(salesOrder);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Browse));
                }
                catch
                {
                    ModelState.AddModelError("", "SO number already existed");
                }
            }
            ViewBag.Customers = _custService.GetCustomerList();
            return View(salesOrder);
        }

        // GET: SalesOrders/Edit/5
        public async Task<IActionResult> Update(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrder = await _context.SalesOrders.FindAsync(id);
            if (salesOrder == null)
            {
                return NotFound();
            }
            ViewBag.Customers = _custService.GetCustomerList();
            return View(salesOrder);
        }

        // POST: SalesOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string id, [Bind("SoNbr,CustCode,ShipTo,OrdDate,ReqDate,DueDate,PriceDate,SoCurr,ShipVia,Comment,Confirm,ConfirmBy,UpdatedBy,UpdatedOn")] SalesOrder salesOrder)
        {
            if (id != salesOrder.SoNbr)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salesOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesOrderExists(salesOrder.SoNbr))
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
            ViewBag.Customers = _custService.GetCustomerList();
            return View(salesOrder);
        }

        public async Task<ActionResult> SoConfirm(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            SalesOrder saleOrder = await _context.SalesOrders.FindAsync(id);

            if (saleOrder == null)
            {
                return NotFound();
            }
            saleOrder.Confirm = true;
            saleOrder.ConfirmBy = User.Identity.Name;

            var invRequest = await _context.InvRequests.FindAsync(id);
            if (invRequest != null)
            {
                invRequest.SoConfirm = true;
            }
            else
            {
                invRequest = new InvRequest()
                {
                    RqType = "Issue",
                    Ref = id,
                    RqCmt = saleOrder.Comment,
                    RqBy = saleOrder.UpdatedBy,
                    SoConfirm = saleOrder.Confirm,
                    RqDate = DateTime.Now,
                    RqID = id
                };
                _context.InvRequests.Add(invRequest);
                await _context.SaveChangesAsync();
            }


            _context.RequestDets.RemoveRange(_context.RequestDets.Where(x => x.RqID == id));
            await _context.SaveChangesAsync();

            var soDets = await _context.SoDetails.Where(w => w.SoNbr == saleOrder.SoNbr).ToListAsync();
            foreach (var line in soDets)
            {
                var rqDet = new RequestDet()
                {
                    Arranged = 0,
                    Issued = 0,
                    Closed = null,
                    Picked = 0,
                    ItemNo = line.ItemNo,
                    RequireDate = line.RequiredDate,
                    Shipped = null,
                    Quantity = line.Quantity,
                    SpecDate = line.SpecDate,
                    RqID = id
                };
                _context.Add(rqDet);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET: SalesOrders/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salesOrder = await _context.SalesOrders
                .FirstOrDefaultAsync(m => m.SoNbr == id);
            if (salesOrder == null)
            {
                return NotFound();
            }

            return View(salesOrder);
        }

        // POST: SalesOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var salesOrder = await _context.SalesOrders.FindAsync(id);
            _context.SalesOrders.Remove(salesOrder);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalesOrderExists(string id)
        {
            return _context.SalesOrders.Any(e => e.SoNbr == id);
        }

        // GET: SalesOrderPartial/Create
        public ActionResult SoDetCreate(string id)
        {
            
            ViewBag.SoNbr = id;
            ViewBag.ItemList = _soService.GetItemNAvail();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SoDetCreate([Bind("SodId,SoNbr,DateIn,ItemNo,Quantity,Discount,RequiredDate,NetPrice,Tax")] SoDetail soDet)
        {
            if (ModelState.IsValid)
            {
                var soId = soDet.SoNbr;
                if (soDet.SpecDate != null)
                {
                    soDet.SpecDate = Convert.ToDateTime(soDet.SpecDate);
                }
                var so = await _context.SoDetails.Where(x => x.ItemNo == soDet.ItemNo
                                                                                 && x.Discount == soDet.Discount &&
                                                                                 soDet.NetPrice == x.NetPrice &&
                                                                                 x.Tax == soDet.Tax
                                                                                 && x.SpecDate == soDet.SpecDate).FirstOrDefaultAsync();
                if (so != null)
                {
                    so.Quantity += soDet.Quantity;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _context.SoDetails.Add(soDet);
                    await _context.SaveChangesAsync();
                }

                var invRq = await _context.InvRequests.FindAsync(soId);
                if (invRq == null)
                {
                    _context.InvRequests.Add(new InvRequest
                    {
                        RqType = "Issue",
                        Ref = soId,
                        RqBy = User.Identity.Name,
                        RqDate = DateTime.Now,
                        RqID = soId
                        
                    });
                    await _context.SaveChangesAsync();
                }

                foreach (var rq in _context.RequestDets.Where(x => x.RqID == soId))
                {
                    _context.RequestDets.Remove(rq);
                }

                foreach (var det in _context.SoDetails.Where(x => x.SoNbr == soDet.SoNbr))
                {
                    _context.RequestDets.Add(new RequestDet
                    {
                        RqID = det.SoNbr,
                        ItemNo = det.ItemNo,
                        SpecDate = det.SpecDate,
                        Arranged = 0,
                        Picked = 0,
                        Issued = 0,
                        Ready = 0,
                        RequireDate = det.RequiredDate,
                        Shipped = null,
                        Quantity = det.Quantity
                    });
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = soId });
            }

            ViewBag.SoNbr = soDet.SodId;
            ViewBag.ItemList = _soService.GetItemNAvail();
            return View(soDet);
        }

    }
}
