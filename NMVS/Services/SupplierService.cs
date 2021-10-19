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
    public class SupplierService : ISupplierService
    {

        private readonly ApplicationDbContext _db;
        public SupplierService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CommonResponse<Supplier>> AddSupplier(Supplier supplier)
        {
            CommonResponse<Supplier> commonRespose = new();
            var checkCust = await _db.Suppliers.FindAsync(supplier.SupCode);
            //If user not exist, then create new
            if (checkCust == null)
            {
                try
                {
                    _db.Suppliers.Add(supplier);
                    await _db.SaveChangesAsync();
                    commonRespose.status = 1;
                    commonRespose.message = "Success";
                }
                catch (Exception e)
                {
                    commonRespose.status = -1;
                    commonRespose.message = e.Message;
                }
            }
            else //User exist: return this user to edit
            {
                commonRespose.dataenum = checkCust;
                commonRespose.status = 2;
            }

            return commonRespose;
        }

        public Supplier GetSupplier(string supplier)
        {
            var sup = _db.Suppliers.Find(supplier);
            return sup;
        }

        public List<Supplier> GetSupplierList()
        {
            var suppliers = (from c in _db.Suppliers
                                select new Supplier
                                {
                                    SupCode = c.SupCode,
                                    SupDesc = c.SupDesc,
                                    Active = c.Active,
                                    Addr = c.Addr + (!string.IsNullOrEmpty(c.City) ? ", " + c.City : "") + (!string.IsNullOrEmpty(c.Ctry) ? ", " + c.Ctry : ""),
                                    Email1 = c.Email1 + ((!string.IsNullOrEmpty(c.Email1) && !string.IsNullOrEmpty(c.Email2)) ? " ; " + c.Email2 : c.Email2),
                                    Phone1 = c.Phone1 + ((!string.IsNullOrEmpty(c.Phone1) && !string.IsNullOrEmpty(c.Phone2)) ? " ; " + c.Phone2 : c.Phone2),
                                    Note = c.Note,
                                    TaxCode = c.TaxCode
                                }

                        ).ToList();
            return suppliers;
        }

        public async Task<ExcelRespone> ImportSupplier(string fileName)
        {
            ExcelRespone excelRespose = new();
            ExcelDataHelper _eHelper = new();
            // Open the spreadsheet document for read-only access.
            using (SpreadsheetDocument document =
                SpreadsheetDocument.Open(fileName, false))
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
                    string supCode, supDesc, address;
                    
                    supDesc = _eHelper.GetCellValue(wsPart, wbPart, "B" + readingRow);
                    //if can not find supplier code, break
                    if (string.IsNullOrEmpty(supDesc))
                    {
                        excelRespose.error += "";
                        break;
                    }
                    supCode = _eHelper.GetCellValue(wsPart, wbPart, "A" + readingRow);
                    address = _eHelper.GetCellValue(wsPart, wbPart, "C" + readingRow);

                    if (string.IsNullOrEmpty(supDesc)
                        || string.IsNullOrEmpty(address))
                    {
                        failed++;
                        excelRespose.error += "Line " + readingRow + ":"
                            + (string.IsNullOrEmpty(supDesc) ? " Supplier name not found;" : "")
                            + (string.IsNullOrEmpty(address) ? " Address not found;" : "")
                            ;
                        continue;
                    }

                    //Check Supplier exist
                    Supplier supplier = await _db.Suppliers.FindAsync(supCode);

                    //if supplier exist: Update information
                    try
                    {
                        if (supplier != null)
                        {
                            supplier.SupDesc = supDesc;
                            supplier.Addr = address;
                            supplier.City = _eHelper.GetCellValue(wsPart, wbPart, "D" + readingRow);
                            supplier.Ctry = _eHelper.GetCellValue(wsPart, wbPart, "E" + readingRow);
                            supplier.TaxCode = _eHelper.GetCellValue(wsPart, wbPart, "F" + readingRow);
                            supplier.Email1 = _eHelper.GetCellValue(wsPart, wbPart, "G" + readingRow);
                            supplier.Email2 = _eHelper.GetCellValue(wsPart, wbPart, "H" + readingRow);
                            supplier.Phone1 = _eHelper.GetCellValue(wsPart, wbPart, "I" + readingRow);
                            supplier.Phone2 = _eHelper.GetCellValue(wsPart, wbPart, "J" + readingRow);
                            supplier.Note = _eHelper.GetCellValue(wsPart, wbPart, "K" + readingRow);
                            supplier.Active = true;
                            _db.Update(supplier);
                            updateted++;
                            await _db.SaveChangesAsync();

                        }
                        //if supplier NOT exist: Create new
                        else
                        {
                            supplier = new Supplier
                            {
                                SupCode = supCode,
                                SupDesc = supDesc,
                                Addr = address,
                                City = _eHelper.GetCellValue(wsPart, wbPart, "D" + readingRow),
                                Ctry = _eHelper.GetCellValue(wsPart, wbPart, "E" + readingRow),
                                TaxCode = _eHelper.GetCellValue(wsPart, wbPart, "F" + readingRow),
                                Email1 = _eHelper.GetCellValue(wsPart, wbPart, "G" + readingRow),
                                Email2 = _eHelper.GetCellValue(wsPart, wbPart, "H" + readingRow),
                                Phone1 = _eHelper.GetCellValue(wsPart, wbPart, "I" + readingRow),
                                Phone2 = _eHelper.GetCellValue(wsPart, wbPart, "J" + readingRow),
                                Note = _eHelper.GetCellValue(wsPart, wbPart, "K" + readingRow),
                                Active = true
                            };

                            await _db.AddAsync(supplier);
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

        public async Task<CommonResponse<Supplier>> UpdateSupplier(Supplier supplier)
        {
            CommonResponse<Supplier> common = new();
            try
            {
                _db.Update(supplier);
                await _db.SaveChangesAsync();
                common.status = 1;
            }
            catch (Exception e)
            {
                common.status = -1;
                common.message = e.Message;
            }
            return common;
        }
    }
}
