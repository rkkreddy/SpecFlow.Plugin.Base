using System.Collections.Generic;
using System.Linq;
using Gherkin.Ast;

namespace Itamaram.SpecFlow.Plugin.Base
{
    internal class RowSorter
    {
        public static IEnumerable<TableRow> MaybeSortRows(TableRow header, IReadOnlyList<TableRow> rows)
        {
            if (rows.Count == 0)
                return Enumerable.Empty<TableRow>();

            var first = rows[0];
            if (header.Cells.Count() != first.Cells.Count())
                return rows;

            var map = header.Cells.Select((c, i) => new { c.Value, Index = i }).ToDictionary(o => o.Value, o => o.Index);
            var order = new int[header.Cells.Count()];

            foreach (var o in first.Cells.Select((c, i) => new { c.Value, Index = i }))
            {
                if (map.TryGetValue(o.Value, out var index))
                    order[o.Index] = index;
                else
                    return rows;
            }

            return rows.Skip(1)
                .Select(r =>
                {
                    var result = new TableCell[order.Length];

                    foreach (var o in r.Cells.Select((c, i) => new { c.Value, Index = i }))
                        result[order[o.Index]] = new TableCell(r.Location, o.Value);

                    return new TableRow(r.Location, result);
                });
        }
    }
}