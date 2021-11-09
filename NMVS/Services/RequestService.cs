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

            var ptMstr = _db.ItemMasters.Where(i => i.PtQty > 0 && i.ItemNo == rq.ItemNo).OrderBy(x => x.PtDateIn)
                .ToList();
            var itemVm = (from pt in ptMstr
                          join dt in _db.ItemDatas.ToList() on pt.ItemNo equals dt.ItemNo
                          join loc in _db.Locs.ToList() on pt.LocCode equals loc.LocCode
                          select new ItemMasterVm
                          {
                              Booked = pt.PtHold,
                              DateIn = pt.PtDateIn,
                              Loc = loc.LocDesc,
                              Lot = pt.PtLotNo,
                              Name = dt.ItemName,
                              No = dt.ItemNo,
                              PackingType = dt.ItemPkg,
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

            var dets = _db.RequestDets.Where(x => x.RqID == id).ToList();


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
                             SoConfirm = i.SoConfirm
                         }).ToList();


            return model;
        }

        //public async Task<CommonResponse<UploadReport>> ImportList(string filepath, string fileName, string user)
        //{
        //    CommonResponse<UploadReport> common = new();
        //    common.dataenum = new()
        //    {
        //        FileName = fileName,
        //        UploadBy = user,
        //        UploadTime = DateTime.Now,
        //        UploadId = user + DateTime.Now.ToString("yyyyMMddHHmmss"),
        //        UploadFunction = "MFG Request import"

        //    };
        //    ExcelDataHelper _eHelper = new();
        //    // Open the spreadsheet document for read-only access.
        //    using (SpreadsheetDocument document =
        //        SpreadsheetDocument.Open(filepath, false))
        //    {
        //        // Retrieve a reference to the workbook part.
        //        WorkbookPart wbPart = document.WorkbookPart;

        //        // Find the sheet with the supplied name, and then use that 
        //        // Sheet object to retrieve a reference to the first worksheet.
        //        Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().FirstOrDefault();

        //        // Throw an exception if there is no sheet.
        //        if (theSheet == null)
        //        {
        //            throw new ArgumentException("sheetName");
        //        }

        //        // Retrieve a reference to the worksheet part.
        //        WorksheetPart wsPart =
        //            (WorksheetPart)(wbPart.GetPartById(theSheet.Id));

        //        int readingRow = 12;

        //        var delDate = DateTime.Now.Date;
        //        bool headerCorrect = true;

        //        headerCorrect = _eHelper.VefiryHeader(wsPart, wbPart, "A3", "A4", "A6", "B6", "C6", "D6", "", "", "", "",
        //            "Batch No.", "Note", "No.", "Item code", "Item name", "Date in", "Quantity", "", "", "");

        //        //check header
        //        string batchRef;
        //        batchRef = _eHelper.GetCellValue(wsPart, wbPart, "C3");
        //        var existRequest = await _db.InvRequests.FindAsync("MFG-" + batchRef);
        //        //if can not find supplier code, break
        //        if (string.IsNullOrEmpty(batchRef) || !headerCorrect || existRequest != null)
        //        {
        //            if (string.IsNullOrEmpty(batchRef))
        //            {
        //                common.message += "Batch no. is not found ";
        //            }
        //            else if (existRequest != null)
        //            {
        //                common.message += "This batch was created before, please choose another batch no.! ";
        //            }
        //            if (!headerCorrect)
        //            {
        //                common.message += "File header is incorrect! Please check your file";
        //            }
        //            common.dataenum.TotalRecord = 0;
        //            common.dataenum.Updated = 0;
        //            common.dataenum.Errors = 1;
        //            common.dataenum.Inserted = 0;

        //            common.status = -1;


        //            _db.Add(new UploadError
        //            {
        //                UploadId = common.dataenum.UploadId,
        //                Error = common.message
        //            });

        //            _db.Add(common.dataenum);
        //            await _db.SaveChangesAsync();
        //            return common;
        //        }

                    
                    

        //        if (headerCorrect)
        //        {
        //            var note = _eHelper.GetCellValue(wsPart, wbPart, "C3");
        //            var rq = new InvRequest()
        //            {

        //            };
        //            await _db.AddAsync(incomingList);
        //            await _db.SaveChangesAsync();
        //            int icId = incomingList.IcId;

        //            //Read data from table
        //            while (headerCorrect)
        //            {
        //                readingRow++;
        //                string lot, supRef, note;
        //                DateTime refDate;
        //                string itemNo = _eHelper.GetCellValue(wsPart, wbPart, "B" + readingRow).Trim();
        //                //Check Supplier exist
        //                ItemData item = await _db.ItemDatas.FindAsync(itemNo);
        //                var checkQty = double.TryParse(_eHelper.GetCellValue(wsPart, wbPart, "E" + readingRow), out double qty);
        //                var err = itemNo;
        //                incomingList = _db.IncomingLists.Find(icId);

        //                try
        //                {
        //                    lot = _eHelper.GetCellValue(wsPart, wbPart, "D" + readingRow);
        //                    note = _eHelper.GetCellValue(wsPart, wbPart, "H" + readingRow);
        //                    supRef = _eHelper.GetCellValue(wsPart, wbPart, "F" + readingRow);
        //                    var checkDate = DateTime.TryParse(_eHelper.GetCellValue(wsPart, wbPart, "G" + readingRow), out refDate);
        //                    if (item != null && checkQty)
        //                    {

        //                        var pt = _db.ItemMasters
        //                            .Where(x => x.ItemNo == item.ItemNo
        //                                && x.IcId == icId
        //                                && x.PtLotNo == lot
        //                                && x.RefDate == refDate
        //                                && x.RefNo == supRef
        //                                && x.PtCmt == note
        //                            ).FirstOrDefault();
        //                        //if existed: add quantity and rec user. Update
        //                        if (pt != null)
        //                        {
        //                            pt.RecQty += qty;
        //                            pt.RecBy = user;
        //                            _db.Update(item);
        //                            common.dataenum.Updated++;
        //                            await _db.SaveChangesAsync();
        //                        }
        //                        //if NOT exist: Create new
        //                        else
        //                        {
        //                            var newPt = new ItemMaster
        //                            {
        //                                ItemNo = itemNo,
        //                                IcId = icId,
        //                                LocCode = "",
        //                                PtCmt = note,
        //                                PtDateIn = delDate,
        //                                PtHold = 0,
        //                                PtLotNo = lot,
        //                                PtQty = 0,
        //                                Accepted = 0,
        //                                Qc = "",
        //                                RecBy = user,
        //                                RecQty = qty,
        //                                RefDate = refDate,
        //                                SupCode = batchRef,
        //                                RefNo = supRef

        //                            };
        //                            incomingList.ItemCount++;
        //                            await _db.AddAsync(newPt);
        //                            await _db.SaveChangesAsync();
        //                            newPt.ParentId = newPt.PtId;
        //                            _db.Update(newPt);
        //                            await _db.SaveChangesAsync();
        //                            common.dataenum.Inserted++;
        //                            common.dataenum.TotalRecord++;

        //                        }

        //                    }
        //                    else
        //                    {
        //                        if (string.IsNullOrEmpty(lot)
        //                            && string.IsNullOrEmpty(note)
        //                            && string.IsNullOrEmpty(supRef))
        //                        {
        //                            break;
        //                        }

        //                        common.dataenum.Errors++;
        //                        _db.Add(new UploadError
        //                        {
        //                            UploadId = common.dataenum.UploadId,
        //                            Error = " Line " + readingRow + ": Error with "
        //                            + (item == null ? "Item no." : "")
        //                            + (!checkQty ? "quantity" : "")

        //                            + ", Skipped line: " + readingRow
        //                        });
        //                        common.dataenum.TotalRecord++;
        //                        continue;
        //                    }



        //                }
        //                catch (Exception e)
        //                {
        //                    common.dataenum.Errors++;
        //                    _db.Add(new UploadError
        //                    {
        //                        UploadId = common.dataenum.UploadId,
        //                        Error = "Line " + readingRow + ": "
        //                       + e.Message + ";"
        //                    });
        //                }
        //            }
        //        }
        //    }

        //    _db.Add(common.dataenum);
        //    await _db.SaveChangesAsync();
        //    return common;
        //}
    }
}
