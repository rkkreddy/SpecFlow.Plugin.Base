using System;
using System.Collections.Generic;
using System.Linq;
using BoDi;

namespace Itamaram.SpecFlow.Plugin.Base
{
    public class ExamplesGeneratorContainer
    {
        private readonly IObjectContainer container;
        private readonly HashSet<Type> types = new HashSet<Type>();

        public ExamplesGeneratorContainer(IObjectContainer container) => this.container = container;

        public void RegisterGenerator<T>() where T : ExamplesGenerator => types.Add(typeof(T));

        public IEnumerable<ExamplesGenerator> GetGenerators()
        {
            return types
                .Select(t => container.Resolve(t))
                .Cast<ExamplesGenerator>();
        }
    }
}