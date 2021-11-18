using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
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

                return "";
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
           

            if (!string.IsNullOrEmpty(columnAddr2))
            {
                if (string.Equals(GetCellValue(wsPart, wbPart, columnAddr2), value2, StringComparison.OrdinalIgnoreCase))
                {
                    headerCorrect = true;
                }
                else
                {
                    return false;
                }
            }
           
            if (!string.IsNullOrEmpty(columnAddr3))
            {
                if (string.Equals(GetCellValue(wsPart, wbPart, columnAddr3), value3, StringComparison.OrdinalIgnoreCase))
                {
                    headerCorrect = true;
                }
                else
                {
                    return false;
                }
            }
           

            if (!string.IsNullOrEmpty(columnAddr4))
            {
                if (string.Equals(GetCellValue(wsPart, wbPart, columnAddr4), value4, StringComparison.OrdinalIgnoreCase))
                {
                    headerCorrect = true;
                }
                else
                {
                    return false;
                }
            }
           

            if (!string.IsNullOrEmpty(columnAddr5))
            {
                if (string.Equals(GetCellValue(wsPart, wbPart, columnAddr5), value5, StringComparison.OrdinalIgnoreCase))
                {
                    headerCorrect = true;
                }
                else
                {
                    return false;
                }
            }
            

            if (!string.IsNullOrEmpty(columnAddr6))
            {
                if (string.Equals(GetCellValue(wsPart, wbPart, columnAddr6), value6, StringComparison.OrdinalIgnoreCase))
                {
                    headerCorrect = true;
                }
                else
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(columnAddr7))
            {
                if (string.Equals(GetCellValue(wsPart, wbPart, columnAddr7), value7, StringComparison.OrdinalIgnoreCase))
                {
                    headerCorrect = true;
                }
                else
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(columnAddr8))
            {
                if (string.Equals(GetCellValue(wsPart, wbPart, columnAddr8), value8, StringComparison.OrdinalIgnoreCase))
                {
                    headerCorrect = true;
                }
                else
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(columnAddr9))
            {

                if (string.Equals(GetCellValue(wsPart, wbPart, columnAddr9), value9, StringComparison.OrdinalIgnoreCase))
                {
                    headerCorrect = true;
                }
                else
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(columnAddr10))
            {
                if (string.Equals(GetCellValue(wsPart, wbPart, columnAddr10), value10, StringComparison.OrdinalIgnoreCase))
                {
                    headerCorrect = true;
                }
                else
                {
                    return false;
                }
            }

            return headerCorrect;
        }

        public static void InsertText(SpreadsheetDocument spreadSheet, WorksheetPart worksheetPart, string text)
        {

            // Get the SharedStringTablePart. If it does not exist, create a new one.
            SharedStringTablePart shareStringPart;
            if (spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
            {
                shareStringPart = spreadSheet.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
            }
            else
            {
                shareStringPart = spreadSheet.WorkbookPart.AddNewPart<SharedStringTablePart>();
            }

            // Insert the text into the SharedStringTablePart.
            int index = InsertSharedStringItem(text, shareStringPart);

            // Insert cell A1 into the new worksheet.
            Cell cell = InsertCellInWorksheet("A", 1, worksheetPart);

            // Set the value of cell A1.
            cell.CellValue = new CellValue(index.ToString());
            cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);

            // Save the new worksheet.
            worksheetPart.Worksheet.Save();

        }

        // Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        // and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new DocumentFormat.OpenXml.Spreadsheet.Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }


        // Given a column name, a row index, and a WorksheetPart, inserts a cell into the worksheet. 
        // If the cell already exists, returns it. 
        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.  
            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                    {
                        refCell = cell;
                        break;
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }
    }
}
