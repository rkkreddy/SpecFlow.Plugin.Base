using System.Collections.Generic;
using System.Linq;

namespace Itamaram.SpecFlow.Plugin.Base
{
    public class ExamplesHeader
    {
        public ExamplesHeader(IEnumerable<string> values)
        {
            Values = values.ToList();
            Count = Values.Count();
        }

        public int Count { get; }

        public IEnumerable<string> Values { get; }
    }
}