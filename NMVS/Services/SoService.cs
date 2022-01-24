using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using NMVS.Common;
using NMVS.Models;
using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Services
{
    public class SoService : ISoService
    {
        private readonly ApplicationDbContext _context;

        public SoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<SoVm> GetBrowseData()
        {
            var model = (from so in _context.SalesOrders
                         join cust in _context.Customers on so.CustCode equals cust.CustCode into all

                         from a in all.DefaultIfEmpty()
                         join c in _context.Customers on so.ShipTo equals c.CustCode
                         select new SoVm
                         {
                             CustCode = a.CustName,
                             SoType = so.SoType,
                             ShipTo = c.CustName,
                             Comment = so.Comment,
                             Confirm = so.Confirm,
                             ConfirmBy = so.ConfirmBy,
                             DueDate = so.DueDate,
                             OrdDate = so.OrdDate,
                             PriceDate = so.PriceDate,
                             ReqDate = so.ReqDate,
                             ShipVia = so.ShipVia,
                             SoCurr = so.SoCurr,
                             SoNbr = so.SoNbr,
                             UpdatedBy = so.UpdatedBy,
                             UpdatedOn = so.UpdatedOn,
                             WhConfirmed = so.RequestConfirmed,
                             WhConfirmedBy = so.RequestConfirmedBy,
                             ConfirmationNote = so.ConfirmationNote,
                             Closed = so.Closed,
                             ReqReported = so.ReqReported,
                             ReqReportedNote = so.ReqReportedNote,
                             ApprovalNote = so.ApprovalNote
                         }).ToList();

            return model;
        }

        public List<ItemAvailVm> GetItemNAvail()
        {
            List<ItemAvailVm> ls = (from dt in _context.ItemDatas.ToList()
                                    select new ItemAvailVm
                                    {
                                        ItemNo = dt.ItemNo,
                                        Desc = dt.ItemName
                                    }).ToList();

            return ls;
        }
        public SoDetailVm GetSoDetail(string soNbr)
        {
            var soVm = GetBrowseData().FirstOrDefault(x => x.SoNbr == soNbr);
            var soDets = _context.SoDetails.Where(x => x.SoNbr == soNbr).ToList();
            var oSoDets = (from so in soDets
                           join dt in _context.ItemDatas on so.ItemNo equals dt.ItemNo into all
                           from i in all.DefaultIfEmpty()
                           select new SoDetVm
                           {
                               Discount = so.Discount,
                               SpecDate = so.SpecDate,
                               ItemName = i.ItemName,
                               ItemNo = i.ItemNo,
                               NetPrice = so.NetPrice,
                               Quantity = so.Quantity,
                               RequiredDate = so.RequiredDate,
                               RqDetId = so.RqDetId,
                               SalesOrder = so.SalesOrder,
                               Shipped = so.Shipped,
                               SodId = so.SodId,
                               SoNbr = so.SoNbr,
                               Tax = so.Tax
                           }).ToList();

            return new SoDetailVm
            {
                SoDets = oSoDets,
                SoVm = soVm
            };
        }

        public string GetSoNbr(string input, int soType)
        {
            string nbr = input[2..];
            string header = input.Substring(0, 2);
            if (soType == 0)
            {
                return input;
            }
            else if (soType == 2)
            {
                if (header == "SO" || header == "WR")
                {
                    return "WT" + nbr;
                }
                else if (header != "WT")
                {
                    return "WT" + input;
                }
                else
                {
                    return input;
                }
            }
            else if (soType == 1)
            {
                var checkParentSO = _context.SalesOrders.Find("SO" + nbr);
                if (checkParentSO == null)
                {
                    return "";
                }
                else
                {
                    if (!checkParentSO.Closed)
                    {
                        return "uc";
                    }
                    var uq = _context.Unqualifieds.Where(x => x.SoNbr == checkParentSO.SoNbr);
                    if (uq == null)
                    {
                        return "uq404";
                    }
                }
                if (header == "SO" || header == "WR")
                {
                    return "WR" + nbr;
                }
                else if (header != "WR")
                {
                    return "WR" + input;
                }
                else
                {
                    return input;
                }
            }
            else
            {
                return "";
            }
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
                UploadFunction = "Sales order import"

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


                //check header
                string soNbr, soTo = "", shipTo = "", curr = "", note = "";
                int soType = -1;
                DateTime? delDate = DateTime.Now;
                DateTime? ordDate = delDate;
                try
                {
                    delDate = DateTime.FromOADate(double.Parse(_eHelper.GetCellValue(wsPart, wbPart, "C6")));
                    ordDate = DateTime.FromOADate(double.Parse(_eHelper.GetCellValue(wsPart, wbPart, "C5")));
                }
                catch
                {
                }
                bool headerCorrect = _eHelper.VefiryHeader(wsPart, wbPart, "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "B11", "F11",
                    "SO No.", "Sold-To ID", "Ship-To ID", "Order date", "Delivery Date", "Currency", "Note", "Type", "Item code", "Quantity");

                if (headerCorrect)
                {
                    soTo = _eHelper.GetCellValue(wsPart, wbPart, "C3");
                    shipTo = _eHelper.GetCellValue(wsPart, wbPart, "C4");
                    curr = _eHelper.GetCellValue(wsPart, wbPart, "C7");
                    note = _eHelper.GetCellValue(wsPart, wbPart, "C8");

                }

                //Check So Type
                headerCorrect = int.TryParse(_eHelper.GetCellValue(wsPart, wbPart, "C9"), out soType);
                if (soType == 0 || soType == 1)
                {
                    headerCorrect = true;
                }
                else
                {
                    common.dataenum.TotalRecord = 0;
                    common.dataenum.Updated = 0;
                    common.dataenum.Errors = 1;
                    common.dataenum.Inserted = 0;
                    common.message = "Sales order type incorrect";
                    common.status = -1;
                    _context.Add(new UploadError
                    {
                        UploadId = common.dataenum.UploadId,
                        Error = common.message
                    });

                    _context.Add(common.dataenum);
                    await _context.SaveChangesAsync();
                    return common;
                }

                //check so
                soNbr = _eHelper.GetCellValue(wsPart, wbPart, "C2");
                var oldSo = await _context.SalesOrders.FindAsync(soNbr);
                //if can not find customer code, break
                if (oldSo != null || string.IsNullOrEmpty(soTo) || string.IsNullOrEmpty(shipTo) || string.IsNullOrEmpty(curr))
                {

                    common.dataenum.TotalRecord = 0;
                    common.dataenum.Updated = 0;
                    common.dataenum.Errors = 1;
                    common.dataenum.Inserted = 0;
                    common.message = "Header data is incorrect:" +
                        (string.IsNullOrEmpty(soTo) ? " Sold-to not found;" : "") +
                        (string.IsNullOrEmpty(shipTo) ? " Ship-to not found;" : "") +
                        (string.IsNullOrEmpty(curr) ? " Currency not found;" : "") +
                        (oldSo != null ? " Sales order already existed;" : "")
                        ;
                    common.status = -1;


                    _context.Add(new UploadError
                    {
                        UploadId = common.dataenum.UploadId,
                        Error = common.message
                    });

                    _context.Add(common.dataenum);
                    await _context.SaveChangesAsync();
                    return common;
                }

                var soldTo = _context.Customers.Find(soTo);
                var shipToCust = _context.Customers.Find(shipTo);
                if (shipToCust == null || soldTo == null)
                {
                    common.message = shipTo == null ? "Sold-to not found" : soldTo == null ? "Ship-to not found" : common.message;
                    common.dataenum.TotalRecord = 0;
                    common.dataenum.Updated = 0;
                    common.dataenum.Errors = 1;
                    common.dataenum.Inserted = 0;
                    common.status = -1;


                    _context.Add(new UploadError
                    {
                        UploadId = common.dataenum.UploadId,
                        Error = common.message
                    });

                    _context.Add(common.dataenum);
                    await _context.SaveChangesAsync();
                    return common;
                }

                if (headerCorrect)
                {
                    SalesOrder salesOrder = new()
                    {
                        Comment = note,
                        CustCode = soTo,
                        ShipTo = shipTo,
                        PriceDate = (DateTime)delDate,
                        UpdatedOn = DateTime.Now,
                        OrdDate = (DateTime)ordDate,
                        ReqDate = delDate,
                        DueDate = (DateTime)delDate,
                        ShipVia = "Truck",
                        UpdatedBy = user,
                        SoType = soType,
                        SoCurr = curr,
                        SoNbr = soNbr
                    };
                    _context.Add(salesOrder);
                    await _context.SaveChangesAsync();

                    var requestCreated = RequestCreated(soNbr, user);
                    if (!requestCreated)
                    {
                        return ThrowExcelError("System error while creating inventory request", common.dataenum.UploadId);
                    }

                    int readingRow = 11;
                    //Read data from table
                    while (headerCorrect)
                    {
                        readingRow++;

                        string itemNo = _eHelper.GetCellValue(wsPart, wbPart, "B" + readingRow);

                        double quantity = 0;
                        var inputNumberCorrect = double.TryParse(_eHelper.GetCellValue(wsPart, wbPart, "F" + readingRow), out quantity);

                        if (string.IsNullOrEmpty(itemNo) && (quantity == 0 || !inputNumberCorrect))
                        {
                            break;
                        }
                        try
                        {
                            if (string.IsNullOrEmpty(itemNo))
                            {
                                common.dataenum.Errors++;
                                _context.Add(new UploadError
                                {
                                    UploadId = common.dataenum.UploadId,
                                    Error = "Line " + readingRow + ": Item no not found. Line skipped"
                                });
                                continue;
                            }
                            else
                            {
                                var itemData = _context.ItemDatas.Find(itemNo);
                                if (itemData == null)
                                {
                                    common.dataenum.Errors++;
                                    _context.Add(new UploadError
                                    {
                                        UploadId = common.dataenum.UploadId,
                                        Error = "Line " + readingRow + ": Item no. " + itemNo + " is not existed in system. Line skipped"
                                    });
                                    continue;
                                }
                            }
                            double discount = double.Parse(_eHelper.GetCellValue(wsPart, wbPart, "H" + readingRow));
                            double netprice = double.Parse(_eHelper.GetCellValue(wsPart, wbPart, "I" + readingRow));
                            double tax = double.Parse(_eHelper.GetCellValue(wsPart, wbPart, "J" + readingRow));
                            //Check Supplier exist

                            var det = new SoDetail
                            {
                                ItemNo = itemNo,
                                SoNbr = salesOrder.SoNbr,
                                Discount = discount,
                                NetPrice = netprice,
                                Quantity = quantity,
                                Tax = tax,
                                RequiredDate = delDate,

                            };
                            await _context.AddAsync(det);
                            await _context.SaveChangesAsync();

                            var rqDet = new RequestDet
                            {
                                RqID = soNbr,
                                ItemNo = det.ItemNo,
                                SpecDate = det.SpecDate,
                                Arranged = 0,
                                Picked = 0,
                                Issued = 0,
                                Ready = 0,
                                RequireDate = det.RequiredDate,
                                Shipped = null,
                                Quantity = det.Quantity,
                                SodId = det.SodId
                            };
                            _context.Add(rqDet);
                            _context.SaveChanges();

                            rqDet.SodId = det.SodId;
                            det.RqDetId = rqDet.DetId;
                            _context.Update(rqDet);
                            _context.Update(det);

                            common.dataenum.Inserted++;
                            common.dataenum.TotalRecord++;


                        }
                        catch (Exception e)
                        {
                            common.dataenum.Errors++;
                            _context.Add(new UploadError
                            {
                                UploadId = common.dataenum.UploadId,
                                Error = "Line " + readingRow + ": "
                               + e.Message + ";"
                            });
                            break;
                        }
                    }
                }
            }

            _context.Add(common.dataenum);
            await _context.SaveChangesAsync();
            return common;
        }

        private bool RequestCreated(string wrNbr, string user)
        {
            try
            {
                var invRq = _context.InvRequests.Find(wrNbr);
                if (invRq == null)
                {
                    _context.InvRequests.Add(new InvRequest
                    {
                        RqType = "Issue",
                        Ref = wrNbr,
                        RqBy = user,
                        RqDate = DateTime.Now,
                        RqID = wrNbr

                    });
                    _context.SaveChanges();
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private CommonResponse<UploadReport> ThrowExcelError(string mess, string uploadId)
        {
            CommonResponse<UploadReport> common = new();
            common.message = mess;
            common.dataenum.TotalRecord = 0;
            common.dataenum.Updated = 0;
            common.dataenum.Errors = 1;
            common.dataenum.Inserted = 0;
            common.status = -1;


            _context.Add(new UploadError
            {
                UploadId = mess,
                Error = mess
            });

            _context.Add(common.dataenum);
            _context.SaveChanges();
            return common;
        }


    }
}
