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
    public class IncomingService : IIncomingService
    {
        private readonly ApplicationDbContext _db;

        public IncomingService(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<IncomingListVm> BrowseIncomingList(bool closed)
        {
            if (closed)
            {
                var model = (from ic in _db.IncomingLists.Where(x => x.Closed == closed)
                             join sp in _db.Suppliers
                             on ic.SupCode equals sp.SupCode into all
                             from a in all.DefaultIfEmpty()
                             select new IncomingListVm
                             {
                                 IcId = ic.IcId,
                                 DeliveryDate = ic.DeliveryDate,
                                 Driver = ic.Driver,
                                 IsWarranty = ic.IsWarranty,
                                 Vehicle = ic.Vehicle,
                                 Po = ic.Po,
                                 PoDate = ic.PoDate,
                                 Supplier = a.SupDesc,
                                 Checked = ic.Checked,
                                 ItemCount = ic.ItemCount,
                                 Closed = ic.Closed,
                                 LastModified = ic.LastModifiedBy
                             }).ToList();


                return model;
            }
            else
            {
                var model = (from ic in _db.IncomingLists
                             join sp in _db.Suppliers
                             on ic.SupCode equals sp.SupCode into all
                             from a in all.DefaultIfEmpty()
                             select new IncomingListVm
                             {
                                 IcId = ic.IcId,
                                 DeliveryDate = ic.DeliveryDate,
                                 Driver = ic.Driver,
                                 IsWarranty = ic.IsWarranty,
                                 Vehicle = ic.Vehicle,
                                 Po = ic.Po,
                                 PoDate = ic.PoDate,
                                 Supplier = a.SupDesc,
                                 Checked = ic.Checked,
                                 ItemCount = ic.ItemCount,
                                 Closed = ic.Closed,
                                 LastModified = ic.LastModifiedBy
                             }).ToList();


                return model;
            }
        }

        public IcmListVm GetListDetail(int? id)
        {


            var icm = (from ic in _db.IncomingLists.Where(x => x.IcId == id)
                       join sp in _db.Suppliers
                       on ic.SupCode equals sp.SupCode
                       select new IncomingListVm
                       {
                           IcId = ic.IcId,
                           DeliveryDate = ic.DeliveryDate,
                           Driver = ic.Driver,
                           IsWarranty = ic.IsWarranty,
                           Vehicle = ic.Vehicle,
                           Po = ic.Po,
                           PoDate = ic.PoDate,
                           Supplier = sp.SupDesc,
                           Checked = ic.Checked,
                           ItemCount = ic.ItemCount,
                           Closed = ic.Closed,
                           LastModified = ic.LastModifiedBy
                       }).FirstOrDefault();

            var ptMstr = (from pt in _db.ItemMasters.Where(x => x.IcId == id && x.ParentId == x.PtId)
                          join data in _db.ItemDatas on pt.ItemNo equals data.ItemNo
                          into all
                          from a in all.DefaultIfEmpty()
                          select new PtVm
                          {
                              PtId = pt.PtId,
                              ItemNo = pt.ItemNo,
                              AcceptQty = pt.Accepted,
                              CheckedBy = pt.Qc,
                              IsChecked = icm.Closed ? (!string.IsNullOrEmpty(pt.Qc)) : null,
                              ItemName = a.ItemName,
                              RcvQty = pt.RecQty,
                              Note = pt.PtCmt,
                              Ref = pt.RefNo,
                              RefDate = pt.RefDate
                          }).ToList();
            IcmListVm icmList = new()
            {
                Icm = icm,
                Pt = ptMstr
            };

            return icmList;
        }

        public List<TypeVm> GetSupplier()
        {
            var model = (from sup in _db.Suppliers
                         where sup.Active == true
                         select new TypeVm
                         {
                             Code = sup.SupCode,
                             Desc = sup.SupDesc
                         }).ToList();

            return model;

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
                UploadFunction = "Incoming list upload"

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

                int readingRow = 12;

                //check header
                string supCode, po = "", vehicle = "", driver = "", sWarr = "";
                DateTime poDate = new();
                var delDate = DateTime.Now.Date;
                bool headerCorrect = true;

                headerCorrect = _eHelper.VefiryHeader(wsPart, wbPart, "A3", "A4", "A5", "A6", "A7", "A8", "A9", "A11", "B11", "C11",
                    "Supplier ID", "PO No.", "PO Date", "Vehicle", "Driver", "Delivery Date", "Warranty return", "No.", "Item code", "Item name");


                supCode = _eHelper.GetCellValue(wsPart, wbPart, "C3");
                var supplier = await _db.Suppliers.FindAsync(supCode);
                //if can not find supplier code, break
                if (string.IsNullOrEmpty(supCode) || !headerCorrect || supplier == null)
                {
                    if (string.IsNullOrEmpty(supCode))
                    {
                        common.message += "Supplier code is not found! ";
                    }
                    else if (supplier == null)
                    {
                        common.message += "Supplier code not exist! ";
                    }
                    if (!headerCorrect)
                    {
                        common.message += "File header is incorrect! Please check your file";
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
                    po = _eHelper.GetCellValue(wsPart, wbPart, "C4");
                    vehicle = _eHelper.GetCellValue(wsPart, wbPart, "C6");
                    driver = _eHelper.GetCellValue(wsPart, wbPart, "C7");
                    sWarr = _eHelper.GetCellValue(wsPart, wbPart, "C9");



                    poDate = DateTime.FromOADate(double.Parse(_eHelper.GetCellValue(wsPart, wbPart, "C5")));
                    delDate = DateTime.FromOADate(double.Parse(_eHelper.GetCellValue(wsPart, wbPart, "C8")));

                    var sWarrIsEmpty = string.IsNullOrEmpty(sWarr) || (sWarr.ToUpper(new CultureInfo("en-US", false)) != "YES" && sWarr.ToUpper(new CultureInfo("en-US", false)) != "NO");

                    if (string.IsNullOrEmpty(vehicle)
                        || string.IsNullOrEmpty(driver)
                        || sWarrIsEmpty)
                    {
                        common.dataenum.Errors++;
                        _db.Add(new UploadError
                        {
                            UploadId = common.dataenum.UploadId,
                            Error =
                            (string.IsNullOrEmpty(vehicle) ? " Vehicle info not found; " : "")
                            + (string.IsNullOrEmpty(driver) ? " Driver name not found; " : "")
                            + (sWarrIsEmpty ? " Warranty return not found; '" + sWarr.ToUpper(new CultureInfo("en-US", false)) + "'" : ""
                            )
                        });

                        headerCorrect = false;
                    }
                }

                if (headerCorrect)
                {
                    sWarr = sWarr.ToUpper();
                    IncomingList incomingList = new()
                    {
                        SupCode = supCode,
                        DeliveryDate = delDate,
                        Checked = 0,
                        Closed = false,
                        Driver = driver,
                        IsWarranty = sWarr.ToUpper().Equals("YES"),
                        ItemCount = 0,
                        LastModifiedBy = user,
                        Po = po,
                        Vehicle = vehicle,
                        PoDate = poDate

                    };
                    await _db.AddAsync(incomingList);
                    await _db.SaveChangesAsync();
                    int icId = incomingList.IcId;

                    //Read data from table
                    while (headerCorrect)
                    {
                        readingRow++;
                        string lot, supRef, note;
                        DateTime refDate;
                        string itemNo = _eHelper.GetCellValue(wsPart, wbPart, "B" + readingRow).Trim();
                        //Check Supplier exist
                        ItemData item = await _db.ItemDatas.FindAsync(itemNo);
                        var checkQty = double.TryParse(_eHelper.GetCellValue(wsPart, wbPart, "E" + readingRow), out double qty);
                        var err = itemNo;
                        incomingList = _db.IncomingLists.Find(icId);

                        try
                        {
                            lot = _eHelper.GetCellValue(wsPart, wbPart, "D" + readingRow);
                            note = _eHelper.GetCellValue(wsPart, wbPart, "H" + readingRow);
                            supRef = _eHelper.GetCellValue(wsPart, wbPart, "F" + readingRow);
                            var checkDate = DateTime.TryParse(_eHelper.GetCellValue(wsPart, wbPart, "G" + readingRow), out refDate);
                            if (item != null && checkQty)
                            {

                                var pt = _db.ItemMasters
                                    .Where(x => x.ItemNo == item.ItemNo
                                        && x.IcId == icId
                                        && x.PtLotNo == lot
                                        && x.RefDate == refDate
                                        && x.RefNo == supRef
                                        && x.PtCmt == note
                                    ).FirstOrDefault();
                                //if existed: add quantity and rec user. Update
                                if (pt != null)
                                {
                                    pt.RecQty += qty;
                                    pt.RecBy = user;
                                    _db.Update(item);
                                    common.dataenum.Updated++;
                                    await _db.SaveChangesAsync();
                                }
                                //if NOT exist: Create new
                                else
                                {
                                    var newPt = new ItemMaster
                                    {
                                        ItemNo = itemNo,
                                        IcId = icId,
                                        LocCode = "",
                                        PtCmt = note,
                                        PtDateIn = delDate,
                                        PtHold = 0,
                                        PtLotNo = lot,
                                        PtQty = 0,
                                        Accepted = 0,
                                        Qc = "",
                                        RecBy = user,
                                        RecQty = qty,
                                        RefDate = refDate,
                                        SupCode = supCode,
                                        RefNo = supRef

                                    };
                                    incomingList.ItemCount++;
                                    await _db.AddAsync(newPt);
                                    await _db.SaveChangesAsync();
                                    newPt.ParentId = newPt.PtId;
                                    _db.Update(newPt);
                                    await _db.SaveChangesAsync();
                                    common.dataenum.Inserted++;
                                    common.dataenum.TotalRecord++;

                                }

                            }
                            else
                            {
                                if (string.IsNullOrEmpty(lot)
                                    && string.IsNullOrEmpty(note)
                                    && string.IsNullOrEmpty(supRef))
                                {
                                    break;
                                }

                                common.dataenum.Errors++;
                                _db.Add(new UploadError
                                {
                                    UploadId = common.dataenum.UploadId,
                                    Error = " Line " + readingRow + ": Error with " 
                                    + (item == null ? "Item no." : "")
                                    + (!checkQty ? "quantity" : "")

                                    + ", Skipped line: " + readingRow
                                });
                                common.dataenum.TotalRecord++;
                                continue;
                            }


                            
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

        public async Task<CommonResponse<UploadReport>> ImportWarranty(string filepath, string fileName, string user)
        {
            CommonResponse<UploadReport> common = new();
            common.dataenum = new()
            {
                FileName = fileName,
                UploadBy = user,
                UploadTime = DateTime.Now,
                UploadId = user + DateTime.Now.ToString("yyyyMMddHHmmss"),
                UploadFunction = "Incoming list upload"

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

                int readingRow = 12;

                //check header
                string supCode, po = "", vehicle = "", driver = "", sWarr = "";
                DateTime poDate = new();
                var delDate = DateTime.Now.Date;
                bool headerCorrect = true;

                headerCorrect = _eHelper.VefiryHeader(wsPart, wbPart, "A3", "A4", "A5", "A6", "A7", "A8", "A9", "A11", "B11", "C11",
                    "Supplier ID", "PO No.", "PO Date", "Vehicle", "Driver", "Delivery Date", "Warranty return", "No.", "Item code", "Item name");


                supCode = _eHelper.GetCellValue(wsPart, wbPart, "C3");
                var supplier = await _db.Suppliers.FindAsync(supCode);
                //if can not find supplier code, break
                if (string.IsNullOrEmpty(supCode) || !headerCorrect || supplier == null)
                {
                    if (string.IsNullOrEmpty(supCode))
                    {
                        common.message += "Supplier code is not found! ";
                    }
                    else if (supplier == null)
                    {
                        common.message += "Supplier code not exist! ";
                    }
                    if (!headerCorrect)
                    {
                        common.message += "File header is incorrect! Please check your file";
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
                    po = _eHelper.GetCellValue(wsPart, wbPart, "C4");
                    vehicle = _eHelper.GetCellValue(wsPart, wbPart, "C6");
                    driver = _eHelper.GetCellValue(wsPart, wbPart, "C7");
                    sWarr = _eHelper.GetCellValue(wsPart, wbPart, "C9");



                    poDate = DateTime.FromOADate(double.Parse(_eHelper.GetCellValue(wsPart, wbPart, "C5")));
                    delDate = DateTime.FromOADate(double.Parse(_eHelper.GetCellValue(wsPart, wbPart, "C8")));

                    var sWarrIsEmpty = string.IsNullOrEmpty(sWarr) || (sWarr.ToUpper(new CultureInfo("en-US", false)) != "YES" && sWarr.ToUpper(new CultureInfo("en-US", false)) != "NO");

                    if (string.IsNullOrEmpty(vehicle)
                        || string.IsNullOrEmpty(driver)
                        || sWarrIsEmpty)
                    {
                        common.dataenum.Errors++;
                        _db.Add(new UploadError
                        {
                            UploadId = common.dataenum.UploadId,
                            Error =
                            (string.IsNullOrEmpty(vehicle) ? " Vehicle info not found; " : "")
                            + (string.IsNullOrEmpty(driver) ? " Driver name not found; " : "")
                            + (sWarrIsEmpty ? " Warranty return not found; '" + sWarr.ToUpper(new CultureInfo("en-US", false)) + "'" : ""
                            )
                        });

                        headerCorrect = false;
                    }
                }

                if (headerCorrect)
                {
                    sWarr = sWarr.ToUpper();
                    IncomingList incomingList = new()
                    {
                        SupCode = supCode,
                        DeliveryDate = delDate,
                        Checked = 0,
                        Closed = false,
                        Driver = driver,
                        IsWarranty = sWarr.ToUpper().Equals("YES"),
                        ItemCount = 0,
                        LastModifiedBy = user,
                        Po = po,
                        Vehicle = vehicle,
                        PoDate = poDate

                    };
                    await _db.AddAsync(incomingList);
                    await _db.SaveChangesAsync();
                    int icId = incomingList.IcId;

                    //Read data from table
                    while (headerCorrect)
                    {
                        readingRow++;
                        string lot, supRef, note;
                        DateTime refDate;
                        string itemNo = _eHelper.GetCellValue(wsPart, wbPart, "B" + readingRow).Trim();
                        //Check Supplier exist
                        ItemData item = await _db.ItemDatas.FindAsync(itemNo);
                        var checkQty = double.TryParse(_eHelper.GetCellValue(wsPart, wbPart, "E" + readingRow), out double qty);
                        var err = itemNo;
                        incomingList = _db.IncomingLists.Find(icId);

                        try
                        {
                            lot = _eHelper.GetCellValue(wsPart, wbPart, "D" + readingRow);
                            note = _eHelper.GetCellValue(wsPart, wbPart, "H" + readingRow);
                            supRef = _eHelper.GetCellValue(wsPart, wbPart, "F" + readingRow);
                            var checkDate = DateTime.TryParse(_eHelper.GetCellValue(wsPart, wbPart, "G" + readingRow), out refDate);
                            if (item != null && checkQty)
                            {

                                var pt = _db.ItemMasters
                                    .Where(x => x.ItemNo == item.ItemNo
                                        && x.IcId == icId
                                        && x.PtLotNo == lot
                                        && x.RefDate == refDate
                                        && x.RefNo == supRef
                                        && x.PtCmt == note
                                    ).FirstOrDefault();
                                //if existed: add quantity and rec user. Update
                                if (pt != null)
                                {
                                    pt.RecQty += qty;
                                    pt.RecBy = user;
                                    _db.Update(item);
                                    common.dataenum.Updated++;
                                    await _db.SaveChangesAsync();
                                }
                                //if NOT exist: Create new
                                else
                                {
                                    var newPt = new ItemMaster
                                    {
                                        ItemNo = itemNo,
                                        IcId = icId,
                                        LocCode = "",
                                        PtCmt = note,
                                        PtDateIn = delDate,
                                        PtHold = 0,
                                        PtLotNo = lot,
                                        PtQty = 0,
                                        Accepted = 0,
                                        Qc = "",
                                        RecBy = user,
                                        RecQty = qty,
                                        RefDate = refDate,
                                        SupCode = supCode,
                                        RefNo = supRef

                                    };
                                    incomingList.ItemCount++;
                                    await _db.AddAsync(newPt);
                                    await _db.SaveChangesAsync();
                                    newPt.ParentId = newPt.PtId;
                                    _db.Update(newPt);
                                    await _db.SaveChangesAsync();
                                    common.dataenum.Inserted++;
                                    common.dataenum.TotalRecord++;

                                }

                            }
                            else
                            {
                                if (string.IsNullOrEmpty(lot)
                                    && string.IsNullOrEmpty(note)
                                    && string.IsNullOrEmpty(supRef))
                                {
                                    break;
                                }

                                common.dataenum.Errors++;
                                _db.Add(new UploadError
                                {
                                    UploadId = common.dataenum.UploadId,
                                    Error = " Line " + readingRow + ": Error with "
                                    + (item == null ? "Item no." : "")
                                    + (!checkQty ? "quantity" : "")

                                    + ", Skipped line: " + readingRow
                                });
                                common.dataenum.TotalRecord++;
                                continue;
                            }



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
