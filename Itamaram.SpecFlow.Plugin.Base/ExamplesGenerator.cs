using System.Collections.Generic;

namespace Itamaram.SpecFlow.Plugin.Base
{
    public interface ExamplesGenerator
    {
        bool Handles(string tag);

        IEnumerable<IEnumerable<string>> GetRows(string args, string path, ExamplesHeader header);
    }
}