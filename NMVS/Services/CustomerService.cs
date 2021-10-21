using NMVS.Models;
using NMVS.Models.DbModels;
using NMVS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using NMVS.Common;

namespace NMVS.Services
{
    public class CustomerService : ICustomerService
    {

        private readonly ApplicationDbContext _db;
        public CustomerService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<CommonResponse<Customer>> AddCustomer(CustomerVm customer)
        {
            CommonResponse<Customer> commonRespose = new();
            var checkCust = await _db.Customers.FindAsync(customer.CustCode);
            //If user not exist, then create new
            if (checkCust == null)
            {
                try
                {
                    _db.Customers.Add(new Customer
                    {
                        CustCode = customer.CustCode,
                        Active = customer.Active,
                        Addr = customer.Addr,
                        AgentNo = customer.AgentNo,
                        ApCode = customer.ApCode,
                        City = customer.City,
                        Ctry = customer.Ctry,
                        CustName = customer.CustName,
                        Note = customer.Note,
                        Email1 = customer.Email1,
                        Email2 = customer.Email2,
                        Phone1 = customer.Phone1,
                        Phone2 = customer.Phone2,
                        TaxCode = customer.TaxCode
                    });
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


        public Customer GetCustomer(string customer)
        {
            var cust = _db.Customers.Find(customer);
            return cust;
        }

        public List<CustomerVm> GetCustomerList()
        {
            var customerList = (from c in _db.Customers
                                select new CustomerVm
                                {
                                    CustCode = c.CustCode,
                                    Active = c.Active,
                                    Addr = c.Addr + (!string.IsNullOrEmpty(c.City) ? ", " + c.City : "") + (!string.IsNullOrEmpty(c.Ctry) ? ", " + c.Ctry : ""),
                                    Email1 = c.Email1 + ((!string.IsNullOrEmpty(c.Email1) && !string.IsNullOrEmpty(c.Email2)) ? " ; " + c.Email2 : c.Email2),
                                    Phone1 = c.Phone1 + ((!string.IsNullOrEmpty(c.Phone1) && !string.IsNullOrEmpty(c.Phone2)) ? " ; " + c.Phone2 : c.Phone2),
                                    Note = c.Note,
                                    AgentNo = c.AgentNo,
                                    TaxCode = c.TaxCode,
                                    ApCode = c.ApCode,
                                    CustName = c.CustName
                                }

                         ).ToList();
            return customerList;
        }

        public async Task<ExcelRespone> ImportCustomer(string fileName)
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
                    string custCode, apCode, agent, custName, address;
                    custCode = _eHelper.GetCellValue(wsPart, wbPart, "A" + readingRow);

                    //if can not find customer code, breack
                    if (string.IsNullOrEmpty(custCode))
                    {
                        excelRespose.error += "Customer code not found at line " + readingRow + ", Import session terminated!";
                        break;
                    }
                    apCode = _eHelper.GetCellValue(wsPart, wbPart, "B" + readingRow);
                    agent = _eHelper.GetCellValue(wsPart, wbPart, "C" + readingRow);
                    custName = _eHelper.GetCellValue(wsPart, wbPart, "D" + readingRow);
                    address = _eHelper.GetCellValue(wsPart, wbPart, "E" + readingRow);

                    if (string.IsNullOrEmpty(apCode)
                        || string.IsNullOrEmpty(agent)
                        || string.IsNullOrEmpty(custName)
                        || string.IsNullOrEmpty(address))
                    {
                        failed++;
                        excelRespose.error += "Line " + readingRow + ":"
                            + (string.IsNullOrEmpty(apCode) ? " AP code not found;" : "")
                            + (string.IsNullOrEmpty(agent) ? " Agent No. not found;" : "")
                            + (string.IsNullOrEmpty(custName) ? " Customer name not found;" : "")
                            + (string.IsNullOrEmpty(address) ? " Address not found;" : "")
                            ;
                        continue;
                    }

                    //Check customer exist
                    Customer customer = await _db.Customers.FindAsync(custCode);

                    //if customer exist: Update information
                    try
                    {
                        if (customer != null)
                        {
                            customer.CustName = custName;
                            customer.ApCode = apCode;
                            customer.AgentNo = agent;
                            customer.Addr = address;
                            customer.City = _eHelper.GetCellValue(wsPart, wbPart, "F" + readingRow);
                            customer.Ctry = _eHelper.GetCellValue(wsPart, wbPart, "G" + readingRow);
                            customer.TaxCode = _eHelper.GetCellValue(wsPart, wbPart, "H" + readingRow);
                            customer.Email1 = _eHelper.GetCellValue(wsPart, wbPart, "I" + readingRow);
                            customer.Email2 = _eHelper.GetCellValue(wsPart, wbPart, "J" + readingRow);
                            customer.Phone1 = _eHelper.GetCellValue(wsPart, wbPart, "K" + readingRow);
                            customer.Phone2 = _eHelper.GetCellValue(wsPart, wbPart, "L" + readingRow);
                            customer.Note = _eHelper.GetCellValue(wsPart, wbPart, "M" + readingRow);
                            customer.Active = true;
                            _db.Update(customer);
                            updateted++;
                            await _db.SaveChangesAsync();

                        }
                        //if customer NOT exist: Create new
                        else
                        {
                            customer = new Customer
                            {
                                CustCode = custCode,
                                CustName = custName,
                                ApCode = apCode,
                                AgentNo = agent,
                                Addr = address,
                                City = _eHelper.GetCellValue(wsPart, wbPart, "F" + readingRow),
                                Ctry = _eHelper.GetCellValue(wsPart, wbPart, "G" + readingRow),
                                TaxCode = _eHelper.GetCellValue(wsPart, wbPart, "H" + readingRow),
                                Email1 = _eHelper.GetCellValue(wsPart, wbPart, "I" + readingRow),
                                Email2 = _eHelper.GetCellValue(wsPart, wbPart, "J" + readingRow),
                                Phone1 = _eHelper.GetCellValue(wsPart, wbPart, "K" + readingRow),
                                Phone2 = _eHelper.GetCellValue(wsPart, wbPart, "L" + readingRow),
                                Note = _eHelper.GetCellValue(wsPart, wbPart, "H" + readingRow),
                                Active = true
                            };

                            await _db.AddAsync(customer);
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

        public async Task<CommonResponse<Customer>> UpdateCustomer(Customer customer)
        {
            CommonResponse<Customer> common = new();
            try
            {
                _db.Update(customer);
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
