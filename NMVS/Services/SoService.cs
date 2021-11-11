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
            List<ItemAvailVm> ls = new();
            var ptMstr = _context.ItemMasters.ToList();
            foreach (var item in _context.ItemDatas.ToList())
            {
                var pt = ptMstr.Where(x => x.ItemNo == item.ItemNo);
                double avail = 0;
                if (pt.Any())
                {
                    var hold = pt.Sum(x => x.PtHold);
                    var qty = pt.Sum(x => x.PtQty);
                    avail = qty - hold;
                }

                ls.Add(new ItemAvailVm
                {
                    ItemNo = item.ItemNo,
                    Quantity = avail,
                    Desc = item.ItemName
                });


            }
            return ls;
        }
        public SoDetailVm GetSoDetail(string soNbr)
        {
            var soVm = GetBrowseData().FirstOrDefault(x => x.SoNbr == soNbr);
            var soDets = _context.SoDetails.Where(x => x.SoNbr == soNbr).ToList();

            return new SoDetailVm
            {
                SoDets = soDets,
                SoVm = soVm
            };
        }

        public string GetSoNbr(string input, string soType)
        {
            string nbr = input[2..];
            string header = input.Substring(0, 2);
            if (soType == "Sale")
            {
                if (header == "WT" || header == "WR")
                {
                    return "SO" + nbr;
                }
                else if (header != "SO")
                {
                    return "SO" + input;
                }
                else
                {
                    return input;
                }
            }
            else if (soType == "WH Transfer")
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
            else if (soType == "Warranty return")
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

                int readingRow = 10;

                //check header
                string soNbr, soTo = "", shipTo = "", curr = "", note = "";
                var delDate = DateTime.FromOADate(double.Parse(_eHelper.GetCellValue(wsPart, wbPart, "C6")));
                var ordDate = DateTime.FromOADate(double.Parse(_eHelper.GetCellValue(wsPart, wbPart, "C5")));
                bool headerCorrect = true;

                if (headerCorrect)
                {
                    soTo = _eHelper.GetCellValue(wsPart, wbPart, "C3");
                    shipTo = _eHelper.GetCellValue(wsPart, wbPart, "C4");
                    curr = _eHelper.GetCellValue(wsPart, wbPart, "C7");
                    note = _eHelper.GetCellValue(wsPart, wbPart, "C8");

                }

                soNbr = _eHelper.GetCellValue(wsPart, wbPart, "C2");
                var oldSo = await _context.SalesOrders.FindAsync(soNbr);
                //if can not find supplier code, break
                if (oldSo != null || string.IsNullOrEmpty(soTo) || string.IsNullOrEmpty(shipTo) || string.IsNullOrEmpty(curr))
                {

                    common.dataenum.TotalRecord = 0;
                    common.dataenum.Updated = 0;
                    common.dataenum.Errors = 1;
                    common.dataenum.Inserted = 0;
                    common.message = "input header is incorrect!" +
                        (string.IsNullOrEmpty(soTo) ? "Sold-to not found" : "") +
                        (string.IsNullOrEmpty(shipTo) ? "ship-to not found" : "") +
                        (string.IsNullOrEmpty(curr) ? "currency not found" : "")+
                        (oldSo != null ? "Sales order already existed" : "")
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
                var shipToCust = _context.Customers.Find(soTo);
                if (shipToCust == null || soldTo == null)
                {
                    common.dataenum.TotalRecord = 0;
                    common.dataenum.Updated = 0;
                    common.dataenum.Errors = 1;
                    common.dataenum.Inserted = 0;
                    common.message = "Sold-to and Ship-to not found";
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
                        PriceDate = delDate,
                        UpdatedOn = DateTime.Now,
                        OrdDate = ordDate,
                        ReqDate = delDate,
                        DueDate = delDate,
                        ShipVia = "Truck",
                        UpdatedBy = user,
                        SoType = "Sale",
                        SoCurr = curr,
                        SoNbr = soNbr


                    };
                    _context.Add(salesOrder);
                    await _context.SaveChangesAsync();
                    string sonbr = salesOrder.SoNbr;

                    //Read data from table
                    while (headerCorrect)
                    {
                        readingRow++;
                        try
                        {
                            string itemNo = _eHelper.GetCellValue(wsPart, wbPart, "B" + readingRow).Trim();
                            if (string.IsNullOrEmpty(itemNo))
                            {
                                break;
                            }
                            double quantity = double.Parse(_eHelper.GetCellValue(wsPart, wbPart, "F" + readingRow).Trim());
                            double discount = double.Parse(_eHelper.GetCellValue(wsPart, wbPart, "H" + readingRow).Trim());
                            double netprice = double.Parse(_eHelper.GetCellValue(wsPart, wbPart, "I" + readingRow).Trim());

                            double tax = double.Parse(_eHelper.GetCellValue(wsPart, wbPart, "J" + readingRow).Trim());
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
    }
}
