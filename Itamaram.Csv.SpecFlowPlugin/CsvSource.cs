using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Itamaram.SpecFlow.Plugin.Base;

namespace Itamaram.Csv.SpecFlowPlugin
{
    public class CsvSource: NamedTagExampleGenerator
    {
        protected override string TagName { get; } = "csv";

        public override IEnumerable<IEnumerable<string>> GetRowsInternal(string args, string dir, string specfile, IEnumerable<string> header)
        {
            var readheader = false;
            var file = args;

            if (args.EndsWith("+h"))
            {
                readheader = true;
                file = file.Substring(0, file.Length - 2);
            }

            using (var stream = new StreamReader(Path.Combine(dir, file)))
            using (var reader = new CsvReader(stream))
                return ReadAllRecords(reader, readheader).ToList();
        }

        private static IEnumerable<IEnumerable<string>> ReadAllRecords(ICsvReader reader, bool header)
        {
            reader.Configuration.HasHeaderRecord = header;

            while (reader.Read())
                yield return reader.CurrentRecord;
        }
    }
}
