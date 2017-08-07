using System.Collections.Generic;
using System.Linq;
using Gherkin.Ast;
using NUnit.Framework;

namespace Itamaram.SpecFlow.Plugin.Base.Tests
{
    public class RowSorterTests
    {
        [Test]
        public void InorderTest()
        {
            var result = RowSorter.MaybeSortRows(
                Row("1", "2", "3"),
                new List<TableRow>
                {
                    Row("1", "2", "3"),
                    Row("4", "5", "6")
                });

            CollectionAssert.AreEqual(new[] {"4", "5", "6"}, result.Single().Values());
        }

        [Test]
        public void OutOfOrderTest()
        {
            var result = RowSorter.MaybeSortRows(
                Row("1", "2", "3"),
                new List<TableRow>
                {
                    Row("2", "3", "1"),
                    Row("4", "5", "6")
                });

            CollectionAssert.AreEqual(new[] { "6", "4", "5"}, result.Single().Values());
        }

        [Test]
        public void UnsortableTest()
        {
            var result = RowSorter.MaybeSortRows(
                Row("1", "2", "3"),
                new List<TableRow>
                {
                    Row("x", "y", "z"),
                    Row("4", "5", "6")
                }).ToList();

            Assert.AreEqual(2, result.Count);
            CollectionAssert.AreEqual(new[] { "x", "y", "z" }, result[0].Values());
            CollectionAssert.AreEqual(new[] { "4", "5", "6" }, result[1].Values());
        }

        private static TableRow Row(params string[] values)
        {
            return new TableRow(null, values.Select(v => new TableCell(null, v)).ToArray());
        }
    }

    public static class Extensions
    {
        public static IEnumerable<string> Values(this TableRow row)
        {
            return row.Cells.Select(c => c.Value);
        }
    }
}