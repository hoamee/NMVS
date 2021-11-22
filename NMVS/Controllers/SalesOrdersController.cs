using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NMVS.Models;
using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using NMVS.Services;

namespace NMVS.Controllers
{
    [Authorize(Roles = "Create sales order, Approve SO")]
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
        public async Task<IActionResult> NewSo([Bind("SoNbr,SoType,CustCode,ShipTo,OrdDate,ReqDate,DueDate,PriceDate,SoCurr,ShipVia,Comment,Confirm,ConfirmBy,UpdatedBy,UpdatedOn")] SalesOrder salesOrder)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    salesOrder.SoNbr = _soService.GetSoNbr(salesOrder.SoNbr, salesOrder.SoType);
                    if (salesOrder.SoNbr == "" && salesOrder.SoType == 1)
                    {
                        ModelState.AddModelError("", "System coundn't found related SO. " + salesOrder.SoNbr);
                    }
                    else if (salesOrder.SoNbr == "uc")
                    {
                        ModelState.AddModelError("", "Parent SO is unclosed");
                    }
                    else if (salesOrder.SoNbr == "uq404")
                    {
                        ModelState.AddModelError("", "No warranty item found in Unqualified list");
                    }
                    else
                    {
                        var so = await _context.SalesOrders.FindAsync(salesOrder.SoNbr);
                        if (so != null)
                        {
                            return RedirectToAction(nameof(Details), new { id = salesOrder.SoNbr });
                        }

                        salesOrder.UpdatedBy = User.Identity.Name;
                        salesOrder.UpdatedOn = DateTime.Now;
                        _context.Add(salesOrder);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Browse));
                    }
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
        public async Task<IActionResult> Update(string id, [Bind("SoNbr,SoType,CustCode,ShipTo,OrdDate,ReqDate,DueDate,PriceDate,SoCurr,ShipVia,Comment,Confirm,ConfirmBy,UpdatedBy,UpdatedOn")] SalesOrder salesOrder)
        {
            if (id != salesOrder.SoNbr)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    salesOrder.UpdatedOn = DateTime.Now;
                    salesOrder.UpdatedBy = User.Identity.Name;
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
        public async Task<ActionResult> SoDetCreate([Bind("SodId,SoNbr,SpecDate,ItemNo,Quantity,Discount,RequiredDate,NetPrice,Tax")] SoDetail soDet)
        {
            var soId = soDet.SoNbr;
            if (ModelState.IsValid)
            {
                var dataReady = true;
                var uq = _context.Unqualifieds.Where(x => x.SoNbr == soDet.SoNbr);
                if (soDet.SpecDate != null)
                {
                    soDet.SpecDate = Convert.ToDateTime(soDet.SpecDate);
                    var availItem = _context.ItemMasters.Where(x => x.ItemNo == soDet.ItemNo && x.PtDateIn.Date == soDet.SpecDate).Sum(x => x.PtQty - x.PtHold);
                    if (availItem < soDet.Quantity)
                    {
                        ModelState.AddModelError("", "Input quantity couldn't be greater than available! (" + availItem + ")");
                        dataReady = false;
                    }
                }
                if (dataReady)
                {
                    var so = await _context.SoDetails.Where(x => x.ItemNo == soDet.ItemNo
                                                                                 && x.Discount == soDet.Discount &&
                                                                                 soDet.NetPrice == x.NetPrice &&
                                                                                 x.Tax == soDet.Tax
                                                                                 && x.SpecDate == soDet.SpecDate).FirstOrDefaultAsync();

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

                    if (so != null)
                    {
                        so.Quantity += soDet.Quantity;
                        var rqD = _context.RequestDets.Find(so.RqDetId);
                        rqD.Quantity = so.Quantity;
                        _context.Update(rqD);
                        _context.Update(so);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.SoDetails.Add(soDet);
                        var rqDet = new RequestDet
                        {
                            RqID = soId,
                            ItemNo = soDet.ItemNo,
                            SpecDate = soDet.SpecDate,
                            Arranged = 0,
                            Picked = 0,
                            Issued = 0,
                            Ready = 0,
                            RequireDate = soDet.RequiredDate,
                            Shipped = null,
                            Quantity = soDet.Quantity,
                            SodId = soDet.SodId
                        };
                        await _context.SaveChangesAsync();
                        _context.Add(rqDet);
                        _context.SaveChanges();
                        rqDet.SodId = soDet.SodId;
                        soDet.RqDetId = rqDet.DetId;
                        _context.Update(rqDet);
                        _context.Update(soDet);
                        await _context.SaveChangesAsync();
                    }
                    return RedirectToAction("Details", new { id = soId });
                }
            }

            ViewBag.SoNbr = soId;
            ViewBag.ItemList = _soService.GetItemNAvail();
            return View(soDet);
        }

        public ActionResult UpdateSodet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var soDet = _context.SoDetails.Find(id);
            if (soDet == null)
            {
                return NotFound();
            }
            ViewBag.ItemList = _soService.GetItemNAvail();
            return View(soDet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateSodet([Bind("SodId,SoNbr,SpecDate,ItemNo,Quantity,Discount,RequiredDate,NetPrice,RqDetId,Tax,Shipped")] SoDetail soDet)
        {
            var soId = soDet.SoNbr;
            if (ModelState.IsValid)
            {
                var rqDet = _context.RequestDets.Find(soDet.RqDetId);
                var invRq = await _context.InvRequests.FindAsync(soId);
                while (true)
                {

                    //if spec date specified, check if input quantity > available
                    if (soDet.SpecDate != null)
                    {
                        soDet.SpecDate = Convert.ToDateTime(soDet.SpecDate);
                        var availItem = _context.ItemMasters.Where(x => x.ItemNo == soDet.ItemNo && x.PtDateIn.Date == soDet.SpecDate).Sum(x => x.PtQty - x.PtHold);
                        if (availItem < (soDet.Quantity - rqDet.Picked))
                        {
                            ModelState.AddModelError("", "Input quantity couldn't be greater than available! (" + availItem + ")");
                            break;
                        }
                    }

                    //Case 1: Request is rejected or denied:
                    //Compare & check if input quantity < Issued quantity: Rerturn an error

                    if (invRq.Confirmed == false || invRq.Reported == true)
                    {
                        if (soDet.Quantity < soDet.Shipped)
                        {
                            ModelState.AddModelError("", "New quantity should greater than or equal to issued quantity");
                            break;
                        }
                    }

                    //Case 2: Request is unconfirmed: No compare needed

                    soDet.Shipped = rqDet.Arranged;
                    rqDet.Quantity = soDet.Quantity;
                    _context.Update(rqDet);
                    _context.Update(soDet);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = soId });
                }
            }
            else
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors);
                ModelState.AddModelError("", string.Join(", ", allErrors));
            }

            ViewBag.ItemList = _soService.GetItemNAvail();
            return View(soDet);
        }

        
        public async Task<JsonResult> UploadList(IList<IFormFile> files)
        {
            var common = new CommonResponse<UploadReport>();
            foreach (IFormFile source in files)
            {
                string filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');

                filename = EnsureCorrectFilename(filename);

                using (FileStream output = System.IO.File.Create("uploads/" + filename))
                    await source.CopyToAsync(output);

                common = await _soService.ImportWarranty("uploads/" + filename, filename, User.Identity.Name);

            }


            return Json(common);
        }

        private string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            return filename;
        }

    }
}
