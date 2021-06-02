using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Itamaram.SpecFlow.Plugin.Base;
using OfficeOpenXml;

namespace Itamaram.Excel.SpecFlowPlugin
{
    public class ExcelSource : NamedTagExampleGenerator
    {
        protected override string TagName { get; } = "excel";
        JsonConfigFactory _jsonConfigFactory;
        public ExcelSource() : base()
        {
            _jsonConfigFactory = new JsonConfigFactory();
        }
        public override IEnumerable<IEnumerable<string>> GetRowsInternal(string args, string dir, string specfile, ExamplesHeader examplesHeader)
        {
            var parts = args.Split(new[] { ':' }, 2);
            var name = parts[0];
            var worksheet = parts.Length > 1 ? parts[1] : null;
            bool? skipFirstRow = null;
            JsonConfig jsonConfig = new JsonConfigFactory().GetConfig();
            skipFirstRow = jsonConfig == null ? false : jsonConfig.SkipFirstRow;
            if (skipFirstRow == null) throw new Exception("SkipFirstRow is null");
            var columns = examplesHeader.Count;
            var reqColumns = new List<int>();
            using (var excel = new ExcelPackage(new FileInfo(Path.Combine(dir, name))))
            {
                var sheet = string.IsNullOrEmpty(worksheet)
                    ? excel.Workbook.Worksheets.First()
                    : excel.Workbook.Worksheets[worksheet];
                int startRow = 1, endRow = sheet.Dimension.End.Row, rowOffset = 0;
                if (sheet.Dimension == null)
                {
                    return Enumerable.Empty<IEnumerable<string>>();
                }
                
                if ((bool)skipFirstRow) rowOffset = 1;
                startRow += rowOffset;
                endRow -= rowOffset;
                if (!(bool)skipFirstRow)
                {
                    return Enumerable.Range(startRow, endRow)
                    .Select(i => Enumerable.Range(1, columns).Select(j => sheet.Cells[i, j]))
                    .Select(r => r.Select(c => c.Text ?? string.Empty).ToList())
                    .ToList();
                }
                string examplesTableColumnNames = "", excelColumnNames = "";
                foreach (string header in examplesHeader.Values)
                {
                    examplesTableColumnNames = examplesTableColumnNames == "" ? header : examplesTableColumnNames + ";" + header;
                }
                foreach (ExcelRangeBase cell in sheet.Cells["1:1"])
                {
                    excelColumnNames = excelColumnNames == "" ? cell.Value.ToString() : excelColumnNames + ";" + cell.Value.ToString();
                }
                
                foreach (string header in examplesHeader.Values)
                {
                    try
                    {
                        int colIdx = sheet.Cells["1:1"].First(c => c.Value.ToString() == header).Start.Column;
                        reqColumns.Add(colIdx);
                    }
                    catch (InvalidOperationException iOpEx)
                    {
                        throw new Exception($"Couldn't find Examples table column name '{header}' in Excel file '{name}' column names '{excelColumnNames}'. Examples table column names are '{examplesTableColumnNames}'", iOpEx);
                    }

                }
                return Enumerable.Range(startRow, endRow)
                    .Select(i => reqColumns.Select(j => sheet.Cells[i, j]))
                    .Select(r => r.Select(c => c.Text ?? string.Empty).ToList())
                    .ToList();
            }
        }
    }
}
