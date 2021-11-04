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

        public async Task<CommonResponse<UploadReport>> ImportSupplier(string filepath, string fileName, string user)
        {
            CommonResponse<UploadReport> common = new();
            common.dataenum = new()
            {
                FileName = fileName,
                UploadBy = user,
                UploadTime = DateTime.Now,
                UploadId = user + DateTime.Now.ToString("yyyyMMddHHmmss"),
                UploadFunction = "Supplier upload"

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
                var headerCorrect = _eHelper.VefiryHeader(wsPart, wbPart, "A2", "B2", "C2", "D2", "E2", "F2", "G2", "H2", "I2", "J2",
                    "Supplier code", "Supplier Desc", "Address", "City", "Country", "Tax code", "Email 1", "Email 2", "Phone 1", "Phone 2");

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
                    string supCode, supDesc, address;

                    supDesc = _eHelper.GetCellValue(wsPart, wbPart, "B" + readingRow);
                    //if can not find supplier code, break

                    supCode = _eHelper.GetCellValue(wsPart, wbPart, "A" + readingRow);
                    address = _eHelper.GetCellValue(wsPart, wbPart, "C" + readingRow);



                    if (string.IsNullOrEmpty(supDesc)
                        || string.IsNullOrEmpty(address)
                        || string.IsNullOrEmpty(supCode))
                    {
                        if (string.IsNullOrEmpty(supDesc)
                        && string.IsNullOrEmpty(address)
                        && string.IsNullOrEmpty(supCode))
                        {
                            common.dataenum.TotalRecord--;
                            break;
                        }

                        common.dataenum.Errors++;
                        _db.Add(new UploadError
                        {
                            Error = "Line " + readingRow + ":"
                            + (string.IsNullOrEmpty(supDesc) ? " Supplier name not found;" : "")
                            + (string.IsNullOrEmpty(address) ? " Address not found;" : "")
                            + (string.IsNullOrEmpty(supCode) ? " Supplier code not found;" : ""),
                            UploadId = common.dataenum.UploadId
                        });
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
                            common.dataenum.Updated++;
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
