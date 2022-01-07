using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using NMVS.Common;
using NMVS.Models;
using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public class RequestService : IRequestService
    {
        private readonly ApplicationDbContext _db;

        public RequestService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CommonResponse<int>> CloseShipperNote(MfgIssueNote issueNote, string user)
        {
            CommonResponse<int> common = new();
            try
            {

                var shipper = await _db.Shippers.FindAsync(issueNote.IsNId);
                if (shipper != null)
                {
                    int orderCount = _db.IssueOrders.Where(x => x.ToVehicle == shipper.ShpId && x.Confirm != true).Count();
                    if (orderCount > 0)
                    {
                        var many = orderCount > 1 ? "Unable to finish. There are " + orderCount + " unfinished movements to this vehicle"
                            : "Unable to finish. There is an unfinished movement to this vehicle";
                        common.message = many;
                        common.status = -1;
                        return common;
                    }
                    shipper.IssueConfirmedTime = DateTime.Now;
                    shipper.IssueConfirmed = true;

                    var noteDet = (from d in _db.ShipperDets.Where(x => x.ShpId == shipper.ShpId)
                                   join p in _db.ItemMasters on d.InventoryId equals p.PtId into ptmstr
                                   from pt in ptmstr.DefaultIfEmpty()
                                   join i in _db.ItemDatas on d.ItemNo equals i.ItemNo into all
                                   from a in all.DefaultIfEmpty()
                                   join s in _db.SalesOrders on d.RqId equals s.SoNbr into sOrder
                                   from so in sOrder.DefaultIfEmpty()
                                   join c in _db.Customers on so.CustCode equals c.CustCode into soldTo
                                   from soto in soldTo.DefaultIfEmpty()
                                   join c2 in _db.Customers on so.ShipTo equals c2.CustCode into shipTo
                                   from shto in shipTo.DefaultIfEmpty()
                                   select new In01Vm
                                   {
                                       InventoryId = d.InventoryId,
                                       DetId = d.DetId,
                                       ItemName = a.ItemName,
                                       ItemNo = a.ItemNo,
                                       Quantity = d.Quantity,
                                       RqId = d.RqId,
                                       ShpId = shipper.ShpId,
                                       SoldTo = soto.CustCode,
                                       SoldToName = soto.CustName,
                                       ShipToId = shto.CustCode,
                                       ShipToAddr = shto.Addr + (!string.IsNullOrEmpty(shto.City) ? ", " + shto.City : "") + (!string.IsNullOrEmpty(shto.Ctry) ? ", " + shto.Ctry : ""),
                                       ShipToName = shto.CustName,
                                       ShipToTax = shto.TaxCode,
                                       SoldToTax = soto.TaxCode,
                                       SoltToAddr = soto.Addr + (!string.IsNullOrEmpty(soto.City) ? ", " + soto.City : "") + (!string.IsNullOrEmpty(soto.Ctry) ? ", " + soto.Ctry : ""),
                                       PkgType = a.ItemPkg,
                                       PkgQty = a.ItemPkgQty,
                                       ItemUnit = a.ItemUm,
                                       BatchNo = pt.BatchNo

                                   }).OrderBy(x => x.RqId).ToList();

                    var listSoNbr = noteDet.Select(x => x.RqId).Distinct();

                    foreach (var so in listSoNbr)
                    {
                        var noteNbr = 0;
                        var salesOrder = _db.SalesOrders.Find(so);
                        noteNbr = GetNewIssueNoteId(salesOrder, user, shipper);
                        if (noteNbr == 0)
                        {
                            common.message = "An error occurred with note creation";
                            common.status = -1;
                            return common;
                        }

                        var itemCount = 1;
                        foreach (var line in noteDet.Where(x => x.RqId == so))
                        {
                            var packCount = Convert.ToInt32(Math.Floor(line.Quantity / line.PkgQty));
                            double remainder = line.Quantity % line.PkgQty;

                            if (itemCount > 7)
                            {
                                noteNbr = GetNewIssueNoteId(salesOrder, user, shipper);
                                if (noteNbr == 0)
                                {
                                    common.message = "An error occurred with note creation";
                                    common.status = -1;
                                    return common;
                                }
                                itemCount = 1;
                            }

                            if (packCount > 0)
                            {
                                AddNoteLine(salesOrder.SoType, noteNbr, line.ItemNo, line.InventoryId, line.Quantity - remainder, packCount);
                                itemCount++;
                            }

                            if (itemCount > 7)
                            {
                                noteNbr = GetNewIssueNoteId(salesOrder, user, shipper);
                                if (noteNbr == 0)
                                {
                                    common.message = "An error occurred with note creation";
                                    common.status = -1;
                                    return common;
                                }
                                itemCount = 1;
                            }

                            if (remainder > 0)
                            {
                                AddNoteLine(salesOrder.SoType, noteNbr, line.ItemNo, line.InventoryId, remainder, 1);
                                itemCount++;


                            }



                        }

                    }


                    _db.Update(shipper);
                    _db.SaveChanges();
                    common.message = "Success!";
                    common.status = 1;
                }
                else
                {
                    common.status = 0;
                    common.message = "Shipper not found!";
                }
            }
            catch (Exception e)
            {
                common.status = -1;
                common.message = e.ToString();
            }

            return common;
        }


        public async Task<CommonResponse<int>> FinishIssueToVehicle(AllocateOrder alo, string user)
        {
            CommonResponse<int> commonResponse = new();

            try
            {
                var order = await _db.IssueOrders.FindAsync(alo.AlcOrdId);
                if (order != null)
                {
                    //amount of movement
                    var issueQty = alo.AlcOrdQty;

                    //Check if shipper is checked out
                    var shp = await _db.Shippers.FindAsync(order.ToVehicle);
                    if (shp != null)
                    {
                        if (!string.IsNullOrEmpty(shp.CheckOutBy))
                        {
                            commonResponse.message = "Shipper is already checked out! (Msg no: 400)";
                            commonResponse.status = -1;
                            return commonResponse;
                        }

                    }

                    //Check item exist
                    commonResponse.message = "Inventory id not found (Msg no: 401)";
                    var pt = await _db.ItemMasters.FindAsync(order.PtId);
                    if (pt != null)
                    {
                        //Check if error with inventory quantity
                        //Case movement quantity > available quantity (available = ptQty - ptHold)
                        //If the number after movement is negative. throw an error
                        pt.PtHold -= issueQty;
                        pt.PtQty -= issueQty;
                        if (pt.PtHold < 0 || pt.PtQty < 0)
                        {
                            commonResponse.message = "Item quantity error (Msg no: 402)";
                            commonResponse.status = -1;
                            return commonResponse;
                        }
                        _db.Update(pt);


                        //check FROM-location
                        commonResponse.message = "From loc not found! (Msg no: 403)";
                        var fromLoc = await _db.Locs.FindAsync(pt.LocCode);
                        if (fromLoc != null)
                        {
                            fromLoc.LocRemain += issueQty;

                            //Check request det
                            var reDet = _db.RequestDets.Find(order.DetId);
                            commonResponse.message = "Request not found! (Msg no: 404)";
                            var itemNote = "";
                            if (reDet != null)
                            {
                                reDet.Arranged += issueQty;
                                _db.Update(reDet);
                                // case order type = Issue
                                if (order.IssueType == "Issue")
                                {
                                    var shpDet = _db.ShipperDets.FirstOrDefault(x => x.DetId == order.DetId && x.ShpId == order.ToVehicle);
                                    if (shpDet == null)
                                    {
                                        _db.Add(new ShipperDet
                                        {
                                            InventoryId = order.PtId,
                                            DetId = order.DetId,
                                            ItemNo = pt.ItemNo,
                                            Quantity = issueQty,
                                            RqId = order.RqID,
                                            ShpId = (int)order.ToVehicle
                                        });

                                    }
                                    else
                                    {
                                        shpDet.Quantity += issueQty;
                                        _db.Update(shpDet);
                                    }

                                    _db.Add(new InventoryTransac
                                    {
                                        From = pt.LocCode,
                                        To = "Shipper Id: " + shp.ShpId,
                                        LastId = pt.PtId,
                                        NewId = null,
                                        OrderNo = order.ExpOrdId,
                                        IsAllocate = false,
                                        IsDisposed = false,
                                        MovementTime = DateTime.Now
                                    });
                                    var soDet = _db.SoDetails.Find(reDet.SodId);

                                    soDet.Shipped += issueQty;
                                    _db.Update(soDet);
                                }
                                else
                                {
                                    commonResponse.status = -1;
                                    commonResponse.message = "System error! Incorrect direction (Msg no: 200)";
                                }


                                order.ConfirmedBy = user;
                                order.MovedQty += issueQty;
                                if (order.ExpOrdQty <= (order.MovedQty + order.Reported))
                                {
                                    order.Confirm = true;
                                    order.CompletedTime = DateTime.Now;
                                }
                                pt.MovementNote += itemNote;

                                _db.Update(pt);
                                _db.Update(order);
                                _db.SaveChanges();
                                commonResponse.message = "Success"!;
                                commonResponse.status = 1;
                            }


                        }

                    }


                }
                else
                {
                    commonResponse.message = "ERROR! Order is not found!";
                    commonResponse.status = -1;
                }
            }
            catch (Exception e)
            {
                commonResponse.message = e.ToString();
            }

            return commonResponse;
        }

        public List<ItemAvailVm> GetItemAvails(string id)
        {
            var s = (from i in _db.ItemMasters.Where(x => x.ItemNo == id)
                     join d in _db.ItemDatas on i.ItemNo equals d.ItemNo
                     select new ItemAvailVm
                     {
                         ItemNo = i.ItemNo,
                         sDateIn = i.PtDateIn.Date.ToString(),
                         Desc = d.ItemName,
                         Quantity = i.PtQty - i.PtHold
                     }).ToList();

            return s;
        }

        public List<ItemMasterVm> GetItemMasterVms(RequestDet rq)
        {

            var ptMstr = _db.ItemMasters.Where(i => i.PtQty > 0 && i.ItemNo == rq.ItemNo && i.Passed == true).ToList();
            var itemVm = (from pt in ptMstr
                          join dt in _db.ItemDatas.ToList() on pt.ItemNo equals dt.ItemNo into ptdt
                          from pd in ptdt.DefaultIfEmpty()

                          join loc in _db.Locs.ToList() on pt.LocCode equals loc.LocCode into ptloc
                          from ptl in ptloc.DefaultIfEmpty()
                          select new ItemMasterVm
                          {
                              Booked = pt.PtHold,
                              DateIn = pt.PtDateIn,
                              Loc = ptl.LocDesc,
                              Lot = pt.PtLotNo,
                              Name = pd.ItemName,
                              No = pd.ItemNo,
                              Ref = pt.RefNo,
                              PackingType = pd.ItemPkg,
                              Ptid = pt.PtId,
                              Qty = pt.PtQty,
                              Sup = pt.SupCode
                          }).ToList();


            return itemVm;
        }

        public RequestDetailVm GetRequestDetail(string id)
        {
            var rqs = (from i in _db.InvRequests.Where(x => x.RqID == id)
                       select new InvRequestVm
                       {
                           RqType = i.RqType,
                           Date = i.RqDate,
                           Id = i.RqID,
                           Note = i.RqCmt,
                           Ref = i.Ref,
                           RqBy = i.RqBy,
                           SoConfirm = i.SoConfirm,
                           ConfirmationNote = i.ConfirmationNote,
                           Confirmed = i.Confirmed,
                           ConfirmedBy = i.ConfirmedBy
                       }).FirstOrDefault();

            var dets = (from det in _db.RequestDets.Where(x => x.RqID == id)
                        join it in _db.ItemDatas on det.ItemNo equals it.ItemNo into all
                        from al in all.DefaultIfEmpty()
                        select new RequestDet
                        {
                            ItemNo = det.ItemNo,
                            ItemName = al.ItemName,
                            SpecDate = det.SpecDate,
                            Quantity = det.Quantity,
                            Picked = det.Picked,
                            Arranged = det.Arranged,
                            MovementNote = det.MovementNote,
                            RequireDate = det.RequireDate,
                            DetId = det.DetId,

                        }).ToList();


            return new RequestDetailVm
            {
                Rq = rqs,
                Dets = dets,
            };

        }

        public List<InvRequestVm> GetRequestList()
        {
            var model = (from i in _db.InvRequests
                         select new InvRequestVm
                         {
                             RqType = i.RqType,
                             Date = i.RqDate,
                             Id = i.RqID,
                             Note = i.RqCmt,
                             Ref = i.Ref,
                             RqBy = i.RqBy,
                             SoConfirm = i.SoConfirm,
                             Confirmed = i.Confirmed,
                             closed = i.Closed
                         }).ToList();

            foreach(var item in model){
                item.closed = item.closed == false ? (Unfinished(item.Id) ? true : false) : true;
            }
            return model;
        }

        private bool Unfinished(string rqId)
        {
            var pickedItem = _db.RequestDets.Where(x => x.RqID == rqId).Sum(x => x.Quantity - x.Picked);
            if (pickedItem > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private int GetNewIssueNoteId(SalesOrder salesOrder, string user, Shipper shipper)
        {
            if (salesOrder.SoType == 1)
            {
                var note = new WrIssueNote
                {
                    IssuedBy = user,
                    IssuedOn = DateTime.Now,
                    Shipper = shipper.ShpId,
                    ShipTo = salesOrder.ShipTo,
                    SoldTo = salesOrder.CustCode,
                    SoNbr = salesOrder.SoNbr
                };
                _db.Add(note);
                _db.SaveChanges();
                return note.InId;

            }
            else if (salesOrder.SoType == 0)
            {
                var note = new SoIssueNote
                {
                    IssuedBy = user,
                    IssuedOn = DateTime.Now,
                    Shipper = shipper.ShpId,
                    ShipTo = salesOrder.ShipTo,
                    SoldTo = salesOrder.CustCode,
                    SoNbr = salesOrder.SoNbr
                };
                _db.Add(note);
                _db.SaveChanges();
                return note.InId;

            }
            else if (salesOrder.SoType == 2)
            {
                var note = new WtIssueNote
                {
                    IssuedBy = user,
                    IssuedOn = DateTime.Now,
                    Shipper = shipper.ShpId,
                    ShipTo = salesOrder.ShipTo,
                    SoldTo = salesOrder.CustCode,
                    SoNbr = salesOrder.SoNbr
                };
                _db.Add(note);
                _db.SaveChanges();
                return note.InId;

            }

            return 0;
        }

        private void AddNoteLine(int soType, int noteNbr, string itemNo, int ptId, double quantity, int packCount)
        {
            if (soType == 0)
            {
                _db.Add(new SoIssueNoteDet
                {
                    InId = noteNbr,
                    ItemNo = itemNo,
                    PtId = ptId,
                    Quantity = quantity,
                    PackCount = packCount,
                    InType = 0
                });
            }
            else if (soType == 1)
            {
                _db.Add(new SoIssueNoteDet
                {
                    InId = noteNbr,
                    ItemNo = itemNo,
                    PtId = ptId,
                    Quantity = quantity,
                    PackCount = packCount,
                    InType = 1
                });
            }
            else if (soType == 2)
            {
                _db.Add(new SoIssueNoteDet
                {
                    InId = noteNbr,
                    ItemNo = itemNo,
                    PtId = ptId,
                    Quantity = quantity,
                    PackCount = packCount,
                    InType = 2
                });
            }
            _db.SaveChanges();
        }

        public List<IssueNoteVm> GetListIssueNote()
        {
            var soNote = (from d in _db.SoIssueNotes
                          join s in _db.SalesOrders on d.SoNbr equals s.SoNbr into sOrder
                          from so in sOrder.DefaultIfEmpty()
                          join c in _db.Customers on so.CustCode equals c.CustCode into soldTo
                          from soto in soldTo.DefaultIfEmpty()
                          join c2 in _db.Customers on so.ShipTo equals c2.CustCode into shipTo
                          from shto in shipTo.DefaultIfEmpty()
                          join shp in _db.Shippers on d.Shipper equals shp.ShpId into shps
                          from shipper in shps.DefaultIfEmpty()
                          select new IssueNoteVm
                          {
                              SoNbr = d.SoNbr,
                              IssuedBy = d.IssuedBy,
                              IssuedOn = d.IssuedOn,
                              Id = d.InId,
                              SearchId = "so." + d.InId,
                              ShipTo = shto.CustName,
                              SoldTo = soto.CustName,
                              Vehicle = shipper.ShpDesc,
                              DriverInfo = shipper.Driver + (string.IsNullOrEmpty(shipper.DrContact) ? "" : " (" + shipper.DrContact + ")"),
                              NoteType = 0,
                              ShipperId = d.Shipper

                          }).ToList();
            var wrNote = (from d in _db.WrIssueNotes
                          join s in _db.SalesOrders on d.SoNbr equals s.SoNbr into sOrder
                          from so in sOrder.DefaultIfEmpty()
                          join c in _db.Customers on so.CustCode equals c.CustCode into soldTo
                          from soto in soldTo.DefaultIfEmpty()
                          join c2 in _db.Customers on so.ShipTo equals c2.CustCode into shipTo
                          from shto in shipTo.DefaultIfEmpty()
                          join shp in _db.Shippers on d.Shipper equals shp.ShpId into shps
                          from shipper in shps.DefaultIfEmpty()
                          select new IssueNoteVm
                          {
                              SoNbr = d.SoNbr,
                              IssuedBy = d.IssuedBy,
                              IssuedOn = d.IssuedOn,
                              Id = d.InId,
                              SearchId = "wr." + d.InId,
                              ShipTo = shto.CustName,
                              SoldTo = soto.CustName,
                              Vehicle = shipper.ShpDesc,
                              DriverInfo = shipper.Driver + (string.IsNullOrEmpty(shipper.DrContact) ? "" : " (" + shipper.DrContact + ")"),
                              NoteType = 1,
                              ShipperId = d.Shipper


                          }).ToList();
            var wtNote = (from d in _db.WtIssueNotes
                          join s in _db.SalesOrders on d.SoNbr equals s.SoNbr into sOrder
                          from so in sOrder.DefaultIfEmpty()
                          join c in _db.Customers on so.CustCode equals c.CustCode into soldTo
                          from soto in soldTo.DefaultIfEmpty()
                          join c2 in _db.Customers on so.ShipTo equals c2.CustCode into shipTo
                          from shto in shipTo.DefaultIfEmpty()
                          join shp in _db.Shippers on d.Shipper equals shp.ShpId into shps
                          from shipper in shps.DefaultIfEmpty()
                          select new IssueNoteVm
                          {
                              SoNbr = d.SoNbr,
                              IssuedBy = d.IssuedBy,
                              IssuedOn = d.IssuedOn,
                              Id = d.InId,
                              SearchId = "wt." + d.InId,
                              ShipTo = shto.CustName,
                              SoldTo = soto.CustName,
                              Vehicle = shipper.ShpDesc,
                              DriverInfo = shipper.Driver + (string.IsNullOrEmpty(shipper.DrContact) ? "" : " (" + shipper.DrContact + ")"),
                              NoteType = 2,
                              ShipperId = d.Shipper


                          }).ToList();
            return soNote.Concat(wrNote).Concat(wtNote).ToList();
        }

        public IssueNoteSoDetail GetIssueNoteDetail(int id, int sot)
        {
            IssueNoteVm soNote;

            if (sot == 0)
            {
                soNote = (from d in _db.SoIssueNotes.Where(x => x.InId == id)
                          join s in _db.SalesOrders on d.SoNbr equals s.SoNbr into sOrder
                          from sor in sOrder.DefaultIfEmpty()
                          join c in _db.Customers on sor.CustCode equals c.CustCode into soldTo
                          from soto in soldTo.DefaultIfEmpty()
                          join c2 in _db.Customers on sor.ShipTo equals c2.CustCode into shipTo
                          from shto in shipTo.DefaultIfEmpty()
                          join shp in _db.Shippers on d.Shipper equals shp.ShpId into shps
                          from shipper in shps.DefaultIfEmpty()
                          select new IssueNoteVm
                          {
                              SoNbr = d.SoNbr,
                              IssuedBy = d.IssuedBy,
                              IssuedOn = d.IssuedOn,
                              Id = d.InId,
                              SearchId = "so." + d.InId,
                              ShipTo = shto.CustName,
                              SoldTo = soto.CustName,
                              Vehicle = shipper.ShpDesc,
                              DriverInfo = shipper.Driver + (string.IsNullOrEmpty(shipper.DrContact) ? "" : " (" + shipper.DrContact + ")"),
                              NoteType = 0

                          }).FirstOrDefault();
            }
            else if (sot == 1)
            {
                soNote = (from d in _db.WrIssueNotes.Where(x => x.InId == id)
                          join s in _db.SalesOrders on d.SoNbr equals s.SoNbr into sOrder
                          from sor in sOrder.DefaultIfEmpty()
                          join c in _db.Customers on sor.CustCode equals c.CustCode into soldTo
                          from soto in soldTo.DefaultIfEmpty()
                          join c2 in _db.Customers on sor.ShipTo equals c2.CustCode into shipTo
                          from shto in shipTo.DefaultIfEmpty()
                          join shp in _db.Shippers on d.Shipper equals shp.ShpId into shps
                          from shipper in shps.DefaultIfEmpty()
                          select new IssueNoteVm
                          {
                              SoNbr = d.SoNbr,
                              IssuedBy = d.IssuedBy,
                              IssuedOn = d.IssuedOn,
                              Id = d.InId,
                              SearchId = "so." + d.InId,
                              ShipTo = shto.CustName,
                              SoldTo = soto.CustName,
                              Vehicle = shipper.ShpDesc,
                              DriverInfo = shipper.Driver + (string.IsNullOrEmpty(shipper.DrContact) ? "" : " (" + shipper.DrContact + ")"),
                              NoteType = 1

                          }).FirstOrDefault();
            }
            else
            {
                soNote = (from d in _db.WtIssueNotes.Where(x => x.InId == id)
                          join s in _db.SalesOrders on d.SoNbr equals s.SoNbr into sOrder
                          from sor in sOrder.DefaultIfEmpty()
                          join c in _db.Customers on sor.CustCode equals c.CustCode into soldTo
                          from soto in soldTo.DefaultIfEmpty()
                          join c2 in _db.Customers on sor.ShipTo equals c2.CustCode into shipTo
                          from shto in shipTo.DefaultIfEmpty()
                          join shp in _db.Shippers on d.Shipper equals shp.ShpId into shps
                          from shipper in shps.DefaultIfEmpty()
                          select new IssueNoteVm
                          {
                              SoNbr = d.SoNbr,
                              IssuedBy = d.IssuedBy,
                              IssuedOn = d.IssuedOn,
                              Id = d.InId,
                              SearchId = "so." + d.InId,
                              ShipTo = shto.CustName,
                              SoldTo = soto.CustName,
                              Vehicle = shipper.ShpDesc,
                              DriverInfo = shipper.Driver + (string.IsNullOrEmpty(shipper.DrContact) ? "" : " (" + shipper.DrContact + ")"),
                              NoteType = 2

                          }).FirstOrDefault();
            }

            var noteDet = (from d in _db.SoIssueNoteDets.Where(x => x.InId == id && x.InType == sot)
                           join i in _db.ItemDatas on d.ItemNo equals i.ItemNo into all
                           from a in all.DefaultIfEmpty()
                           select new ShipperDet
                           {
                               InventoryId = d.PtId,
                               ItemName = a.ItemName,
                               ItemNo = a.ItemNo,
                               Quantity = d.Quantity,

                           }).ToList();



            return new IssueNoteSoDetail
            {
                Dets = noteDet,
                Isn = soNote
            };
        }

        public IssueNoteShipperVm GetVehicleNoteDetail(int id)
        {
            var shp = _db.Shippers.Find(id);
            var issueNotes = GetIssueNotes(id);
            List<ShipperDet> noteDet = new();

            foreach (var item in issueNotes)
            {
                var soto = _db.Customers.Find(item.SoldTo);
                var shto = _db.Customers.Find(item.ShipTo);
                var tempList = (from d in _db.SoIssueNoteDets.Where(x => x.InId == item.Id && x.InType == item.NoteType)
                                join i in _db.ItemDatas on d.ItemNo equals i.ItemNo into all
                                from a in all.DefaultIfEmpty()

                                select new ShipperDet
                                {
                                    InventoryId = d.PtId,
                                    ItemName = a.ItemName,
                                    ItemNo = a.ItemNo,
                                    Quantity = d.Quantity,
                                    PackCount = d.PackCount,
                                    RqId = item.SoNbr,
                                    ShpId = id,
                                    SoldTo = soto.CustCode,
                                    SoldToName = soto.CustName,
                                    ShipToId = shto.CustCode,
                                    ShipToAddr = shto.Addr,
                                    ShipToName = shto.CustName
                                }).ToList();
                noteDet = noteDet.Concat(tempList).ToList();
            }

            var model = new IssueNoteShipperVm
            {
                Det = noteDet,
                Shp = shp
            };
            return model;
        }

        private List<IssueNoteVm> GetIssueNotes(int shpId)
        {

            var soNote = (from d in _db.SoIssueNotes.Where(x => x.Shipper == shpId)
                          select new IssueNoteVm
                          {
                              SoNbr = d.SoNbr,
                              IssuedBy = d.IssuedBy,
                              IssuedOn = d.IssuedOn,
                              Id = d.InId,
                              ShipTo = d.ShipTo,
                              SoldTo = d.SoldTo,
                              ShipperId = shpId,
                              NoteType = 0

                          }).ToList();

            var wrNote = (from d in _db.WrIssueNotes.Where(x => x.Shipper == shpId)
                          select new IssueNoteVm
                          {
                              SoNbr = d.SoNbr,
                              IssuedBy = d.IssuedBy,
                              IssuedOn = d.IssuedOn,
                              Id = d.InId,
                              ShipTo = d.ShipTo,
                              SoldTo = d.SoldTo,
                              ShipperId = shpId,
                              NoteType = 1

                          }).ToList();

            var wtNote = (from d in _db.WtIssueNotes.Where(x => x.Shipper == shpId)
                          select new IssueNoteVm
                          {
                              SoNbr = d.SoNbr,
                              IssuedBy = d.IssuedBy,
                              IssuedOn = d.IssuedOn,
                              Id = d.InId,
                              ShipTo = d.ShipTo,
                              SoldTo = d.SoldTo,
                              ShipperId = shpId,
                              NoteType = 2

                          }).ToList();

            return soNote.Concat(wrNote).Concat(wtNote).ToList();
        }


        public async Task<CommonResponse<UploadReport>> ImportList(string filepath, string fileName, string user)
        {
            CommonResponse<UploadReport> common = new();
            common.dataenum = new()
            {
                FileName = fileName,
                UploadBy = user,
                UploadTime = DateTime.Now,
                UploadId = user + DateTime.Now.ToString("yyyyMMddHHmmss"),
                UploadFunction = "MFG Request import"

            };
            ExcelDataHelper _eHelper = new();
            // Open the spreadsheet document for read-only access.
            using (SpreadsheetDocument document =
                SpreadsheetDocument.Open(filepath, false))
            {
                // Retrieve a reference to the workbook part.
                WorkbookPart wbPart = document.WorkbookPart;

                // Find the sheet with the supplied name, and then use that 
                // Sheet object to retrieve a reference to the first worksheet.
                Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().FirstOrDefault();

                // Throw an exception if there is no sheet.
                if (theSheet == null)
                {
                    throw new ArgumentException("sheetName");
                }

                // Retrieve a reference to the worksheet part.
                WorksheetPart wsPart =
                    (WorksheetPart)(wbPart.GetPartById(theSheet.Id));

                bool headerCorrect = true;

                headerCorrect = _eHelper.VefiryHeader(wsPart, wbPart, "A3", "A4", "A5", "A7", "B7", "C7", "D7", "", "", "",
                    "Batch No.", "Required Date", "Note", "No.", "Item code", "Item name", "Date in", "Quantity", "", "");

                //check header
                bool dateCheck;
                DateTime requiredDate = new();
                try
                {
                    requiredDate = DateTime.FromOADate(double.Parse(_eHelper.GetCellValue(wsPart, wbPart, "C4")));
                    dateCheck = true;
                }
                catch
                {
                    dateCheck = false;
                }
                string batchRef;
                batchRef = _eHelper.GetCellValue(wsPart, wbPart, "C3");
                var existRequest = _db.InvRequests.FirstOrDefault(x => x.Ref == batchRef);

                //if can not find supplier code, break
                if (string.IsNullOrEmpty(batchRef) || !headerCorrect || existRequest != null)
                {
                    if (string.IsNullOrEmpty(batchRef))
                    {
                        common.message += " Batch no. is not found.";
                    }
                    else if (existRequest != null)
                    {
                        common.message += " This batch was created before, please choose another batch no.! ";
                    }
                    if (!headerCorrect)
                    {
                        common.message += " File layout is incorrect! Please check your file.";
                    }
                    if (!dateCheck)
                    {
                        common.message += " Required date is not recognized.";
                    }


                    common.dataenum.TotalRecord = 0;
                    common.dataenum.Updated = 0;
                    common.dataenum.Errors = 1;
                    common.dataenum.Inserted = 0;

                    common.status = -1;


                    _db.Add(new UploadError
                    {
                        UploadId = common.dataenum.UploadId,
                        Error = common.message
                    });

                    _db.Add(common.dataenum);
                    await _db.SaveChangesAsync();
                    return common;
                }

                if (headerCorrect)
                {
                    var note = _eHelper.GetCellValue(wsPart, wbPart, "C5");
                    var rq = new InvRequest()
                    {
                        Closed = false,
                        ConfirmationNote = "",
                        ConfirmedBy = "",
                        Confirmed = null,
                        Ref = batchRef,
                        Reported = false,
                        ReportedNote = "",
                        RqBy = user,
                        RqCmt = note,
                        RqDate = DateTime.Now,
                        RqID = "MFG-" + batchRef,
                        RqType = "MFG",
                        SoConfirm = false
                    };
                    await _db.AddAsync(rq);
                    await _db.SaveChangesAsync();

                    int readingRow = 7;
                    //Read data from table
                    while (headerCorrect)
                    {
                        readingRow++;
                        string itemNo = _eHelper.GetCellValue(wsPart, wbPart, "B" + readingRow);
                        double quantity = 0;
                        var inputNumberCorrect = double.TryParse(_eHelper.GetCellValue(wsPart, wbPart, "E" + readingRow), out quantity);

                        if (string.IsNullOrEmpty(itemNo))
                        {
                            if (quantity == 0 || !inputNumberCorrect)
                                break;

                            common.dataenum.Errors++;
                            _db.Add(new UploadError
                            {
                                UploadId = common.dataenum.UploadId,
                                Error = "Line " + readingRow + ": Item no not found. Line skipped"
                            });
                            continue;
                        }
                        else
                        {
                            if (quantity == 0 || !inputNumberCorrect)
                            {
                                common.dataenum.Errors++;
                                _db.Add(new UploadError
                                {
                                    UploadId = common.dataenum.UploadId,
                                    Error = "Line " + readingRow + ": Quantity error. Line skipped"
                                });
                                continue;
                            }

                            ItemData item = await _db.ItemDatas.FindAsync(itemNo);
                            if (item == null)
                            {
                                common.dataenum.Errors++;
                                _db.Add(new UploadError
                                {
                                    UploadId = common.dataenum.UploadId,
                                    Error = "Line " + readingRow + ": Item no. " + itemNo + " is not existed in system. Line skipped"
                                });
                                continue;
                            }
                        }


                        try
                        {
                            DateTime dateIn;
                            var checkDate = DateTime.TryParse(_eHelper.GetCellValue(wsPart, wbPart, "D" + readingRow), out dateIn);
                            var rqDet = new RequestDet
                            {
                                Arranged = 0,
                                Closed = null,
                                Issued = 0,
                                IssueOn = null,
                                ItemNo = itemNo,
                                Picked = 0,
                                Quantity = quantity,
                                Ready = 0,
                                SpecDate = checkDate ? dateIn : null,
                                RqID = rq.RqID,
                                Shipped = null,
                                RequireDate = requiredDate,
                            };
                            _db.Add(rqDet);
                            common.dataenum.Inserted++;
                        }
                        catch (Exception e)
                        {
                            common.dataenum.Errors++;
                            _db.Add(new UploadError
                            {
                                UploadId = common.dataenum.UploadId,
                                Error = "Line " + readingRow + ": "
                               + e.Message + ";"
                            });
                        }
                    }
                }
            }

            _db.Add(common.dataenum);
            await _db.SaveChangesAsync();
            return common;
        }
    }
}
