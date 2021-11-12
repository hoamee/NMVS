using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore.Internal;
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
    public class ItemDataService : IItemDataService
    {
        private readonly ApplicationDbContext _db;
        public ItemDataService(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<ItemData> GetItemList()
        {
            var model = (from item in _db.ItemDatas
                         from code in _db.GeneralizedCodes.Where(x => x.CodeFldName == "ItemType" && item.ItemType == x.CodeValue).DefaultIfEmpty()
                         select new ItemData
                         {
                             ItemNo = item.ItemNo,
                             Active = item.Active,
                             ItemType = code != null ? code.CodeDesc : "",
                             Flammable = item.Flammable,
                             ItemName = item.ItemName,
                             ItemPkg = item.ItemPkg,
                             ItemPkgQty = item.ItemPkgQty,
                             ItemUm = item.ItemUm,
                             ItemWhUnit = item.ItemWhUnit
                         });

            //var data = _db.ItemDatas
            //.Join(
            // _db.GeneralizedCodes,
            // item => item.ItemType,
            // code => code.CodeValue,
            // (item, code) => new ItemData
            // {
            //     ItemNo = item.ItemNo,
            //     Active = item.Active,
            //     ItemType = code.CodeDesc,
            //     Flammable = item.Flammable,
            //     ItemName = item.ItemName,
            //     ItemPkg = item.ItemPkg,
            //     ItemPkgQty = item.ItemPkgQty,
            //     ItemUm = item.ItemUm,
            //     ItemWhUnit = item.ItemWhUnit
            // });
            return model.ToList();
        }

        public async Task<CommonResponse<UploadReport>> ImportItemData(string filepath, string fileName, string user)
        {
            CommonResponse<UploadReport> common = new();
            common.dataenum = new()
            {
                FileName = fileName,
                UploadBy = user,
                UploadTime = DateTime.Now,
                UploadId = user + DateTime.Now.ToString("yyyyMMddHHmmss"),
                UploadFunction = "Item data upload"

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

                var headerCorrect = _eHelper.VefiryHeader(wsPart, wbPart, "A2", "B2", "C2", "D2", "E2", "F2", "A1", "", "", "",
                    "Item code", "Item name", "Unit", "Packing type", "Quantity per pack", "Warehouse space (per 1 pack)", "Document No.: IDL01", "","","");

                int readingRow = 2;

                if (!headerCorrect)
                {
                    common.dataenum.TotalRecord = 0;
                    common.dataenum.Updated = 0;
                    common.dataenum.Errors = 1;
                    common.dataenum.Inserted = 0;

                    common.status = -1;
                    common.message = "File header is in correct!";

                    _db.Add(new UploadError
                    {
                        UploadId = common.dataenum.UploadId,
                        Error = common.message
                    });

                    _db.Add(common.dataenum);
                    await _db.SaveChangesAsync();
                    return common;
                }

                while (headerCorrect)
                {
                    common.dataenum.TotalRecord++;
                    readingRow++;
                    string itemNo, itemName, um, pkgType;
                    double pkgQty, whSpace;
                    itemNo = _eHelper.GetCellValue(wsPart, wbPart, "A" + readingRow);
                    
                    itemName = _eHelper.GetCellValue(wsPart, wbPart, "B" + readingRow);
                    um = _eHelper.GetCellValue(wsPart, wbPart, "C" + readingRow);
                    pkgType = _eHelper.GetCellValue(wsPart, wbPart, "D" + readingRow);
                    var checkPgkQty = double.TryParse(_eHelper.GetCellValue(wsPart, wbPart, "E" + readingRow), out pkgQty);
                    var checkWthSpace = double.TryParse(_eHelper.GetCellValue(wsPart, wbPart, "F" + readingRow), out whSpace);
                    //if can not find supplier code, break
                    if (string.IsNullOrEmpty(itemNo))
                    {
                        if (string.IsNullOrEmpty(itemName)
                        && string.IsNullOrEmpty(um)
                        && string.IsNullOrEmpty(pkgType)
                        && !checkPgkQty
                        && !checkWthSpace)
                        {
                            common.dataenum.TotalRecord--;
                            break;
                        }

                        common.dataenum.Errors++;
                        _db.Add(new UploadError
                        {
                            Error = "Line " + readingRow + ": Missing item code",
                            UploadId = common.dataenum.UploadId
                        });
                        continue;
                    }

                    if (string.IsNullOrEmpty(itemName)
                        || string.IsNullOrEmpty(um)
                        || string.IsNullOrEmpty(pkgType)
                        || !checkPgkQty
                        || !checkWthSpace)
                    {
                        common.dataenum.Errors++;
                        var error = "Line " + readingRow + ":"
                            + (string.IsNullOrEmpty(itemName) ? " Item name not found; " : "")
                            + (string.IsNullOrEmpty(um) ? " Unit not found; " : "")
                            + (string.IsNullOrEmpty(pkgType) ? " Unit not found; " : "")
                            + (!checkPgkQty ? " Packing quantity not found; " : "")
                            + (!checkWthSpace ? " Warehouse space not found; " : "");
                        _db.Add(new UploadError
                        {
                            UploadId = common.dataenum.UploadId,
                            Error = error
                        });
                        continue;
                    }

                    //Check Supplier exist
                    ItemData item = await _db.ItemDatas.FindAsync(itemNo);

                    //if supplier exist: Update information
                    try
                    {
                        if (item != null)
                        {
                            item.ItemName = itemName;
                            item.ItemUm = um;
                            item.ItemPkg = pkgType;
                            item.ItemPkgQty = pkgQty;
                            item.ItemWhUnit = whSpace;
                            item.Active = true;
                            _db.Update(item);
                            common.dataenum.Updated++;
                            await _db.SaveChangesAsync();

                        }
                        //if supplier NOT exist: Create new
                        else
                        {
                            item = new ItemData
                            {
                                ItemNo = itemNo,
                                ItemName = itemName,
                                ItemUm = um,
                                ItemPkg = pkgType,
                                ItemPkgQty = pkgQty,
                                ItemWhUnit = whSpace,
                                Active = true
                            };

                            await _db.AddAsync(item);
                            await _db.SaveChangesAsync();
                            common.dataenum.Inserted++;
                            common.status = 1;
                            
                        }
                    }
                    catch (Exception e)
                    {
                        common.dataenum.Errors++;
                        var error = "Line " + readingRow + ": "
                           + e.Message + ";";
                        _db.Add(new UploadError
                        {
                            UploadId = common.dataenum.UploadId,
                            Error = error
                        });
                    }
                    
                }
            }

            _db.Add(common.dataenum);
            await _db.SaveChangesAsync();
            return common;
        }
    }
}
