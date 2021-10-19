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
    public class ItemDataService : IItemDataService
    {
        private readonly ApplicationDbContext _db;
        public ItemDataService(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<ItemData> GetItemList()
        {
            var data = _db.ItemDatas
            .Join(
             _db.GeneralizedCodes,
             item => item.ItemType,
             code => code.CodeValue,
             (item, code) => new ItemData
             {
                 ItemNo = item.ItemNo,
                 Active = item.Active,
                 ItemType = code.CodeDesc,
                 Flammable = item.Flammable,
                 ItemName = item.ItemName,
                 ItemPkg = item.ItemPkg,
                 ItemPkgQty = item.ItemPkgQty,
                 ItemUm = item.ItemUm,
                 ItemWhUnit = item.ItemWhUnit
             });
            return data.ToList();
        }

        public async Task<ExcelRespone> ImportItemData(string filepath)
        {
            ExcelRespone excelRespose = new();
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
                int readingRow = 2;
                int imported = 0;
                int updateted = 0;
                int recordCount = 0;
                int failed = 0;
                while (true)
                {
                    readingRow++;
                    string itemNo, itemName, um, pkgType;
                    double pkgQty, whSpace;
                    itemNo = _eHelper.GetCellValue(wsPart, wbPart, "A" + readingRow);
                    //if can not find supplier code, break
                    if (string.IsNullOrEmpty(itemNo))
                    {
                        excelRespose.error += "Line " + readingRow + ": Missing item code; ";
                        break;
                    }
                    itemName = _eHelper.GetCellValue(wsPart, wbPart, "B" + readingRow);
                    um = _eHelper.GetCellValue(wsPart, wbPart, "C" + readingRow);
                    pkgType = _eHelper.GetCellValue(wsPart, wbPart, "D" + readingRow);
                    var checkPgkQty = double.TryParse(_eHelper.GetCellValue(wsPart, wbPart, "E" + readingRow), out pkgQty);
                    var checkWthSpace = double.TryParse(_eHelper.GetCellValue(wsPart, wbPart, "F" + readingRow), out whSpace);

                    if (string.IsNullOrEmpty(itemName)
                        || string.IsNullOrEmpty(um)
                        || string.IsNullOrEmpty(pkgType)
                        || !checkPgkQty
                        || !checkWthSpace)
                    {
                        failed++;
                        excelRespose.error += "Line " + readingRow + ":"
                            + (string.IsNullOrEmpty(itemName) ? " Item name not found; " : "")
                            + (string.IsNullOrEmpty(um) ? " Unit not found; " : "")
                            + (string.IsNullOrEmpty(pkgType) ? " Unit not found; " : "")
                            + (!checkPgkQty ? " Packing quantity not found; " : "")
                            + (!checkWthSpace ? " Warehouse space not found; " : "");
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
                            
                            _db.Update(item);
                            updateted++;
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
                                ItemWhUnit = whSpace
                            };

                            await _db.AddAsync(item);
                            await _db.SaveChangesAsync();
                            imported++;
                        }
                    }
                    catch (Exception e)
                    {
                        failed++;
                        excelRespose.error += "Line " + readingRow + ": "
                           + e.Message + ";";
                    }
                    recordCount++;
                }
                excelRespose.processed = recordCount;
                excelRespose.updated = updateted;
                excelRespose.imported = imported;
            }


            return excelRespose;
        }
    }
}
