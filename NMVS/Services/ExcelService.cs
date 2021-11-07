using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using NMVS.Common;
using NMVS.Models;
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

        public ExcelService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CommonResponse<string>> GetshipperNote(int shpId, string user)
        {
            CommonResponse<string> common = new();
            ExcelDataHelper _eHelper = new();
            common.dataenum = @"D:\Temp download\";
            try
            {
                //Check icoming list exist
                var shipper = await _db.Shippers.FindAsync(shpId);
                if (shipper == null)
                {
                    common.message = "List not found";
                    common.status = 0;
                    return common;

                }

                var noteDet = (from d in _db.ShipperDets.Where(x => x.ShpId == shpId)
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
                               select new Spn01Vm
                               {
                                   ItemName = a.ItemName,
                                   Batch = pt.BatchNo,
                                   CustomerName = shto.CustName,
                                   PkgType = a.ItemPkg,
                                   PkgQty = a.ItemPkgQty,
                                   Qty = d.Quantity,
                                   Total = a.ItemPkgQty * d.Quantity

                               }).ToList();

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
                        sheet.Cells["B" + writingRow].Value = noteDet[i].CustomerName;
                        sheet.Cells["C" + writingRow].Value = noteDet[i].ItemName;
                        sheet.Cells["D" + writingRow].Value = noteDet[i].PkgQty;
                        sheet.Cells["E" + writingRow].Value = noteDet[i].PkgType;
                        sheet.Cells["F" + writingRow].Value = noteDet[i].Qty;
                        sheet.Cells["G" + writingRow].Value = noteDet[i].Total;
                        sheet.Cells["H" + writingRow].Value = noteDet[i].Batch;


                        ptIndex++;
                        if (writingRow >= 25)
                        {
                            writingRow = 10;
                            break;
                        }
                        writingRow++;
                    }
                }

                common.dataenum = @"D:\Temp download\" + common.message;
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
            common.dataenum = @"D:\Temp download\";
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
                var pt_mstr = (from pt in _db.ItemMasters
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

                common.dataenum = @"D:\Temp download\" + common.message;
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

        public async Task<CommonResponse<string>> GetIssueNoteSo(int shpId, string user)
        {
            CommonResponse<string> common = new();
            ExcelDataHelper _eHelper = new();
            common.dataenum = @"D:\Temp download\";
            try
            {
                //Check icoming list exist
                var shipper = await _db.Shippers.FindAsync(shpId);
                if (shipper == null)
                {
                    common.message = "List not found";
                    common.status = 0;
                    return common;

                }

                var noteDet = (from d in _db.ShipperDets.Where(x => x.ShpId == shpId)
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
                                   ShpId = shpId,
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

                if (!noteDet.Any())
                {
                    common.message = "No item in list";
                    common.status = 0;
                    return common;
                }




                var date = (DateTime)shipper.IssueConfirmedTime;
                var shipperInfo = shipper.ShpDesc + " - " + shipper.Driver + (string.IsNullOrEmpty(shipper.DrContact) ? "" : " - " + shipper.DrContact);
                common.message = user + "_Issue note_" + (date).ToString("yyyyMMdd") + ".xlsx";
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
                    sheet.Cells["A11"].Value += " " + noteDet[ptIndex].SoltToAddr;
                    sheet.Cells["A12"].Value += " " + noteDet[ptIndex].SoldToTax;
                    sheet.Cells["A14"].Value += " " + shipper.ShpDesc + "(" + shipper.Driver + ")" ;
                    sheet.Cells["F10"].Value += " " + noteDet[ptIndex].ShipToName;
                    sheet.Cells["F11"].Value += " " + noteDet[ptIndex].ShipToAddr;
                    sheet.Cells["F12"].Value += " " + noteDet[ptIndex].ShipToTax;

                    for (int i = ptIndex; i < noteDet.Count; i++)
                    {


                        sheet.Cells["B" + writingRow].Value = noteDet[i].ItemName;
                        sheet.Cells["C" + writingRow].Value = noteDet[i].ItemUnit;
                        sheet.Cells["D" + writingRow].Value = noteDet[i].PkgQty;
                        sheet.Cells["E" + writingRow].Value = noteDet[i].Quantity;
                        sheet.Cells["F" + writingRow].Value = noteDet[i].Quantity * noteDet[i].PkgQty;
                        sheet.Cells["G" + writingRow].Value = noteDet[i].RqId;
                        sheet.Cells["H" + writingRow].Value = noteDet[i].BatchNo;

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

                common.dataenum = @"D:\Temp download\" + common.message;
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
            common.dataenum = @"D:\Temp download\";
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


                        sheet.Cells["B" + writingRow].Value = noteDet[i].ItemName;
                        sheet.Cells["C" + writingRow].Value = noteDet[i].ItemUnit;
                        sheet.Cells["D" + writingRow].Value = noteDet[i].PkgQty;
                        sheet.Cells["E" + writingRow].Value = noteDet[i].Quantity;
                        sheet.Cells["F" + writingRow].Value = noteDet[i].Quantity * noteDet[i].PkgQty;

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

                common.dataenum = @"D:\Temp download\" + common.message;
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
    }
}
