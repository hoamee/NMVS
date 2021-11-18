using Microsoft.AspNetCore.Mvc;
using NMVS.Models;
using NMVS.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Controllers
{
    public class UnqualifiedsController : Controller
    {

        ApplicationDbContext _context;

        public UnqualifiedsController(ApplicationDbContext context)
        {
            _context = context;

        }

        public IActionResult OverView()
        {
            var model = _context.Unqualifieds.ToList();
            return View(model);
        }

        public IActionResult InputWarranty()
        {
            return View();
        }

        [HttpPost]
        public IActionResult InputWarranty(Unqualified uq)
        {
            var qty = uq.Quantity;
            if (string.IsNullOrEmpty(uq.SoNbr))
            {
                ModelState.AddModelError("", "Sales order no. is required");
            }
            else
            {
                //check so exist
                var so = _context.SalesOrders.FirstOrDefault(x => x.SoNbr == uq.SoNbr && x.SoType == 0);
                if (so == null)
                {
                    ModelState.AddModelError("", "Sales order " + uq.SoNbr + " is not found");
                }
                else
                {
                    //Check so line exist
                    var sodet = _context.SoDetails.Where(x => x.SoNbr == uq.SoNbr && x.ItemNo == uq.ItemNo);
                    if (!sodet.Any())
                    {
                        ModelState.AddModelError("", "No related item found");
                    }
                    else
                    {
                        //Get ordered Qty
                        var maximumQty = sodet.Sum(x => x.Quantity);
                        var wrNbr = "WR" + so.SoNbr[2..];
                        var dets = _context.SoDetails.Where(x => x.SoNbr == wrNbr);
                        double unshippedQty = 0;
                        if (dets.Any())
                        {
                            unshippedQty = _context.SoDetails.Where(x => x.SoNbr == wrNbr && x.ItemNo == uq.ItemNo).Sum(x => x.Quantity - x.Shipped);                        
                        }

                        if (qty > maximumQty)
                        {
                            ModelState.AddModelError("", "Warranty return quantity couldn't be larger than sold quantity");
                        }
                        else if (qty > (maximumQty - unshippedQty))
                        {
                            ModelState.AddModelError("", "Warranty return quantity is too large. Please check unshipped item from " + wrNbr);
                        }
                        else
                        {

                            var wrSo = _context.SalesOrders.Find(wrNbr);
                            //check if WR so is already created
                            if (wrSo == null)
                            {
                                wrSo = new SalesOrder
                                {
                                    Closed = true,
                                    Comment = "Warranty return for SO: " + so.SoNbr,
                                    SoNbr = wrNbr,
                                    CustCode = so.CustCode,
                                    DueDate = so.DueDate,
                                    OrdDate = so.OrdDate,
                                    PriceDate = so.PriceDate,
                                    ReqDate = so.ReqDate,
                                    ShipTo = so.ShipTo,
                                    UpdatedOn = DateTime.Now,
                                    UpdatedBy = User.Identity.Name,
                                    ShipVia = so.ShipVia,
                                    SoCurr = so.SoCurr,
                                    SoType = 1
                                };
                                _context.Add(wrSo);
                                _context.SaveChanges();

                                
                                
                            }
                            else
                            {
                                wrSo.Confirm = null;
                                _context.Update(wrSo);
                            }


                            var wrDet = new SoDetail
                            {
                                Quantity = qty,
                                Discount = 0,
                                ItemNo = uq.ItemNo,
                                NetPrice = 0,
                                RequiredDate = DateTime.Now.AddDays(1),
                                Shipped = 0,
                                SoNbr = wrNbr,
                                Tax = 0
                            };
                            _context.Add(wrDet);
                            _context.SaveChanges();

                            var invRq = _context.InvRequests.Find(wrNbr);
                            if (invRq == null)
                            {
                                _context.InvRequests.Add(new InvRequest
                                {
                                    RqType = "Issue",
                                    Ref = wrNbr,
                                    RqBy = User.Identity.Name,
                                    RqDate = DateTime.Now,
                                    RqID = wrNbr

                                });
                                _context.SaveChanges();
                            }

                            var rqDet = new RequestDet
                            {
                                RqID = wrNbr,
                                ItemNo = wrDet.ItemNo,
                                SpecDate = wrDet.SpecDate,
                                Arranged = 0,
                                Picked = 0,
                                Issued = 0,
                                Ready = 0,
                                RequireDate = wrDet.RequiredDate,
                                Shipped = null,
                                Quantity = wrDet.Quantity,
                                SodId = wrDet.SodId
                            };
                            _context.Add(rqDet);
                            _context.SaveChanges();

                            rqDet.SodId = wrDet.SodId;
                            wrDet.RqDetId = rqDet.DetId;
                            _context.Update(rqDet);
                            _context.Update(wrDet);


                            uq.DisposedQty = 0;
                            uq.RecycleQty = 0;
                            _context.Add(uq);
                            _context.SaveChanges();

                            return RedirectToAction(nameof(OverView));


                        }

                    }


                }
            }
            return View(uq);
        }
    }
}
