using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Plugins;

namespace Itamaram.SpecFlow.Plugin.Base
{
    public abstract class ExamplesGeneratorPlugin<T> : IGeneratorPlugin where T : ExamplesGenerator
    {
        public void Initialize(GeneratorPluginEvents events, GeneratorPluginParameters parameters)
        {
            events.CustomizeDependencies += (_, args) =>
            {
                args.ObjectContainer.RegisterTypeAs<Generator, ITestGenerator>();
                args.ObjectContainer.Resolve<ExamplesGeneratorContainer>().RegisterGenerator<T>();
            };

            InitializeInternal(events, parameters);
        }

        public virtual void InitializeInternal(GeneratorPluginEvents events, GeneratorPluginParameters parameters) { }
    }
}