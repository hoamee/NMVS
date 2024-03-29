﻿using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NMVS.Common;
using NMVS.Models;
using NMVS.Models.ConfigModels;
using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public class ExcelService : IExcelService
    {
        private readonly ApplicationDbContext _db;
        private readonly IOptions<TempPath> _config;

        public ExcelService(ApplicationDbContext db, IOptions<TempPath> config)
        {
            _db = db;
            _config = config;
        }

        public async Task<CommonResponse<string>> GetshipperNote(int shpId, string user)
        {
            CommonResponse<string> common = new();
            ExcelDataHelper _eHelper = new();
            common.dataenum = _config.Value.Path;
            try
            {
                Shipper shipper;
                List<IssueNoteVm> issueNotes;

                //shpId == 0 mean we are downloading from issue note page
                //issue note is defined by noteId

                shipper = await _db.Shippers.FindAsync(shpId);
                if (shipper == null)
                {
                    common.message = "List not found";
                    common.status = 0;
                    return common;

                }
                issueNotes = GetIssueNoteVmByShipper(shpId);
                List<In01Vm> noteDet = new();
                foreach (var item in issueNotes)
                {
                    var customer = _db.Customers.Find(item.ShipTo);
                    var issueLines = (from d in _db.SoIssueNoteDets.Where(x => x.InId == item.Id && x.InType == item.NoteType)
                                      join p in _db.ItemMasters on d.PtId equals p.PtId into pd
                                      from pt in pd.DefaultIfEmpty()
                                      join i in _db.ItemDatas on d.ItemNo equals i.ItemNo into all
                                      from a in all.DefaultIfEmpty()
                                      select new In01Vm
                                      {
                                          ItemName = a.ItemName,
                                          Quantity = d.Quantity,
                                          PkgQty = a.ItemPkgQty,
                                          ItemUnit = a.ItemUm,
                                          BatchNo = string.IsNullOrEmpty(pt.BatchNo) ? (string.IsNullOrEmpty(pt.RefNo) ? "" : pt.RefNo) : pt.BatchNo,
                                          PkgType = a.ItemPkg,
                                          Um = a.ItemUm,
                                          PackCount = d.PackCount,
                                          ShipToName = customer.CustName

                                      }).ToList();
                    noteDet = noteDet.Concat(issueLines).ToList();
                }

                if (!noteDet.Any())
                {
                    common.message = "No item in list";
                    common.status = 0;
                    return common;
                }

                var itemCount = noteDet.Count;
                var dataRows = 16;
                var date = (DateTime)shipper.IssueConfirmedTime;
                common.message = user + "_Shipper note_" + (date).ToString("yyyyMMdd") + ".xlsx";
                string templatePath = "xlsx/SPN01_Shipper note.xlsx";

                int pageCount = (itemCount + dataRows - 1) / dataRows;
                bool multiplePage = itemCount > dataRows;

                FileInfo fileInfo = new(templatePath);

                ExcelPackage excel = new(fileInfo);


                //Set header by incoming list's data
                var fst = excel.Workbook.Worksheets.FirstOrDefault(x => x.Name == "ORDER");
                fst.Cells["A5"].Value += " " + date.ToString("dd-MM-yyyy");
                fst.Cells["A6"].Value += " " + shipper.Driver + " (" + shipper.DrContact + ")";
                fst.Cells["C6"].Value += " " + shipper.ShpDesc;

                //End of header

                if (pageCount > 1)
                {
                    for (int i = 1; i < pageCount; i++)
                    {
                        excel.Workbook.Worksheets.Copy("ORDER", "ORDER(" + i + ")");

                    }
                }


                int ptIndex = 0;
                int writingRow = 10;
                int sheetIndex = 1;
                foreach (var sheet in excel.Workbook.Worksheets)
                {
                    if (pageCount > 1)
                    {
                        sheet.Cells["G7"].Value += sheetIndex + " of " + itemCount;
                        sheetIndex++;
                    }
                    for (int i = ptIndex; i < noteDet.Count; i++)
                    {
                        sheet.Cells["B" + writingRow].Value = noteDet[i].ShipToName;
                        sheet.Cells["C" + writingRow].Value = noteDet[i].ItemName;
                        sheet.Cells["D" + writingRow].Value = noteDet[i].PkgQty;
                        sheet.Cells["E" + writingRow].Value = noteDet[i].PkgType;
                        sheet.Cells["F" + writingRow].Value = noteDet[i].PackCount;
                        sheet.Cells["G" + writingRow].Value = noteDet[i].Quantity;
                        sheet.Cells["H" + writingRow].Value = noteDet[i].BatchNo;


                        ptIndex++;
                        if (writingRow >= 25)
                        {
                            writingRow = 10;
                            break;
                        }
                        writingRow++;
                    }
                }

                common.dataenum = _config.Value.Path + common.message;
                var outputInfo = new FileInfo(common.dataenum);
                // save changes
                excel.SaveAs(outputInfo);

                common.status = 1;
            }
            catch (Exception e)
            {
                common.message = e.ToString();
                common.status = -1;

            }
            return common;
        }

        public async Task<CommonResponse<string>> GetReceiptNote(int icId, string user)
        {
            CommonResponse<string> common = new();
            ExcelDataHelper _eHelper = new();
            common.dataenum = _config.Value.Path;
            try
            {
                //Check icoming list exist
                var incoming = await _db.IncomingLists.FindAsync(icId);
                if (incoming == null)
                {
                    common.message = "List not found";
                    common.status = 0;
                    return common;

                }

                var sup = await _db.Suppliers.FindAsync(incoming.SupCode);
                //Check any item master in list
                var pt_mstr = (from pt in _db.ItemMasters.Where(x => x.ParentId == x.PtId)
                               where pt.IcId == icId
                               join dt in _db.ItemDatas on pt.ItemNo equals dt.ItemNo into all
                               from a in all.DefaultIfEmpty()
                               select new ItemMasterVm
                               {
                                   Name = a.ItemName,
                                   Qty = pt.Accepted,
                                   Um = a.ItemUm,
                                   Ref = pt.RefNo,
                                   RefDate = pt.RefDate,
                                   Note = pt.PtCmt
                               }).ToList();
                if (!pt_mstr.Any())
                {
                    common.message = "No item in list";
                    common.status = 0;
                    return common;
                }

                var itemCount = pt_mstr.Count;
                var dataRows = 12;

                common.message = user + "_Receipt note" + incoming.DeliveryDate.ToString("yyyyMMdd") + ".xlsx";
                string templatePath = "xlsx/GRN01_Goods received note.xlsx";

                int pageCount = (itemCount + dataRows - 1) / dataRows;
                bool multiplePage = itemCount > dataRows;

                FileInfo fileInfo = new(templatePath);

                ExcelPackage excel = new(fileInfo);


                //Set header by incoming list's data
                var fst = excel.Workbook.Worksheets.FirstOrDefault(x => x.Name == "PNK");
                fst.Cells["A7"].Value += " " + sup.SupDesc;
                if (!string.IsNullOrEmpty(incoming.Po))
                {
                    fst.Cells["A8"].Value += " " + incoming.Po;
                }

                if (incoming.PoDate != null)
                {
                    fst.Cells["C8"].Value += " " + ((DateTime)incoming.PoDate).ToString("dd-MM-yyy");
                }
                fst.Cells["A9"].Value += " " + incoming.Driver + " - " + incoming.Vehicle;

                fst.Cells["G9"].Value += " " + incoming.DeliveryDate.ToString("dd-MM-yyy");
                //End of header

                if (pageCount > 1)
                {
                    for (int i = 1; i < pageCount; i++)
                    {
                        excel.Workbook.Worksheets.Copy("PNK", "PNK(" + i + ")");

                    }
                }


                int ptIndex = 0;
                int writingRow = 13;
                int sheetIndex = 1;
                foreach (var sheet in excel.Workbook.Worksheets)
                {
                    if (pageCount > 1)
                    {
                        sheet.Cells["I7"].Value += "Page" + sheetIndex + "/" + pageCount;
                        sheet.Cells["G7"].Value += incoming.IcId.ToString();
                        sheetIndex++;
                    }
                    else
                    {
                        sheet.Cells["G7"].Value += sheetIndex.ToString();
                    }
                    for (int i = ptIndex; i < pt_mstr.Count; i++)
                    {
                        sheet.Cells["B" + writingRow].Value = pt_mstr[i].Name;
                        sheet.Cells["C" + writingRow].Value = pt_mstr[i].Um;
                        sheet.Cells["E" + writingRow].Value = pt_mstr[i].Qty;
                        if (pt_mstr[i].RefDate != null)
                        {
                            sheet.Cells["H" + writingRow].Value = pt_mstr[i].RefDate;
                        }
                        sheet.Cells["G" + writingRow].Value = string.IsNullOrEmpty(pt_mstr[i].Ref) ? "-" : pt_mstr[i].Ref;
                        sheet.Cells["I" + writingRow].Value = string.IsNullOrEmpty(pt_mstr[i].Note) ? "" : pt_mstr[i].Note;


                        ptIndex++;
                        if (writingRow >= 24)
                        {
                            writingRow = 13;
                            break;
                        }
                        writingRow++;
                    }
                }

                common.dataenum = _config.Value.Path + common.message;
                var outputInfo = new FileInfo(common.dataenum);
                // save changes
                excel.SaveAs(outputInfo);

                common.status = 1;
            }
            catch (Exception e)
            {
                common.message = e.ToString();
                common.status = -1;

            }



            return common;
        }

        public async Task<CommonResponse<string>> GetIssueNoteSo(int shpId, string user, int noteId, int sot)
        {
            CommonResponse<string> common = new();
            ExcelDataHelper _eHelper = new();
            common.dataenum = _config.Value.Path;
            try
            {

                Shipper shipper;
                List<IssueNoteVm> issueNotes;

                //shpId == 0 mean we are downloading from issue note page
                //issue note is defined by noteId

                if (shpId == 0)
                {
                    issueNotes = GetIssueNoteVmById(noteId, sot);
                    shipper = await _db.Shippers.FindAsync(issueNotes.First().ShipperId);
                    if (shipper == null)
                    {
                        common.message = "List not found";
                        common.status = 0;
                        return common;

                    }
                }
                else
                {
                    shipper = await _db.Shippers.FindAsync(shpId);
                    if (shipper == null)
                    {
                        common.message = "List not found";
                        common.status = 0;
                        return common;

                    }
                    issueNotes = GetIssueNoteVmByShipper(shpId);
                }

                //Get issued time
                var date = (DateTime)shipper.IssueConfirmedTime;

                //Init download file name
                common.message = user + "_Issue note_" + (date).ToString("yyyyMMdd") + ".xlsx";

                //Get template
                string templatePath = "xlsx/IN01_Issue note.xlsx";
                FileInfo fileInfo = new(templatePath);
                ExcelPackage excel = new(fileInfo);

                int createdPages = 1;

                for (int i = 0; i < issueNotes.Count; i++)
                {
                    if (createdPages != 1)
                    {
                        excel.Workbook.Worksheets.Copy("PXK", "PXK(" + createdPages + ")");
                    }
                    createdPages++;

                }

                int ptIndex = 0;
                int writingRow = 21;

                foreach (var sheet in excel.Workbook.Worksheets)
                {
                    var note = issueNotes[ptIndex];
                    var soto = _db.Customers.Find(note.SoldTo);
                    var shipTo = _db.Customers.Find(note.ShipTo);
                    string idNbr = "";
                    if (note.NoteType == 0)
                    {
                        idNbr = "SO";
                    }
                    else if (note.NoteType == 1)
                    {
                        idNbr = "WR";
                    }
                    else
                    {
                        idNbr = "WT";
                    }

                    //Set header
                    sheet.Cells["A8"].Value += " " + idNbr + note.Id;
                    sheet.Cells["G8"].Value += " " + date;
                    sheet.Cells["A10"].Value += " " + soto.CustName;
                    sheet.Cells["A11"].Value += " " + soto.Addr + (!string.IsNullOrEmpty(soto.City) ? ", " + soto.City : "") + (!string.IsNullOrEmpty(soto.Ctry) ? ", " + soto.Ctry : "");
                    sheet.Cells["A12"].Value += " " + soto.TaxCode;
                    sheet.Cells["A14"].Value += " " + shipper.ShpDesc + "(" + shipper.Driver + ")";
                    sheet.Cells["F10"].Value += " " + shipTo.CustName;
                    sheet.Cells["F11"].Value += " " + shipTo.Addr + (!string.IsNullOrEmpty(shipTo.City) ? ", " + shipTo.City : "") + (!string.IsNullOrEmpty(shipTo.Ctry) ? ", " + shipTo.Ctry : ""); ;
                    sheet.Cells["F12"].Value += " " + shipTo.TaxCode;

                    //Get note line
                    var issueLines = (from d in _db.SoIssueNoteDets.Where(x => x.InId == note.Id && x.InType == note.NoteType)
                                      join p in _db.ItemMasters on d.PtId equals p.PtId into pd
                                      from pt in pd.DefaultIfEmpty()
                                      join i in _db.ItemDatas on d.ItemNo equals i.ItemNo into all
                                      from a in all.DefaultIfEmpty()
                                      select new In01Vm
                                      {
                                          ItemName = a.ItemName,
                                          Quantity = d.Quantity,
                                          PkgQty = a.ItemPkgQty,
                                          ItemUnit = a.ItemUm,
                                          BatchNo = string.IsNullOrEmpty(pt.BatchNo) ? (string.IsNullOrEmpty(pt.RefNo) ? "" : pt.RefNo) : pt.BatchNo,
                                          DetId = d.PackCount

                                      }).ToList();

                    for (int i = 0; i < issueLines.Count; i++)
                    {

                        sheet.Cells["B" + writingRow].Value = issueLines[i].ItemName;
                        sheet.Cells["C" + writingRow].Value = issueLines[i].ItemUnit;
                        sheet.Cells["D" + writingRow].Value = issueLines[i].PkgQty;
                        sheet.Cells["E" + writingRow].Value = issueLines[i].DetId;
                        sheet.Cells["F" + writingRow].Value = issueLines[i].Quantity;
                        sheet.Cells["G" + writingRow].Value = note.SoNbr;
                        sheet.Cells["H" + writingRow].Value = issueLines[i].BatchNo;

                        writingRow++;
                        if (writingRow > 27)
                        {
                            writingRow = 21;
                        }
                    }

                    sheet.Name = idNbr + note.Id;
                    ptIndex++;
                    writingRow = 21;
                }

                common.dataenum = _config.Value.Path + common.message;
                var outputInfo = new FileInfo(common.dataenum);
                // save changes
                excel.SaveAs(outputInfo);

                common.status = 1;

            }
            catch (Exception e)
            {
                common.message = e.ToString();
                common.status = -1;

            }



            return common;
        }

        public async Task<CommonResponse<string>> GetIssueNoteMFG(int shpId, string user)
        {
            CommonResponse<string> common = new();
            ExcelDataHelper _eHelper = new();
            common.dataenum = _config.Value.Path;
            try
            {
                //Check icoming list exist
                var iNote = await _db.MfgIssueNotes.FindAsync(shpId);
                if (iNote == null)
                {
                    common.message = "List not found";
                    common.status = 0;
                    return common;

                }

                var noteDet = (from d in _db.IssueNoteDets.Where(x => x.IsNId == shpId)
                               join i in _db.ItemDatas on d.ItemNo equals i.ItemNo into all
                               from a in all.DefaultIfEmpty()

                               select new In01Vm
                               {
                                   ItemName = a.ItemName,
                                   ItemNo = a.ItemNo,
                                   Quantity = d.Quantity,
                                   RqId = iNote.RqId,
                                   ShpId = shpId,
                                   SoldToName = "Aica",
                                   ShipToName = "Aica",
                                   PkgType = a.ItemPkg,
                                   PkgQty = a.ItemPkgQty,
                                   ItemUnit = a.ItemUm

                               }).OrderBy(x => x.RqId).ToList();

                if (!noteDet.Any())
                {
                    common.message = "No item in list";
                    common.status = 0;
                    return common;
                }




                var date = (DateTime)iNote.IssuedOn;
                common.message = user + "_MFG Issue note_" + (date).ToString("yyyyMMdd") + ".xlsx";
                string templatePath = "xlsx/IN01_Issue note.xlsx";
                FileInfo fileInfo = new(templatePath);
                ExcelPackage excel = new(fileInfo);


                var listSo = noteDet.Select(x => x.RqId).Distinct();

                int createdPages = 1;
                var dataRows = 7;

                foreach (var so in listSo)
                {
                    var itemCount = noteDet.Where(x => x.RqId == so).Count();
                    var detCount = ((itemCount + dataRows - 1) / dataRows);

                    for (int i = 0; i < detCount; i++)
                    {
                        if (createdPages != 1)
                        {

                            excel.Workbook.Worksheets.Copy("PXK", "PXK(" + createdPages + ")");
                        }
                        createdPages++;
                    }
                }


                string rqId = "";
                int ptIndex = 0;
                int writingRow = 21;
                int sheetDuplicate = 1;

                foreach (var sheet in excel.Workbook.Worksheets)
                {
                    if (ptIndex == 0)
                    {
                        rqId = noteDet[0].RqId;
                    }
                    if (sheet.Name == rqId)
                    {
                        sheet.Name = rqId + "(" + sheetDuplicate + ")";
                        sheetDuplicate++;
                    }
                    else
                    {
                        sheet.Name = rqId;
                        sheetDuplicate = 1;
                    }

                    sheet.Cells["A8"].Value += " " + noteDet[ptIndex].RqId;
                    sheet.Cells["G8"].Value += " " + date;
                    sheet.Cells["A10"].Value += " " + noteDet[ptIndex].SoldToName;
                    sheet.Cells["F10"].Value += " " + noteDet[ptIndex].ShipToName;

                    for (int i = ptIndex; i < noteDet.Count; i++)
                    {
                        var packCount = Convert.ToInt32(Math.Ceiling(noteDet[i].Quantity / noteDet[i].PkgQty));


                        sheet.Cells["B" + writingRow].Value = noteDet[i].ItemName;
                        sheet.Cells["C" + writingRow].Value = noteDet[i].ItemUnit;
                        sheet.Cells["D" + writingRow].Value = noteDet[i].PkgQty;
                        sheet.Cells["E" + writingRow].Value = packCount;
                        sheet.Cells["F" + writingRow].Value = noteDet[i].Quantity;

                        ptIndex++;

                        if (i + 1 < noteDet.Count)
                        {
                            var nextDet = noteDet[ptIndex];
                            if (nextDet.RqId != rqId)
                            {
                                rqId = nextDet.RqId;
                                writingRow = 21;
                                break;
                            }
                            if (writingRow >= 27)
                            {
                                writingRow = 21;
                                break;
                            }
                            writingRow++;
                        }

                    }

                }

                common.dataenum = _config.Value.Path + common.message;
                var outputInfo = new FileInfo(common.dataenum);
                // save changes
                excel.SaveAs(outputInfo);

                common.status = 1;
            }
            catch (Exception e)
            {
                common.message = e.ToString();
                common.status = -1;

            }



            return common;
        }

        public async Task<CommonResponse<string>> GenerateIssueNoteSo(int shpId, string user, int noteId)
        {
            CommonResponse<string> common = new();
            ExcelDataHelper _eHelper = new();
            common.dataenum = _config.Value.Path;
            try
            {

                Shipper shipper;
                List<SoIssueNote> issueNotes;

                //shpId == 0 mean we are downloading from issue note page
                //issue note is defined by noteId

                if (shpId == 0)
                {
                    issueNotes = _db.SoIssueNotes.Where(x => x.InId == noteId).ToList();
                    shipper = await _db.Shippers.FindAsync(issueNotes.First().Shipper);
                    if (shipper == null)
                    {
                        common.message = "List not found";
                        common.status = 0;
                        return common;

                    }
                }
                else
                {
                    shipper = await _db.Shippers.FindAsync(shpId);
                    if (shipper == null)
                    {
                        common.message = "List not found";
                        common.status = 0;
                        return common;

                    }
                    issueNotes = _db.SoIssueNotes.Where(x => x.Shipper == shpId).ToList();
                }

                //Get issued time
                var date = (DateTime)shipper.IssueConfirmedTime;

                //Init download file name
                common.message = user + "_Issue note_" + (date).ToString("yyyyMMdd") + ".xlsx";

                //Get template
                string templatePath = "xlsx/IN01_Issue note.xlsx";
                FileInfo fileInfo = new(templatePath);
                ExcelPackage excel = new(fileInfo);

                int createdPages = 1;


                //add sheet when row count > 7
                for (int i = 0; i < issueNotes.Count; i++)
                {
                    if (createdPages != 1)
                    {
                        excel.Workbook.Worksheets.Copy("PXK", "PXK(" + createdPages + ")");
                    }
                    createdPages++;

                }

                int ptIndex = 0;
                int writingRow = 21;

                foreach (var sheet in excel.Workbook.Worksheets)
                {
                    var note = issueNotes[ptIndex];
                    var soto = _db.Customers.Find(note.SoldTo);
                    var shipTo = _db.Customers.Find(note.ShipTo);


                    //Set header
                    sheet.Cells["A8"].Value += " " + note.InId;
                    sheet.Cells["G8"].Value += " " + date;
                    sheet.Cells["A10"].Value += " " + soto.CustName;
                    sheet.Cells["A11"].Value += " " + soto.Addr + (!string.IsNullOrEmpty(soto.City) ? ", " + soto.City : "") + (!string.IsNullOrEmpty(soto.Ctry) ? ", " + soto.Ctry : "");
                    sheet.Cells["A12"].Value += " " + soto.TaxCode;
                    sheet.Cells["A14"].Value += " " + shipper.ShpDesc + "(" + shipper.Driver + ")";
                    sheet.Cells["F10"].Value += " " + shipTo.CustName;
                    sheet.Cells["F11"].Value += " " + shipTo.Addr + (!string.IsNullOrEmpty(shipTo.City) ? ", " + shipTo.City : "") + (!string.IsNullOrEmpty(shipTo.Ctry) ? ", " + shipTo.Ctry : ""); ;
                    sheet.Cells["F12"].Value += " " + shipTo.TaxCode;
                    var issueLines = (from d in _db.SoIssueNoteDets.Where(x => x.InId == note.InId)
                                      join p in _db.ItemMasters on d.PtId equals p.PtId into pd
                                      from pt in pd.DefaultIfEmpty()
                                      join i in _db.ItemDatas on d.ItemNo equals i.ItemNo into all
                                      from a in all.DefaultIfEmpty()
                                      select new In01Vm
                                      {
                                          ItemName = a.ItemName,
                                          Quantity = d.Quantity,
                                          PkgQty = a.ItemPkgQty,
                                          ItemUnit = a.ItemUm,
                                          BatchNo = pt.BatchNo,
                                          DetId = d.PackCount

                                      }).ToList();

                    for (int i = 0; i < issueLines.Count; i++)
                    {

                        sheet.Cells["B" + writingRow].Value = issueLines[i].ItemName;
                        sheet.Cells["C" + writingRow].Value = issueLines[i].ItemUnit;
                        sheet.Cells["D" + writingRow].Value = issueLines[i].PkgQty;
                        sheet.Cells["E" + writingRow].Value = issueLines[i].DetId;
                        sheet.Cells["F" + writingRow].Value = issueLines[i].Quantity;
                        sheet.Cells["G" + writingRow].Value = note.SoNbr;
                        sheet.Cells["H" + writingRow].Value = issueLines[i].BatchNo;

                        writingRow++;
                        if (writingRow > 27)
                        {
                            writingRow = 21;
                        }
                    }

                    sheet.Name = "PXK " + note.InId;
                }

                common.dataenum = _config.Value.Path + common.message;
                var outputInfo = new FileInfo(common.dataenum);
                // save changes
                excel.SaveAs(outputInfo);

                common.status = 1;
            }
            catch (Exception e)
            {
                common.message = e.ToString();
                common.status = -1;

            }



            return common;
        }

        private List<IssueNoteVm> GetIssueNoteVmById(int noteId, int sot)
        {
            if (sot == 0)
            {
                return (from i in _db.SoIssueNotes.Where(x => x.InId == noteId)
                        select new IssueNoteVm
                        {
                            Id = i.InId,
                            SoNbr = i.SoNbr,
                            SoldTo = i.SoldTo,
                            ShipTo = i.ShipTo,
                            IssuedOn = i.IssuedOn,
                            IssuedBy = i.IssuedBy,
                            ShipperId = i.Shipper,
                            NoteType = 0
                        }).ToList();
            }
            else if (sot == 1)
            {
                return (from i in _db.WrIssueNotes.Where(x => x.InId == noteId)
                        select new IssueNoteVm
                        {
                            Id = i.InId,
                            SoNbr = i.SoNbr,
                            SoldTo = i.SoldTo,
                            ShipTo = i.ShipTo,
                            IssuedOn = i.IssuedOn,
                            IssuedBy = i.IssuedBy,
                            ShipperId = i.Shipper,
                            NoteType = 1
                        }).ToList();
            }
            else
            {
                return (from i in _db.WtIssueNotes.Where(x => x.InId == noteId)
                        select new IssueNoteVm
                        {
                            Id = i.InId,
                            SoNbr = i.SoNbr,
                            SoldTo = i.SoldTo,
                            ShipTo = i.ShipTo,
                            IssuedOn = i.IssuedOn,
                            IssuedBy = i.IssuedBy,
                            ShipperId = i.Shipper,
                            NoteType = 2
                        }).ToList();

            }
        }

        private List<IssueNoteVm> GetIssueNoteVmByShipper(int shpId)
        {
            var issueNotes1 = (from i in _db.SoIssueNotes.Where(x => x.Shipper == shpId)
                               select new IssueNoteVm
                               {
                                   Id = i.InId,
                                   SoNbr = i.SoNbr,
                                   SoldTo = i.SoldTo,
                                   ShipTo = i.ShipTo,
                                   IssuedOn = i.IssuedOn,
                                   IssuedBy = i.IssuedBy,
                                   ShipperId = i.Shipper,
                                   NoteType = 0
                               }).ToList();

            var issueNotes2 = (from i in _db.WrIssueNotes.Where(x => x.Shipper == shpId)
                               select new IssueNoteVm
                               {
                                   Id = i.InId,
                                   SoNbr = i.SoNbr,
                                   SoldTo = i.SoldTo,
                                   ShipTo = i.ShipTo,
                                   IssuedOn = i.IssuedOn,
                                   IssuedBy = i.IssuedBy,
                                   ShipperId = i.Shipper,
                                   NoteType = 1
                               }).ToList();

            var issueNotes3 = (from i in _db.WtIssueNotes.Where(x => x.Shipper == shpId)
                               select new IssueNoteVm
                               {
                                   Id = i.InId,
                                   SoNbr = i.SoNbr,
                                   SoldTo = i.SoldTo,
                                   ShipTo = i.ShipTo,
                                   IssuedOn = i.IssuedOn,
                                   IssuedBy = i.IssuedBy,
                                   ShipperId = i.Shipper,
                                   NoteType = 2
                               }).ToList();

            return issueNotes1.Concat(issueNotes2).Concat(issueNotes3).ToList();
        }


        public async Task<CommonResponse<string>> GetPreShipperNote(int shpId, string user)
        {
            CommonResponse<string> common = new();
            ExcelDataHelper _eHelper = new();
            common.dataenum = "";
            var issueList = await _db.IssueOrders.Where(x => x.ToVehicle == shpId).ToListAsync();
            var shp = await _db.Shippers.FindAsync(shpId);


            //excel config
            var date = issueList.Select(x => x.MovementTime).FirstOrDefault();
            common.message = user + "_Shipper note_" + (date).ToString("yyyyMMdd") + ".xlsx";
            string templatePath = "xlsx/SPN01_Shipper note.xlsx";
            FileInfo fileInfo = new(templatePath);
            ExcelPackage excel = new(fileInfo);

            try
            {
                if (issueList.Count == 0)
                {
                    common.message = "No item in list";
                    common.status = 0;
                }


                ExcelWorksheet temp = excel.Workbook.Worksheets.Copy("ORDER", shp.ShpDesc);


                temp.Cells["A5"].Value += " " + date.ToString("dd-MM-yyyy");
                temp.Cells["A6"].Value += " " + shp.Driver + " (" + shp.DrContact + ")";
                temp.Cells["C6"].Value += " " + shp.ShpDesc;

                int writingRow = 10;
                for (int i = 0; i < issueList.Count; i++)
                {
                    var rqId = issueList[i].RqID;
                    var so = await _db.SalesOrders.FindAsync(rqId);
                    var shipTo = await _db.Customers.FindAsync(so.ShipTo);
                    var invRequest = await _db.InvRequests.FindAsync(rqId);                    
                    var itemData = _db.ItemDatas.Find(issueList[i].ItemNo);
                    var pt = _db.ItemMasters.Find(issueList[i].PtId);
                    string batchNo = string.IsNullOrEmpty(pt.BatchNo) ? (string.IsNullOrEmpty(pt.RefNo) ? "" : pt.RefNo) : pt.BatchNo;

                    var packCount = Convert.ToInt32(Math.Floor(issueList[i].ExpOrdQty / itemData.ItemPkgQty));
                    double remainder = issueList[i].ExpOrdQty % itemData.ItemPkgQty;


                    if (packCount > 0)
                    {
                        temp.Cells["B" + writingRow].Value = shipTo.CustName;
                        temp.Cells["C" + writingRow].Value = itemData.ItemName;
                        temp.Cells["D" + writingRow].Value = itemData.ItemPkgQty;
                        temp.Cells["E" + writingRow].Value = itemData.ItemPkg;
                        temp.Cells["F" + writingRow].Value = packCount;
                        temp.Cells["G" + writingRow].Value = itemData.ItemPkgQty * packCount;
                        temp.Cells["H" + writingRow].Value = batchNo;
                    }

                    if (remainder > 0)
                    {
                        if (packCount > 0)
                        {
                            writingRow++;
                        }
                        temp.Cells["B" + writingRow].Value = shipTo.CustName;
                        temp.Cells["C" + writingRow].Value = itemData.ItemName;
                        temp.Cells["D" + writingRow].Value = itemData.ItemPkgQty;
                        temp.Cells["E" + writingRow].Value = itemData.ItemPkg;
                        temp.Cells["F" + writingRow].Value = 1;
                        temp.Cells["G" + writingRow].Value = remainder;
                        temp.Cells["H" + writingRow].Value = batchNo;
                    }

                    writingRow++;
                }



            }
            catch (Exception e)
            {
                common.message = e.ToString();
                common.status = -1;
                await Common.MonitoringService.SendErrorMessage(e.ToString());
                return common;

            }


            var fst = excel.Workbook.Worksheets.FirstOrDefault(x => x.Name == "ORDER");
            excel.Workbook.Worksheets.Delete(fst);
            //remove first (template) sheet
            common.dataenum = common.message;
            var outputInfo = new FileInfo(common.dataenum);
            // save changes
            excel.SaveAs(outputInfo);

            common.status = 1;
            return common;
        }
    }
}
