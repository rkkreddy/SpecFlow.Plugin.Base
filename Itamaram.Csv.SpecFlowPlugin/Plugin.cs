using Itamaram.Csv.SpecFlowPlugin;
using Itamaram.SpecFlow.Plugin.Base;
using TechTalk.SpecFlow.Infrastructure;

[assembly: GeneratorPlugin(typeof(Plugin))]

namespace Itamaram.Csv.SpecFlowPlugin
{
    public class Plugin : ExamplesGeneratorPlugin<CsvSource> { }
}