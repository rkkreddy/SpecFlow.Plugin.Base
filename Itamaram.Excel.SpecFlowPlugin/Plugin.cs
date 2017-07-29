using Itamaram.Excel.SpecFlowPlugin;
using Itamaram.SpecFlow.Plugin.Base;
using TechTalk.SpecFlow.Infrastructure;

[assembly: GeneratorPlugin(typeof(Plugin))]

namespace Itamaram.Excel.SpecFlowPlugin
{
    public class Plugin : ExamplesGeneratorPlugin<ExcelSource> { }
}