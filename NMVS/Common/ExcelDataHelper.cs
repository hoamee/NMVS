using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NMVS.Common
{
    public class ExcelDataHelper
    {
        public string GetCellValue(WorksheetPart wsPart, WorkbookPart wbPart, string addressName)
        {
            string value = null;

            // Use its Worksheet property to get a reference to the cell 
            // whose address matches the address you supplied.
            try
            {
                Cell theCell = wsPart.Worksheet.Descendants<Cell>().
                  Where(c => c.CellReference == addressName).FirstOrDefault();

                // If the cell does not exist, return an empty string.
                if (theCell.InnerText.Length > 0)
                {
                    value = theCell.InnerText;

                    // If the cell represents an integer number, you are done. 
                    // For dates, this code returns the serialized value that 
                    // represents the date. The code handles strings and 
                    // Booleans individually. For shared strings, the code 
                    // looks up the corresponding value in the shared string 
                    // table. For Booleans, the code converts the value into 
                    // the words TRUE or FALSE.
                    if (theCell.DataType != null)
                    {
                        switch (theCell.DataType.Value)
                        {
                            case CellValues.SharedString:

                                // For shared strings, look up the value in the
                                // shared strings table.
                                var stringTable =
                                    wbPart.GetPartsOfType<SharedStringTablePart>()
                                    .FirstOrDefault();

                                // If the shared string table is missing, something 
                                // is wrong. Return the index that is in
                                // the cell. Otherwise, look up the correct text in 
                                // the table.
                                if (stringTable != null)
                                {
                                    value =
                                        stringTable.SharedStringTable
                                        .ElementAt(int.Parse(value)).InnerText;
                                }
                                break;

                            case CellValues.Boolean:
                                switch (value)
                                {
                                    case "0":
                                        value = "FALSE";
                                        break;
                                    default:
                                        value = "TRUE";
                                        break;
                                }
                                break;

                            
                        }
                    }
                }
            }
            catch
            {

                value = "";
            }
            if (!string.IsNullOrEmpty(value))
                value = value.Trim();
            return value;
        }

        public bool VefiryHeader(WorksheetPart wsPart, WorkbookPart wbPart, string columnAddr1, string columnAddr2, string columnAddr3,
            string columnAddr4, string columnAddr5,
            string columnAddr6, string columnAddr7, string columnAddr8, string columnAddr9, string columnAddr10,
            string value1, string value2, string value3, string value4, string value5, string value6, string value7, string value8, string value9, string value10)
        {
            bool headerCorrect = false;
            if (!string.IsNullOrEmpty(columnAddr1))
            {
                if (string.Equals(GetCellValue(wsPart, wbPart, columnAddr1), value1, StringComparison.OrdinalIgnoreCase))
                {
                    headerCorrect = true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (!string.IsNullOrEmpty(columnAddr2))
            {
                if (string.Equals(GetCellValue(wsPart, wbPart, columnAddr2) , value2, StringComparison.OrdinalIgnoreCase))
                {
                    headerCorrect = true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (!string.IsNullOrEmpty(columnAddr3))
            {
                if (string.Equals(GetCellValue(wsPart, wbPart, columnAddr3) , value3, StringComparison.OrdinalIgnoreCase))
                {
                    headerCorrect = true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (!string.IsNullOrEmpty(columnAddr4))
            {
                if (string.Equals(GetCellValue(wsPart, wbPart, columnAddr4) , value4, StringComparison.OrdinalIgnoreCase))
                {
                    headerCorrect = true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (!string.IsNullOrEmpty(columnAddr5))
            {
                if (string.Equals(GetCellValue(wsPart, wbPart, columnAddr5) , value5, StringComparison.OrdinalIgnoreCase))
                {
                    headerCorrect = true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (!string.IsNullOrEmpty(columnAddr6))
            {
                if (string.Equals(GetCellValue(wsPart, wbPart, columnAddr6) , value6, StringComparison.OrdinalIgnoreCase))
                {
                    headerCorrect = true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (!string.IsNullOrEmpty(columnAddr7))
            {
                if (string.Equals(GetCellValue(wsPart, wbPart, columnAddr7) , value7, StringComparison.OrdinalIgnoreCase))
                {
                    headerCorrect = true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (!string.IsNullOrEmpty(columnAddr8))
            {
                if (string.Equals(GetCellValue(wsPart, wbPart, columnAddr8) , value8, StringComparison.OrdinalIgnoreCase))
                {
                    headerCorrect = true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (!string.IsNullOrEmpty(columnAddr9))
            {
                
                if (string.Equals(GetCellValue(wsPart, wbPart, columnAddr9) , value9, StringComparison.OrdinalIgnoreCase))
                {
                    headerCorrect = true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (!string.IsNullOrEmpty(columnAddr10))
            {
                if (string.Equals(GetCellValue(wsPart, wbPart, columnAddr10) , value10, StringComparison.OrdinalIgnoreCase))
                {
                    headerCorrect = true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return headerCorrect;
        }
    }
}
