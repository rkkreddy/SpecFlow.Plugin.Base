using System;
using System.Collections.Generic;
using System.IO;

namespace Itamaram.SpecFlow.Plugin.Base
{
    public abstract class NamedTagExampleGenerator : ExamplesGenerator
    {
        protected abstract string TagName { get; }

        public bool Handles(string tag) => tag.StartsWith(TagName, StringComparison.OrdinalIgnoreCase);

        public IEnumerable<IEnumerable<string>> GetRows(string args, string path, ExamplesHeader header)
        {
            var file = new FileInfo(path);
            args = args.Length > TagName.Length + 1 ? args.Substring(TagName.Length + 1) : null;
            return GetRowsInternal(args, file.DirectoryName, file.Name, header);
        }

        public abstract IEnumerable<IEnumerable<string>> GetRowsInternal(string args, string dir, string specfile, ExamplesHeader header);
    }
}