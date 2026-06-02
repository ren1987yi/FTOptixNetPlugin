using ExcelDataReader;
using InternalWebService.XSSheet.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWebService.XSSheet
{
    public class XSHelper
    {
        public static string GetCellName(int col, int row)
        {
            var colName = string.Empty;
            while (col >= 0)
            {
                colName = (char)('A' + (col % 26)) + colName;
                col = col / 26 - 1;
            }
            return $"{colName}{row}";
        }

        public static List<sheet> ExcelToXSSheets(string filepath)
        {
            var sheets = new List<sheet>();

            DataSet result = null;

            using (var stream = File.Open(filepath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // 将数据读取到 DataSet
                    result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true // 第一行作为列名
                        }
                    });
                }
            }


            result.Tables.Cast<DataTable>().ToList().ForEach(table =>
            {
                var sheet = new sheet()
                {
                    name = table.TableName,
                    rows = new Rows()
                };

                sheet.cols.len = table.Columns.Count;

                var rowHeader = new Row()
                {
                    cells = new Cells()
                };

                for (var i = 0; i < table.Columns.Count; i++)
                {
                    rowHeader.cells.Add(i.ToString(), new Cell(table.Columns[i].ColumnName));

                }
                sheet.rows.Add("0", rowHeader);


                for (var i = 0; i < table.Rows.Count; i++)
                {
                    var row = new Row()
                    {
                        cells = new Cells()
                    };
                    for (var j = 0; j < table.Columns.Count; j++)
                    {
                        var cellValue = table.Rows[i][j]?.ToString() ?? string.Empty;
                        row.cells.Add(j.ToString(), new Cell(cellValue));
                    }
                    sheet.rows.Add((i + 1).ToString(), row);
                }
                sheets.Add(sheet);
            });


            return sheets;
        }
    }

}
